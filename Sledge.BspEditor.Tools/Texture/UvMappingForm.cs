using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LogicAndTrick.Oy;
using Sledge.BspEditor.Documents;
using Sledge.BspEditor.Environment;
using Sledge.BspEditor.Modification;
using Sledge.BspEditor.Modification.Operations.Data;
using Sledge.BspEditor.Primitives;
using Sledge.BspEditor.Primitives.MapData;
using Sledge.BspEditor.Primitives.MapObjectData;
using Sledge.BspEditor.Primitives.MapObjects;
using Sledge.Common.Shell.Components;
using Sledge.Common.Shell.Context;
using Sledge.Common.Shell.Documents;
using Sledge.Common.Translations;
using Sledge.DataStructures.Geometric;
using Sledge.Providers.Texture;
using Sledge.Shell;
using Sledge.Shell.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Sledge.BspEditor.Tools.Texture
{
    /// <summary>
    /// A sub window of the texture application: shows the texture of the selected faces with their
    /// UV layout drawn on top, and lets the user edit the UVs (shift, scale, rotation, axes) either
    /// numerically or by dragging the layout around in the preview.
    /// </summary>
    [Export(typeof(IDialog))]
    [AutoTranslate]
    public partial class UvMappingForm : BaseForm, IDialog, IManualTranslate
    {
        private const string ContextId = "BspEditor:UvMapping";

        // We want to use the shell for the parent
        [Import("Shell", typeof(Form))] private Form _shell;

        private WeakReference<MapDocument> _document = new WeakReference<MapDocument>(null);

        // The texture shown behind the UV layout
        private TextureCollection _collection;
        private ITextureStreamSource _streamSource;
        private Image _textureImage;
        private string _textureName;
        private int _textureWidth;
        private int _textureHeight;

        // Field state: a value plus a flag saying whether all selected faces agree on it.
        // Values are only pushed to the faces when the flag is false (i.e. the user edited the box).
        private bool _freeze;
        private float _shiftX;
        private float _shiftY;
        private float _scaleX;
        private float _scaleY;
        private float _rotation;
        private bool _diffShiftX;
        private bool _diffShiftY;
        private bool _diffScaleX;
        private bool _diffScaleY;
        private bool _diffRotation;

        // 1 while a debounced field edit is waiting to be committed to the undo history
        private int _pendingCommit;
        private readonly Timer _commitTimer;

        // Keeps overlapping edits (field commit, drag, align...) from corrupting each other
        private readonly SemaphoreSlim _applyLock = new SemaphoreSlim(1, 1);

        // View state: pixels per texture tile, and the panel position of UV (0, 0)
        private float _viewScale = 128f;
        private PointF _viewOffset = PointF.Empty;
        private string _fitSignature;

        // Drag state: shift deltas (in texture units) applied while dragging
        private bool _dragging;
        private Point _dragStart;
        private float _dragShiftX;
        private float _dragShiftY;

        private bool _dark;

        // Translated status strings (with fallbacks)
        private string _statusNoSelection = "No faces selected";
        private string _statusFormat = "{0} ({1} x {2}) - {3} faces";

        // The UV vector texts currently shown, so we can tell when the user actually edited one
        private readonly string[] _uvShown = new string[6];

        public MapDocument Document
        {
            get
            {
                MapDocument d;
                return _document.TryGetTarget(out d) ? d : null;
            }
        }

        public UvMappingForm()
        {
            InitializeComponent();

            _commitTimer = new Timer { Interval = 500 };
            _commitTimer.Tick += CommitTimerTick;

            ShiftXValue.ValueChanged += UvValueChanged;
            ShiftYValue.ValueChanged += UvValueChanged;
            ScaleXValue.ValueChanged += UvValueChanged;
            ScaleYValue.ValueChanged += UvValueChanged;
            RotationValue.ValueChanged += UvValueChanged;

            RotMinus90Button.Click += RotateQuickly;
            RotPlus90Button.Click += RotateQuickly;

            foreach (var box in UvBoxes())
            {
                box.KeyDown += UvKeyDown;
                box.Leave += UvLeave;
            }

            AlignFaceButton.Click += AlignToFaceClicked;
            AlignWorldButton.Click += AlignToWorldClicked;
            FitTextureButton.Click += FitTextureClicked;
            FitViewButton.Click += (s, e) => FitView(GetSelectedFaces().Select(x => x.Value).ToList());

            PreviewPanel.Paint += DrawPreview;
            PreviewPanel.MouseEnter += (s, e) => PreviewPanel.Focus();
            PreviewPanel.MouseDown += PreviewMouseDown;
            PreviewPanel.MouseMove += PreviewMouseMove;
            PreviewPanel.MouseUp += PreviewMouseUp;
            PreviewPanel.MouseWheel += PreviewWheel;

            Oy.Subscribe<IDocument>("Document:Activated", SetDocument);
            Oy.Subscribe<Change>("MapDocument:Changed", DocumentChanged);
            Oy.Subscribe<object>("TextureTool:SelectionChanged", SelectionChanged);
        }

        #region Visibility and translation

        public bool IsInContext(IContext context)
        {
            // The window is a sub window of the texture application: it only exists while the
            // texture tool is active and the user has opened it.
            return context.HasAny(ContextId) && context.TryGet("ActiveTool", out TextureTool _);
        }

        public void SetVisible(IContext context, bool visible)
        {
            if (Visible == visible) return;
            if (visible)
            {
                Show(_shell);
                _shell?.Focus();
                _fitSignature = null;
                _ = RefreshAsync();
            }
            else
            {
                Hide();
                // The texture tool went away (or the window was closed): drop the context flag so
                // the window doesn't pop up again the next time the texture tool is activated.
                Oy.Publish("Context:Remove", new ContextInfo(ContextId));
            }
        }

        public void UseDarkTheme(bool dark)
        {
            _dark = dark;
            PreviewPanel?.Invalidate();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // Don't actually close: hide and drop the context flag instead (same as the other dialogs)
            e.Cancel = true;
            if (Volatile.Read(ref _pendingCommit) == 1)
            {
                _ = ApplyChanges((mo, f) => Task.FromResult(false));
            }
            Hide();
            Oy.Publish("Context:Remove", new ContextInfo(ContextId));
            _shell?.Focus();
        }

        public void Translate(ITranslationStringProvider strings)
        {
            if (Handle == null) CreateHandle();
            var prefix = GetType().FullName;
            this.InvokeLater(() =>
            {
                Text = strings.GetString(prefix, "Title");
                ValuesGroup.Text = strings.GetString(prefix, "Values");
                AxesGroup.Text = strings.GetString(prefix, "Axes");
                ShiftLabel.Text = strings.GetString(prefix, "Shift");
                ScaleLabel.Text = strings.GetString(prefix, "Scale");
                RotationLabel.Text = strings.GetString(prefix, "Rotation");
                UAxisLabel.Text = strings.GetString(prefix, "UAxis");
                VAxisLabel.Text = strings.GetString(prefix, "VAxis");
                AlignFaceButton.Text = strings.GetString(prefix, "AlignFace");
                AlignWorldButton.Text = strings.GetString(prefix, "AlignWorld");
                FitTextureButton.Text = strings.GetString(prefix, "Fit");
                FitViewButton.Text = strings.GetString(prefix, "FitView");
                HintLabel.Text = strings.GetString(prefix, "Hint");
                _statusNoSelection = strings.GetString(prefix, "NoSelection") ?? _statusNoSelection;
                _statusFormat = strings.GetString(prefix, "Status") ?? _statusFormat;
            });
        }

        #endregion

        #region Document and selection tracking

        private async Task SetDocument(IDocument doc)
        {
            var md = doc as MapDocument;
            _document = new WeakReference<MapDocument>(md);
            _textureName = null;
            _fitSignature = null;
            await RefreshAsync();
        }

        private async Task DocumentChanged(Change change)
        {
            var doc = Document;
            if (doc == null || !ReferenceEquals(change.Document, doc)) return;

            if (change.HasObjectChanges && change.Updated.Intersect(GetSelectedParents()).Any())
            {
                await RefreshAsync();
            }
            else if (change.HasDataChanges && change.AffectedData.Any(x => x is FaceSelection))
            {
                await RefreshAsync();
            }
        }

        private async Task SelectionChanged(object o)
        {
            await RefreshAsync();
        }

        private FaceSelection GetFaceSelection()
        {
            var doc = Document;
            var fs = doc.Map.Data.GetOne<FaceSelection>();
            if (fs == null)
            {
                fs = new FaceSelection();
                doc.Map.Data.Add(fs);
            }
            return fs;
        }

        private List<KeyValuePair<IMapObject, Face>> GetSelectedFaces()
        {
            if (Document == null) return new List<KeyValuePair<IMapObject, Face>>();
            return GetFaceSelection().GetSelectedFaces().ToList();
        }

        private IEnumerable<IMapObject> GetSelectedParents()
        {
            if (Document == null) return Enumerable.Empty<IMapObject>();
            return GetFaceSelection().GetSelectedParents().ToList();
        }

        #endregion

        #region Refreshing the window

        private async Task RefreshAsync()
        {
            var doc = Document;
            var faces = GetSelectedFaces();

            // Texture info comes from the first selected face
            var name = faces.FirstOrDefault().Value?.Texture.Name;
            if (doc != null && !String.IsNullOrWhiteSpace(name))
            {
                var tc = await doc.Environment.GetTextureCollection();
                await EnsureTexture(tc, name);
            }
            else
            {
                ClearTexture();
            }

            if (IsDisposed) return;

            // While a field edit is still being typed in, don't overwrite what the user is writing
            var pending = Volatile.Read(ref _pendingCommit) == 1;

            this.InvokeLater(() =>
            {
                if (IsDisposed) return;

                if (!pending) UpdateFields(faces.Select(x => x.Value).ToList());
                UpdateStatus(faces);

                var signature = String.Join(",", faces.Select(x => x.Value.ID)) + "|" + (_textureName ?? "");
                if (signature != _fitSignature)
                {
                    _fitSignature = signature;
                    FitView(faces.Select(x => x.Value).ToList());
                }

                PreviewPanel.Cursor = faces.Count > 0 ? Cursors.SizeAll : Cursors.Default;
                PreviewPanel.Invalidate();
            });
        }

        private async Task EnsureTexture(TextureCollection tc, string name)
        {
            if (!ReferenceEquals(_collection, tc))
            {
                _streamSource?.Dispose();
                _streamSource = null;
                _collection = tc;
                _textureName = null;
            }

            // Already loaded (or currently loading) this texture
            if (String.Equals(_textureName, name, StringComparison.InvariantCultureIgnoreCase)) return;

            _textureName = name;
            _textureImage = null;
            _textureWidth = _textureHeight = 0;

            var item = await tc.GetTextureItem(name);
            _textureWidth = item?.Width ?? 0;
            _textureHeight = item?.Height ?? 0;

            if (_streamSource == null) _streamSource = tc.GetStreamSource();
            var imageTask = _streamSource.GetImage(name, 512, 512);
            var images = imageTask == null ? null : await imageTask;

            // A newer refresh may have asked for a different texture while we waited
            if (!String.Equals(_textureName, name, StringComparison.InvariantCultureIgnoreCase)) return;
            _textureImage = images?.FirstOrDefault();
        }

        private void ClearTexture()
        {
            _textureName = null;
            _textureImage = null;
            _textureWidth = _textureHeight = 0;
        }

        private void UpdateFields(List<Face> faces)
        {
            _freeze = true;
            try
            {
                var has = faces.Count > 0;
                ValuesGroup.Enabled = has;
                AxesGroup.Enabled = has;
                AlignFaceButton.Enabled = has;
                AlignWorldButton.Enabled = has;
                FitTextureButton.Enabled = has;

                _diffShiftX = _diffShiftY = _diffScaleX = _diffScaleY = _diffRotation = false;
                _shiftX = _shiftY = 0;
                _scaleX = _scaleY = 1;
                _rotation = 0;

                var num = 0;
                foreach (var face in faces)
                {
                    var t = face.Texture;
                    var rawXScale = t.XScale * t.ULength;
                    var rawYScale = t.YScale * t.VLength;

                    if (num == 0)
                    {
                        if (!float.IsNaN(rawXScale)) _scaleX = rawXScale;
                        if (!float.IsNaN(rawYScale)) _scaleY = rawYScale;
                        if (!float.IsNaN(t.XShift)) _shiftX = t.XShift;
                        if (!float.IsNaN(t.YShift)) _shiftY = t.YShift;
                        if (!float.IsNaN(t.Rotation)) _rotation = t.Rotation;
                    }
                    else
                    {
                        if (rawXScale != _scaleX) _diffScaleX = true;
                        if (rawYScale != _scaleY) _diffScaleY = true;
                        if (t.XShift != _shiftX) _diffShiftX = true;
                        if (t.YShift != _shiftY) _diffShiftY = true;
                        if (t.Rotation != _rotation) _diffRotation = true;
                    }
                    num++;
                }

                // Same trick as the texture application form: when the faces disagree the box is
                // blank, and a value the box doesn't actually hold must still trigger a change.
                if (_diffScaleX) _scaleX = 1.000001f;
                if (_diffScaleY) _scaleY = 1.000001f;

                SetNumericValue(ShiftXValue, _shiftX);
                SetNumericValue(ShiftYValue, _shiftY);
                SetNumericValue(ScaleXValue, _scaleX);
                SetNumericValue(ScaleYValue, _scaleY);
                SetNumericValue(RotationValue, _rotation);

                if (_diffShiftX) ShiftXValue.Text = "";
                if (_diffShiftY) ShiftYValue.Text = "";
                if (_diffScaleX) ScaleXValue.Text = "";
                if (_diffScaleY) ScaleYValue.Text = "";
                if (_diffRotation) RotationValue.Text = "";

                SetUvTexts(GetUvVectorTexts(faces));

                if (!has)
                {
                    foreach (var box in UvBoxes()) box.Text = "";
                }
            }
            finally
            {
                _freeze = false;
            }
        }

        private void UpdateStatus(List<KeyValuePair<IMapObject, Face>> faces)
        {
            if (faces.Count == 0)
            {
                StatusLabel.Text = _statusNoSelection;
                return;
            }

            var name = faces[0].Value.Texture.Name;
            StatusLabel.Text = String.Format(_statusFormat, name, _textureWidth, _textureHeight, faces.Count);
        }

        #endregion

        #region Applying edits

        /// <summary>
        /// The edits currently made in the numeric fields, as a function over a face's texture.
        /// Fields the user didn't touch are skipped (their "different" flag is still set), so
        /// every face keeps its own value for those.
        /// </summary>
        private Action<Primitives.Texture> BuildValueEdits()
        {
            var shiftX = _shiftX;
            var shiftY = _shiftY;
            var scaleX = _scaleX;
            var scaleY = _scaleY;
            var rotation = _rotation;
            var diffShiftX = _diffShiftX;
            var diffShiftY = _diffShiftY;
            var diffScaleX = _diffScaleX;
            var diffScaleY = _diffScaleY;
            var diffRotation = _diffRotation;

            return t =>
            {
                if (!diffShiftX) t.XShift = shiftX;
                if (!diffShiftY) t.YShift = shiftY;
                if (!diffScaleX) t.XScale = scaleX / SafeLength(t.ULength);
                if (!diffScaleY) t.YScale = scaleY / SafeLength(t.VLength);
                if (!diffRotation) t.SetRotation(rotation);
            };
        }

        private void UvValueChanged(object sender, EventArgs e)
        {
            if (_freeze) return;

            if (ReferenceEquals(sender, ShiftXValue)) { _diffShiftX = false; _shiftX = (float)ShiftXValue.Value; }
            else if (ReferenceEquals(sender, ShiftYValue)) { _diffShiftY = false; _shiftY = (float)ShiftYValue.Value; }
            else if (ReferenceEquals(sender, ScaleXValue)) { _diffScaleX = false; _scaleX = (float)ScaleXValue.Value; }
            else if (ReferenceEquals(sender, ScaleYValue)) { _diffScaleY = false; _scaleY = (float)ScaleYValue.Value; }
            else if (ReferenceEquals(sender, RotationValue)) { _diffRotation = false; _rotation = (float)RotationValue.Value; }

            // Throttle field changes so they only apply after 500 ms, this should keep the undo stack clear
            Interlocked.Exchange(ref _pendingCommit, 1);
            _commitTimer.Stop();
            _commitTimer.Start();

            // The preview draws with the field values, so it updates immediately
            PreviewPanel.Invalidate();
        }

        private void CommitTimerTick(object sender, EventArgs e)
        {
            _commitTimer.Stop();
            CommitPendingNow();
        }

        /// <summary>
        /// Commits a field edit that's still waiting in the debounce window (fire and forget).
        /// </summary>
        private void CommitPendingNow()
        {
            if (Volatile.Read(ref _pendingCommit) == 0) return;
            _ = ApplyChanges((mo, f) => Task.FromResult(false));
        }

        /// <summary>
        /// Applies an edit to every selected face as a single undoable operation.
        /// A pending (debounced) field edit is committed first so the two can't overlap,
        /// and the fields/preview are refreshed afterwards.
        /// </summary>
        private async Task ApplyChanges(Func<IMapObject, Face, Task<bool>> apply)
        {
            var doc = Document;
            if (doc == null) return;

            await _applyLock.WaitAsync();
            try
            {
                if (Interlocked.Exchange(ref _pendingCommit, 0) == 1)
                {
                    _commitTimer.Stop();
                    var edits = BuildValueEdits();
                    await ApplyCore(doc, (mo, f) => { edits(f.Texture); return Task.FromResult(true); });
                }

                await ApplyCore(doc, apply);
            }
            finally
            {
                _applyLock.Release();
            }

            await RefreshAsync();
        }

        // Caller must hold _applyLock
        private async Task ApplyCore(MapDocument doc, Func<IMapObject, Face, Task<bool>> apply)
        {
            var sel = GetFaceSelection();
            var edit = new Transaction();
            var found = false;

            // The faces are looked up by ID, so clone them now and swap them in
            foreach (var it in sel.GetSelectedFaces().ToList())
            {
                var clone = (Face)it.Value.Clone();
                var result = await apply(it.Key, clone);
                if (!result) continue;

                found = true;
                edit.Add(new RemoveMapObjectData(it.Key.ID, it.Value));
                edit.Add(new AddMapObjectData(it.Key.ID, clone));
            }

            if (found) await MapDocumentOperation.Perform(doc, edit);
        }

        private async void RotateQuickly(object sender, EventArgs e)
        {
            if (_freeze) return;
            if (!(sender is Button button) || !(button.Tag is float delta)) return;

            await ApplyChanges((mo, f) =>
            {
                f.Texture.SetRotation(f.Texture.Rotation + delta);
                return Task.FromResult(true);
            });
        }

        private async void AlignToFaceClicked(object sender, EventArgs e)
        {
            await ApplyChanges((mo, f) =>
            {
                f.Texture.AlignToNormal(f.Plane.Normal);
                return Task.FromResult(true);
            });
        }

        private async void AlignToWorldClicked(object sender, EventArgs e)
        {
            await ApplyChanges((mo, f) =>
            {
                f.Texture.AlignToNormal(f.Plane.GetClosestAxisToNormal());
                return Task.FromResult(true);
            });
        }

        private async void FitTextureClicked(object sender, EventArgs e)
        {
            var doc = Document;
            if (doc == null) return;

            var tc = await doc.Environment.GetTextureCollection();
            await ApplyChanges(async (mo, f) =>
            {
                var ti = await tc.GetTextureItem(f.Texture.Name);
                if (ti == null) return false;
                f.Texture.FitToPointCloud(ti.Width, ti.Height, new Cloud(f.Vertices), 1, 1);
                return true;
            });
        }

        #endregion

        #region UV vector boxes

        private TextBox[] UvBoxes()
        {
            return new[] { UvUX, UvUY, UvUZ, UvVX, UvVY, UvVZ };
        }

        private void SetUvTexts(string[] texts)
        {
            var boxes = UvBoxes();
            for (var i = 0; i < boxes.Length; i++)
            {
                boxes[i].Text = texts[i];
                _uvShown[i] = texts[i];
            }
        }

        private void RestoreUvRow(int start)
        {
            var boxes = UvBoxes();
            for (var i = start; i < start + 3; i++) boxes[i].Text = _uvShown[i];
        }

        private void UvKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            e.Handled = true;
            CommitUvRow(sender as TextBox);
        }

        private void UvLeave(object sender, EventArgs e)
        {
            CommitUvRow(sender as TextBox);
        }

        private static bool TryParseUv(string text, out float value)
        {
            text = (text ?? "").Trim();
            return (float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out value)
                    || float.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out value))
                   && float.IsFinite(value);
        }

        // Applies an edited U (top row) or V (bottom row) vector to every selected face. The vector is normalised by the texture.
        private async void CommitUvRow(TextBox source)
        {
            var boxes = UvBoxes();
            var index = Array.IndexOf(boxes, source);
            if (index < 0) return;

            var start = index < 3 ? 0 : 3;
            var isU = start == 0;

            var changed = false;
            for (var i = start; i < start + 3; i++)
            {
                if (boxes[i].Text != _uvShown[i]) changed = true;
            }
            if (!changed) return;

            var parts = new float[3];
            for (var i = 0; i < 3; i++)
            {
                if (!TryParseUv(boxes[start + i].Text, out parts[i]))
                {
                    RestoreUvRow(start);
                    return;
                }
            }

            var vector = new System.Numerics.Vector3(parts[0], parts[1], parts[2]);
            if (!(vector.LengthSquared() > 1e-8f))
            {
                RestoreUvRow(start);
                return;
            }

            // The typed numbers are the raw vector, exactly as in Hammer: its length stays with the axis and the scale box keeps
            // meaning the raw scale. The texture draws with unit axes, so the effective scale is rawScale / length.
            var length = vector.Length();

            // Mark as shown straight away so Enter followed by Leave doesn't apply it twice
            for (var i = start; i < start + 3; i++) _uvShown[i] = boxes[i].Text;

            await ApplyChanges((mo, f) =>
            {
                if (isU)
                {
                    var rawScale = f.Texture.XScale * f.Texture.ULength;
                    f.Texture.UAxis = vector;
                    f.Texture.ULength = length;
                    f.Texture.XScale = rawScale / length;
                }
                else
                {
                    var rawScale = f.Texture.YScale * f.Texture.VLength;
                    f.Texture.VAxis = vector;
                    f.Texture.VLength = length;
                    f.Texture.YScale = rawScale / length;
                }

                return Task.FromResult(true);
            });
        }

        private static string[] GetUvVectorTexts(IEnumerable<Face> faces)
        {
            var texts = new string[6];
            System.Numerics.Vector3? u = null, v = null;
            var uDiffers = false;
            var vDiffers = false;

            foreach (var face in faces)
            {
                var t = face.Texture;
                var rawU = t.UAxis * t.ULength;
                var rawV = t.VAxis * t.VLength;
                if (u == null)
                {
                    u = rawU;
                    v = rawV;
                    continue;
                }

                if ((u.Value - rawU).LengthSquared() > 1e-6f) uDiffers = true;
                if ((v.Value - rawV).LengthSquared() > 1e-6f) vDiffers = true;
            }

            string Fmt(float f) => (Math.Abs(f) < 0.00005f ? 0f : f).ToString("0.####", System.Globalization.CultureInfo.InvariantCulture);

            if (u != null && !uDiffers) { texts[0] = Fmt(u.Value.X); texts[1] = Fmt(u.Value.Y); texts[2] = Fmt(u.Value.Z); }
            if (v != null && !vDiffers) { texts[3] = Fmt(v.Value.X); texts[4] = Fmt(v.Value.Y); texts[5] = Fmt(v.Value.Z); }
            for (var i = 0; i < texts.Length; i++) texts[i] = texts[i] ?? "";
            return texts;
        }

        #endregion

        #region Preview and dragging

        /// <summary>
        /// The screen position of each vertex of the face's UV layout, with the current field
        /// edits and any active drag applied, so the preview matches what will be committed.
        /// </summary>
        private bool TryGetFaceUv(Face face, out PointF[] uvs)
        {
            uvs = null;
            if (_textureWidth <= 0 || _textureHeight <= 0 || face.Vertices.Count == 0) return false;

            var t = face.Texture.Clone();
            BuildValueEdits()(t);
            if (_dragging)
            {
                t.XShift += _dragShiftX;
                t.YShift += _dragShiftY;
            }
            if (t.XScale == 0 || t.YScale == 0) return false;

            var points = new PointF[face.Vertices.Count];
            for (var i = 0; i < face.Vertices.Count; i++)
            {
                var v = face.Vertices[i];
                var u = Vector3.Dot(v, t.UAxis) / (_textureWidth * t.XScale) + t.XShift / _textureWidth;
                var w = Vector3.Dot(v, t.VAxis) / (_textureHeight * t.YScale) + t.YShift / _textureHeight;
                if (!float.IsFinite(u) || !float.IsFinite(w)) return false;
                points[i] = new PointF(_viewOffset.X + u * _viewScale, _viewOffset.Y + w * _viewScale);
            }

            uvs = points;
            return true;
        }

        private void DrawPreview(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(_dark ? Color.FromArgb(32, 32, 32) : Color.Black);
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;

            var width = PreviewPanel.ClientSize.Width;
            var height = PreviewPanel.ClientSize.Height;
            var scale = _viewScale;

            // Tiles covering the visible area (span clamped so a small scale can't flood the loop)
            var rawMinTx = (int)Math.Floor(-_viewOffset.X / scale) - 1;
            var rawMaxTx = (int)Math.Ceiling((width - _viewOffset.X) / scale) + 1;
            var rawMinTy = (int)Math.Floor(-_viewOffset.Y / scale) - 1;
            var rawMaxTy = (int)Math.Ceiling((height - _viewOffset.Y) / scale) + 1;
            var minTx = Math.Max(rawMinTx, rawMaxTx - 33);
            var minTy = Math.Max(rawMinTy, rawMaxTy - 33);

            if (_textureImage != null)
            {
                for (var ty = minTy; ty <= rawMaxTy; ty++)
                {
                    for (var tx = minTx; tx <= rawMaxTx; tx++)
                    {
                        var rect = new RectangleF(_viewOffset.X + tx * scale, _viewOffset.Y + ty * scale, scale, scale);
                        g.DrawImage(_textureImage, rect);
                    }
                }
            }

            // Tile grid
            using (var gridPen = new Pen(Color.FromArgb(70, 255, 255, 255)))
            {
                for (var tx = minTx; tx <= rawMaxTx + 1; tx++)
                {
                    var x = _viewOffset.X + tx * scale;
                    if (x >= -1 && x <= width + 1) g.DrawLine(gridPen, x, 0, x, height);
                }
                for (var ty = minTy; ty <= rawMaxTy + 1; ty++)
                {
                    var y = _viewOffset.Y + ty * scale;
                    if (y >= -1 && y <= height + 1) g.DrawLine(gridPen, 0, y, width, y);
                }
            }

            // The UV origin
            using (var axisPen = new Pen(Color.FromArgb(160, 255, 210, 0)))
            {
                var ox = _viewOffset.X;
                var oy = _viewOffset.Y;
                if (ox >= 0 && ox <= width) g.DrawLine(axisPen, ox, 0, ox, height);
                if (oy >= 0 && oy <= height) g.DrawLine(axisPen, 0, oy, width, oy);
            }

            var faces = GetSelectedFaces();

            // The UV layout of every selected face
            using (var polyPen = new Pen(Color.FromArgb(0, 255, 0)) { LineJoin = LineJoin.Round })
            using (var vertexBrush = new SolidBrush(Color.FromArgb(0, 255, 0)))
            {
                foreach (var kv in faces)
                {
                    if (!TryGetFaceUv(kv.Value, out var points)) continue;
                    g.DrawPolygon(polyPen, points);
                    foreach (var p in points)
                    {
                        g.FillRectangle(vertexBrush, p.X - 2.5f, p.Y - 2.5f, 5f, 5f);
                    }
                }
            }

            if (faces.Count == 0)
            {
                var message = _statusNoSelection;
                var size = g.MeasureString(message, Font);
                g.DrawString(message, Font, Brushes.Gray, (width - size.Width) / 2f, (height - size.Height) / 2f);
            }
        }

        private async void PreviewMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (GetSelectedFaces().Count == 0) return;

            PreviewPanel.Focus();

            // Apply anything typed but not committed yet, so the drag starts from the committed state
            if (Volatile.Read(ref _pendingCommit) == 1)
            {
                await ApplyChanges((mo, f) => Task.FromResult(false));
            }

            _dragging = true;
            _dragStart = e.Location;
            _dragShiftX = 0;
            _dragShiftY = 0;
            PreviewPanel.Capture = true;
        }

        private void PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;

            // Dragging moves the UV layout: one screen pixel is 1 / viewScale of a tile,
            // and a shift of one texture-width units is one whole tile in U.
            _dragShiftX = (e.X - _dragStart.X) / _viewScale * Math.Max(1, _textureWidth);
            _dragShiftY = (e.Y - _dragStart.Y) / _viewScale * Math.Max(1, _textureHeight);

            UpdateShiftFieldsDuringDrag();
            PreviewPanel.Invalidate();
        }

        private void UpdateShiftFieldsDuringDrag()
        {
            var faces = GetSelectedFaces();
            if (faces.Count == 0) return;

            _freeze = true;
            try
            {
                SetNumericValue(ShiftXValue, faces[0].Value.Texture.XShift + _dragShiftX);
                SetNumericValue(ShiftYValue, faces[0].Value.Texture.YShift + _dragShiftY);
            }
            finally
            {
                _freeze = false;
            }
        }

        private async void PreviewMouseUp(object sender, MouseEventArgs e)
        {
            if (!_dragging) return;
            _dragging = false;
            PreviewPanel.Capture = false;

            var dx = _dragShiftX;
            var dy = _dragShiftY;
            _dragShiftX = 0;
            _dragShiftY = 0;

            if (Math.Abs(dx) < 0.0001f && Math.Abs(dy) < 0.0001f)
            {
                PreviewPanel.Invalidate();
                return;
            }

            await ApplyChanges((mo, f) =>
            {
                f.Texture.XShift += dx;
                f.Texture.YShift += dy;
                return Task.FromResult(true);
            });
        }

        private void PreviewWheel(object sender, MouseEventArgs e)
        {
            var oldScale = _viewScale;
            var newScale = e.Delta > 0 ? oldScale * 1.25f : oldScale / 1.25f;
            newScale = Math.Clamp(newScale, 16f, 4096f);
            if (Math.Abs(newScale - oldScale) < 0.01f) return;

            // Keep the UV under the cursor in the same place
            var uvx = (e.X - _viewOffset.X) / oldScale;
            var uvy = (e.Y - _viewOffset.Y) / oldScale;
            _viewScale = newScale;
            _viewOffset = new PointF(e.X - uvx * newScale, e.Y - uvy * newScale);
            PreviewPanel.Invalidate();
        }

        /// <summary>
        /// Frames the UV layout of the given faces in the preview (or centres a single tile
        /// when nothing is selected).
        /// </summary>
        private void FitView(List<Face> faces)
        {
            var width = PreviewPanel.ClientSize.Width;
            var height = PreviewPanel.ClientSize.Height;
            if (width <= 0 || height <= 0) return;

            float minU = float.MaxValue, minV = float.MaxValue, maxU = float.MinValue, maxV = float.MinValue;
            var any = false;

            if (_textureWidth > 0 && _textureHeight > 0)
            {
                foreach (var face in faces)
                {
                    var t = face.Texture;
                    if (t.XScale == 0 || t.YScale == 0) continue;

                    foreach (var v in face.Vertices)
                    {
                        var u = Vector3.Dot(v, t.UAxis) / (_textureWidth * t.XScale) + t.XShift / _textureWidth;
                        var w = Vector3.Dot(v, t.VAxis) / (_textureHeight * t.YScale) + t.YShift / _textureHeight;
                        if (!float.IsFinite(u) || !float.IsFinite(w)) continue;
                        minU = Math.Min(minU, u);
                        maxU = Math.Max(maxU, u);
                        minV = Math.Min(minV, w);
                        maxV = Math.Max(maxV, w);
                        any = true;
                    }
                }
            }

            if (!any)
            {
                _viewScale = Math.Clamp(Math.Min(width, height) * 0.5f, 16f, 4096f);
                _viewOffset = new PointF(width / 2f - _viewScale / 2f, height / 2f - _viewScale / 2f);
                PreviewPanel.Invalidate();
                return;
            }

            var du = Math.Max(maxU - minU, 1e-4f);
            var dv = Math.Max(maxV - minV, 1e-4f);
            const float margin = 24f;
            var fit = Math.Min((width - margin * 2f) / du, (height - margin * 2f) / dv);
            _viewScale = Math.Clamp(fit, 16f, 4096f);
            _viewOffset = new PointF((width - du * _viewScale) / 2f - minU * _viewScale, (height - dv * _viewScale) / 2f - minV * _viewScale);
            PreviewPanel.Invalidate();
        }

        #endregion

        #region Helpers

        // A face can hold a value the box doesn't allow, so clamp instead of throwing
        private static void SetNumericValue(NumericUpDown box, float value)
        {
            var clamped = float.IsFinite(value) ? Math.Clamp(value, (float)box.Minimum, (float)box.Maximum) : (float)box.Minimum;
            box.Value = Math.Clamp((decimal)clamped, box.Minimum, box.Maximum);
        }

        private static float SafeLength(float length)
        {
            return length > 0.0001f ? length : 1f;
        }

        private static bool IsUndoRedoKey(Keys keyData)
        {
            if (!keyData.HasFlag(Keys.Control) || keyData.HasFlag(Keys.Alt)) return false;
            var key = keyData & Keys.KeyCode;
            return key == Keys.Z || key == Keys.Y;
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            // A change still inside the debounce window is committed first, so undo/redo sees it
            if (IsUndoRedoKey(keyData)) CommitPendingNow();
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool CheckIgnoreHotkey(Control source, Keys keyData)
        {
            // The numeric fields are text boxes, which normally keep Ctrl+Z / Ctrl+Y for their own
            // text undo. Here they should act on the map's undo history instead.
            if (IsUndoRedoKey(keyData)) return false;
            return base.CheckIgnoreHotkey(source, keyData);
        }

        #endregion

        /// <summary>
        /// The preview surface: double buffered and focusable, so the mouse wheel reaches it
        /// as soon as the pointer is over it.
        /// </summary>
        private class UvPreviewPanel : Panel
        {
            public UvPreviewPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.UserPaint |
                         ControlStyles.ResizeRedraw |
                         ControlStyles.Selectable, true);
                TabStop = true;
            }
        }
    }
}






