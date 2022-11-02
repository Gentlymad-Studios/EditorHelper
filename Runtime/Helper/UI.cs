#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorHelper {
    /// <summary>
    /// Helper class to handle Common Editor UI functions
    /// </summary>
    public static class UI {
        /// <summary>
        /// Draw UI in a disabled state
        /// </summary>
        /// <param name="content"></param>
        public static void DrawAsDisabled(Action content) {
            bool enabled = GUI.enabled;
            GUI.enabled = false;
            content();
            GUI.enabled = enabled;
        }
        /// <summary>
        /// Default bold label style
        /// </summary>
        public static GUIStyle boldLabelStyle {
            get {
                if (_boldLabelStyle == null) {
                    _boldLabelStyle = EditorStyles.boldLabel;
                }
                return _boldLabelStyle;
            }
        }
        private static GUIStyle _boldLabelStyle = null;

        /// <summary>
        /// Foldout style with bold label and aligned MiddleCenter
        /// </summary>
        public static GUIStyle FoldoutBoldStyle {
            get {
                if (_foldoutBoldStyle == null) {
                    _foldoutBoldStyle = new GUIStyle(EditorStyles.foldout);
                    //_foldoutMiddleBoldStyle.alignment = TextAnchor.MiddleCenter;
                    _foldoutBoldStyle.fontStyle = FontStyle.Bold;
                }
                return _foldoutBoldStyle;
            }
        }
        private static GUIStyle _foldoutBoldStyle = null;

        /// <summary>
        /// Foldout style with bold label and aligned MiddleCenter
        /// </summary>
        public static GUIStyle FoldoutWithoutLabel {
            get {
                if (_FoldoutWithoutLabel == null) {
                    _FoldoutWithoutLabel = new GUIStyle(EditorStyles.foldout);
                    _FoldoutWithoutLabel.fixedWidth = 10;
                }
                return _FoldoutWithoutLabel;
            }
        }
        private static GUIStyle _FoldoutWithoutLabel = null;

        public class SliderWithButtons {
            public float minValue = 0f;
            public float maxValue = 1f;
            public float buttonSensitivity = 1f;
            public string label;
            public Action OnValueChanged;

            private bool changed;

            public SliderWithButtons(string label, float minValue, float maxValue, float buttonSensitivity) {
                this.label = label;
                this.minValue = minValue;
                this.maxValue = maxValue;
                this.buttonSensitivity = buttonSensitivity;
            }

            public bool DrawSlider(ref float value) {
                changed = false;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                value = EditorGUILayout.Slider(label, value, minValue, maxValue);
                if (GUILayout.Button("-", GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)) && value > minValue) {
                    value -= buttonSensitivity;
                    changed = true;
                }
                if (GUILayout.Button("+", GUILayout.MaxWidth(18), GUILayout.MaxHeight(18)) && value < maxValue) {
                    value += buttonSensitivity;
                    changed = true;
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck() || changed) {
                    OnValueChanged();
                    changed = true;
                }
                return changed;
            }
        }

        public class FoldoutArea {
            public bool foldout;
            private string header = "";
            private bool initialized = false;
            private Color save;
            private static Color background = new Color(.5f, .5f, .5f);
            public Action doInsideArea = null;
            protected bool defaultState = false;

            public void SetupStyles() {
                if (!initialized) {
                    save = GUI.backgroundColor;
                    initialized = true;
                }
            }

            public FoldoutArea(Action doInsideArea, bool defaultState = false, string header = "") {
                this.header = header;
                this.defaultState = defaultState;
                this.doInsideArea = doInsideArea;
                SetFoldoutState(defaultState);
            }

            public void Draw(string overwriteHeader = null) {
                SetupStyles();
                GUI.backgroundColor = background;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = save;
                SetFoldoutState(EditorGUILayout.Foldout(GetFoldoutState(), overwriteHeader == null ? header : overwriteHeader, FoldoutBoldStyle));
                if (foldout) {
                    doInsideArea();
                }
                EditorGUILayout.EndVertical();
            }

            public virtual bool GetFoldoutState() {
                return foldout;
            }

            public virtual void SetFoldoutState(bool state) {
                foldout = state;
            }
        }

        public class EditorPrefsManagedFoldoutArea : FoldoutArea {
            public string foldoutPrefsKey;
            public EditorPrefsManagedFoldoutArea(Action doInsideArea, string foldoutPrefsKey, bool defaultState = false, string header = "") : base (doInsideArea, defaultState, header) {
                this.foldoutPrefsKey = foldoutPrefsKey;
            }
            public override void SetFoldoutState(bool state) {
                EditorPrefs.SetBool(foldoutPrefsKey, state);
                foldout = state;
            }

            public override bool GetFoldoutState() {
                return EditorPrefs.GetBool(foldoutPrefsKey, false);
            }
        }

        public class ReorderableListBase {
            protected ReorderableList list;
            protected SerializedProperty selectedProperty, tempIndexProperty;
            protected SerializedObject serializedObject;
            protected GUIContent labelContent;
            private bool addOrRemoveTriggered = false;

            public ReorderableListBase(SerializedObject serializedObject, SerializedProperty listProperty, bool draggable = true, bool displayHeader = true, bool displayAddButton = true, bool displayRemoveButton = true) {
                this.serializedObject = serializedObject;

                list = new ReorderableList(serializedObject, listProperty, draggable, displayHeader, displayAddButton, displayRemoveButton) {
                    drawElementCallback = OnElementDraw,
                    elementHeightCallback = OnElementHeight,
                    drawHeaderCallback = OnHeaderDraw,
                    onSelectCallback = OnSelect,
                    onAddCallback = OnAdd,
                    onRemoveCallback = OnRemove,
                    onChangedCallback = OnChange
                };

            }

            protected virtual void OnChange(ReorderableList list) {
                serializedObject.ApplyModifiedProperties();
            }

            protected virtual void OnRemove(ReorderableList list) {
                serializedObject.Update();
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                list.index = -1;
                serializedObject.ApplyModifiedProperties();
                addOrRemoveTriggered = true;
                selectedProperty = null;
            }

            protected virtual void OnAdd(ReorderableList list) {
                serializedObject.Update();
                ReorderableList.defaultBehaviours.DoAddButton(list);
                serializedObject.ApplyModifiedProperties();
                list.index = -1;
                addOrRemoveTriggered = true;
                selectedProperty = null;
            }

            protected virtual void OnSelect(ReorderableList list) {
                if (list.index >= 0) {
                    selectedProperty = list.serializedProperty.GetArrayElementAtIndex(list.index);
                } else {
                    selectedProperty = null;
                }
            }

            public int Index {
                get {
                    return list.index;
                }
            }

            protected virtual void OnHeaderDraw(Rect rect) {
                EditorGUI.LabelField(rect, list.serializedProperty.displayName, UI.boldLabelStyle);
            }

            protected virtual float OnElementHeight(int index) {
                float elementHeight = EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index));
                elementHeight += EditorGUIUtility.standardVerticalSpacing;
                elementHeight += list.serializedProperty.hasVisibleChildren ? 4 : 0;
                return elementHeight;
            }

            protected virtual void OnElementDraw(Rect rect, int index, bool isActive, bool isFocused) {
                // fix vertical misalignment
                rect.y += 2;
                // the foldout caret icon overlays the drag handler, so in case we have children, we adjust the x position to fix that
                if (list.serializedProperty.hasVisibleChildren) {
                    rect.x += 8;
                    rect.width -= 8;
                }

                // get the item at the current index
                if (labelContent == null) {
                    labelContent = new GUIContent();
                }
                tempIndexProperty = list.serializedProperty.GetArrayElementAtIndex(index);
                labelContent.text = tempIndexProperty.displayName;

                // do property field with our custom labelling
                EditorGUI.PropertyField(rect, tempIndexProperty, labelContent, true);
            }


            public virtual void DrawLayoutedInspector() {
                serializedObject.Update();
                EditorGUI.BeginChangeCheck();
                if (!addOrRemoveTriggered) {
                    list.DoLayoutList();
                }
                if (EditorGUI.EndChangeCheck()) {
                    serializedObject.ApplyModifiedProperties();
                }
                addOrRemoveTriggered = false;
            }
        }
        private static bool PropertyTypeHasDefaultCustomDrawer(SerializedPropertyType type) {
            return
            type == SerializedPropertyType.AnimationCurve ||
            type == SerializedPropertyType.Bounds ||
            type == SerializedPropertyType.Color ||
            type == SerializedPropertyType.Gradient ||
            type == SerializedPropertyType.LayerMask ||
            type == SerializedPropertyType.ObjectReference ||
            type == SerializedPropertyType.Rect ||
            type == SerializedPropertyType.Vector2 ||
            type == SerializedPropertyType.Vector3;
        }

        public class SerializedArrayDrawer {
            public SerializedProperty property, listProperty, endProperty;
            private string relativePropertyPath, elementHeader;
            private Color tmpColor;
            private int selectedIndex = -1;

            public SerializedArrayDrawer(string relativePropertyPath, string elementHeader) {
                this.relativePropertyPath = relativePropertyPath;
                this.elementHeader = elementHeader;
            }

            private void Update(SerializedProperty parentProperty) {
                if (relativePropertyPath != null) {
                    listProperty = parentProperty.FindPropertyRelative(relativePropertyPath);
                } else {
                    listProperty = parentProperty;
                }
            }

            public void ForEach(SerializedProperty parentProperty, Action<SerializedProperty> onElement, int selectedIndex = -1, Action<int> onBeforeProperty = null, Action<int> onAfterProperty = null) {
                Update(parentProperty);

                for (int i = 0; i < listProperty.arraySize; i++) {
                    property = listProperty.GetArrayElementAtIndex(i);
                    endProperty = property.GetEndProperty();
                    onBeforeProperty?.Invoke(i);
                    while (property.NextVisible(!PropertyTypeHasDefaultCustomDrawer(property.propertyType))) {
                        if (SerializedProperty.EqualContents(property, endProperty)) {
                            break;
                        }
                        if (property.name != "name") {
                            onElement(property);
                        }
                    }
                    onAfterProperty?.Invoke(i);
                }
            }

            private void OnBeforePropertyBlock(int index) {
                if (selectedIndex >= 0 && selectedIndex == index) {
                    tmpColor = GUI.color;
                    GUI.color = Color.yellow;
                }
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"{elementHeader} {index+1}", UI.boldLabelStyle);
            }

            private void OnAfterPropertyBlock(int index) {
                EditorGUILayout.EndVertical();
                if (selectedIndex >= 0 && selectedIndex == index) {
                    GUI.color = tmpColor;
                }
            }

            private void OnElement(SerializedProperty prop) {
                EditorGUILayout.PropertyField(property, false);
            }

            public void Draw(SerializedProperty parentProperty, int selectedIndex = -1) {
                this.selectedIndex = selectedIndex;
                ForEach(parentProperty, OnElement, selectedIndex, OnBeforePropertyBlock, OnAfterPropertyBlock);
            }
        }
        public class ListField<T> where T : class {

            public string labelName = "";
            public bool show = false;
            public bool showToggableButton = false;
            public List<T> items;
            public ReorderableList list = null;

            public bool IsActive {
                get {
                    return list.index != -1 ? true : false;
                }
            }

            public int index {
                get {
                    return list.index;
                }
            }

            public void DrawIndex(Rect rect, int index, int startIndex = 1) {
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 20, EditorGUIUtility.singleLineHeight), "" + (index + startIndex));
            }

            public void DrawBase(Rect rect, int index) {
                rect.y += 2;

                T item = items[index];
                item = EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width - 10, EditorGUIUtility.singleLineHeight), items[index] as UnityEngine.Object, typeof(T), true) as T;

                if (item != items[index]) {
                    items[index] = item;
                    OnChanged(list);
                }
            }

            public Action<Rect, int, bool, bool> OnDraw = (rect, index, isActive, isFocused) => { };

            public Action<ReorderableList> OnChanged = (l) => { };

            public Action<ReorderableList> OnAdd = (l) => {
            };

            public Action<ReorderableList> OnRemove = (l) => {
                ReorderableList.defaultBehaviours.DoRemoveButton(l);
            };

            public Func<ReorderableList, bool> OnCanRemove = (l) => {
                return l.count >= 1;
            };

            public ListField(List<T> list, string labelName = "", bool draggable = false, bool editable = true, bool showToggableButton = false, bool drawHeader = true) {

                this.items = list;
                this.labelName = labelName;
                this.showToggableButton = showToggableButton;

                this.list = new UnityEditorInternal.ReorderableList(items, typeof(T), draggable, drawHeader, editable, editable);

                this.list.drawHeaderCallback = (Rect rect) => {
                    if (!showToggableButton) {
                        EditorGUI.LabelField(rect, labelName);
                    }
                };

                // specify default OnDraw Handling
                OnDraw = (rect, index, isActive, isFocused) => {
                    DrawBase(rect, index);
                };

                this.list.onAddCallback = (ReorderableList l) => {
                    OnAdd(l);
                };

                this.list.onRemoveCallback = (ReorderableList l) => {
                    OnRemove(l);
                };

                this.list.onCanRemoveCallback = (ReorderableList l) => {
                    return OnCanRemove(l);
                };

                this.list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    OnDraw(rect, index, isActive, isFocused);
                };

                this.list.onChangedCallback = (ReorderableList l) => {
                    OnChanged(l);
                };

            }

            public void Draw() {
                if (showToggableButton) {
                    Color clr = UnityEngine.GUI.color;
                    if (show) {
                        UnityEngine.GUI.color = Color.yellow;
                    }
                    if (GUILayout.Button(labelName)) {
                        show = !show;
                    }
                    UnityEngine.GUI.color = clr;
                    if (!show) {
                        return;
                    }
                }
                list.DoLayoutList();
            }

        }

        // simple data container to store and display error messages at specific world coordinates
        public class FaultyComponent {
            public Vector3 position;
            public string errorMessage;

            public FaultyComponent(Vector3 position, string caller, string errorMessage) {
                this.position = position;
                this.errorMessage = "[" + caller + "]\r\n" + errorMessage;
            }

            public void Draw(GUIStyle style) {
                DrawCenteredText(errorMessage, position, style, fontSize: 8);
            }
        }

        public class FaultyComponentContainer : IModule {
            // list of all rendercomponents that are somehow faulty
            protected List<FaultyComponent> faultyComponents = new List<UI.FaultyComponent>();
            private static GUIStyle faultyStyle = null;

            public void Add(Vector3 position, string caller, string errorMessage) {
                faultyComponents.Add(new FaultyComponent(position, caller, errorMessage));
            }

            public void Cleanup() {
                faultyComponents.Clear();
            }

            public void Draw() {
                foreach (var faultyComponent in faultyComponents) {
                    faultyComponent.Draw(faultyStyle);
                }
            }

            public void SetupStyles() {
                if (faultyStyle == null) {
                    faultyStyle = new GUIStyle(EditorStyles.helpBox);
                    faultyStyle.alignment = TextAnchor.MiddleCenter;
                    faultyStyle.padding = new RectOffset();
                    faultyStyle.margin = new RectOffset();
                    //faultyStyle.border = new RectOffset();
                }
            }
        }
        /// <summary>
        /// A Toggable button that can be used like any other GUILayout.Button
        /// </summary>
        public class ToggableButton {
            public GUIStyle style = null;
            public GUIContent label = null;
            public Color activeColor = Color.yellow;
            public bool active = false;

            public ToggableButton(string label, Texture2D icon) {
                this.label = new GUIContent(label, icon);
            }
            public ToggableButton(string label) {
                this.label = new GUIContent(label);
            }
            public ToggableButton(GUIStyle style, GUIContent content) {
                this.label = content;
                this.style = style;
            }

            public bool Draw(params GUILayoutOption[] options) {
                return Draw(GUI.color, options);
            }

            public bool Draw(Color inactive, params GUILayoutOption[] options) {
                Color defaultColor = GUI.color;
                if (active) {
                    GUI.color = activeColor;
                } else {
                    GUI.color = inactive;
                }

                bool btnClicked = style == null ? GUILayout.Button(label, options) : GUILayout.Button(label, style, options);
                if (btnClicked) {
                    active = !active;
                    GUI.color = defaultColor;
                    return true;
                }
                GUI.color = defaultColor;
                return false;
            }
        }

        public static void DrawSceneGUIOverlay(string header, Action fieldAction, int posX = 10, int posY = 10, int width = 90, int height = 60) {
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(posX, posY, width, height));
            var rect = EditorGUILayout.BeginVertical();
            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle labelStyle = new GUIStyle(EditorStyles.boldLabel);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.padding = new RectOffset(0, 0, 4, 0);
            labelStyle.margin = new RectOffset(0, 0, 0, 0);
            GUILayout.Label(header, labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            fieldAction();
            GUILayout.Space(4);
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        public static void DrawCenteredText(string text, Vector3 position, Color? color = null, int fontSize = 10, float yOffset = 0) {
            DrawCenteredText(text, position, EditorStyles.label, color, fontSize, yOffset);
        }

        public static void DrawCenteredText(string text, Vector3 position, GUIStyle style, Color? color = null, int fontSize = 10, float yOffset = 0) {
            GUIContent textContent = new GUIContent(text);
            GUIStyle styleCopy = new GUIStyle(style);

            if (color != null)
                styleCopy.normal.textColor = (Color)color;
            if (fontSize > 0)
                styleCopy.fontSize = fontSize;

            Vector2 textSize = styleCopy.CalcSize(textContent);
            Vector3 screenPoint = Camera.current.WorldToScreenPoint(position);

            if (screenPoint.z > 0) {
                var worldPosition = Camera.current.ScreenToWorldPoint(new Vector3(screenPoint.x - textSize.x * 0.5f, screenPoint.y + textSize.y * 0.5f + yOffset, screenPoint.z));
                Handles.Label(worldPosition, textContent, styleCopy);
            }
        }

        public static GUIStyle AlignStyle(GUIStyle style, TextAnchor anchor = TextAnchor.MiddleCenter) {
            GUIStyle tmpStyle = new GUIStyle(style);
            tmpStyle.alignment = anchor;
            return tmpStyle;
        }

        public static void DrawInactiveLabelField(string label, GUIStyle style, params GUILayoutOption[] options) {
            UnityEngine.GUI.enabled = false;
            if (style == null) {
                EditorGUILayout.LabelField(label, options);
            } else {
                EditorGUILayout.LabelField(label, style, options);
            }
            UnityEngine.GUI.enabled = true;
        }
        public static void DrawInactiveLabelField(string label) {
            DrawInactiveLabelField(label, null);
        }

        /// <summary>
        /// Wrapper to draw a foldable Item list
        /// </summary>
        public class ToggableEditorPrefsManagedItemList {
            public EditorPrefsManagedFoldoutArea foldoutArea;
            public ToggableEditorPrefsManagedItemList(string prefKey, bool defaultState = false) {
                foldoutArea = new EditorPrefsManagedFoldoutArea(null, prefKey, defaultState);
            }

            public void Draw<T>(IEnumerable items, string header = null, Action<T, int> onItem = null, int maxColumns = 5) {
                foldoutArea.doInsideArea = () => {
                    DrawItemList<T>(items, onItem, maxColumns);
                };
                foldoutArea.Draw(header);
            }
        }

        /// <summary>
        /// Display a list of items of any type of IEnumerable including any type of array, list and dictionary
        /// </summary>
        /// <typeparam name="T"> The type of an item inside the IEnumerable (not the IEnumerable type itself!) </typeparam>
        /// <param name="header"> The content of the Label</param>
        /// <param name="items"> The IEnumerable object holding the items to display </param>
        /// <param name="onItem"> The action called on every item </param>
        /// <param name="maxColumns">The max amount of columns </param>
        public static void DrawItemList<T>(IEnumerable items, Action<T, int> onItem = null, int maxColumns = 5) {
            int i = 0;
            bool beganHorizontal = false;
            foreach (var item in items) {
                if (i == 0) {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    beganHorizontal = true;
                } else if (i % maxColumns == 0) {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    beganHorizontal = true;
                }
                if (onItem == null) {
                    onItem = (_, index) => EditorGUILayout.LabelField(((T)item).ToString());
                }
                onItem((T)item, i);
                i++;
            }
            if (beganHorizontal) {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
        }

        public class ExpandedScrollView {

            public enum ViewType { ExpandAll, ExpandWidth };
            public Vector2 scrollPos = Vector2.zero;
            public Action scrollContent = () => { };
            public Action element = () => { };
            public int height = 150;

            public ExpandedScrollView(Action scrollContent, ViewType viewType = ViewType.ExpandAll, GUIStyle style = null) {
                this.scrollContent = scrollContent;

                if (viewType == ViewType.ExpandAll) {
                    if (style != null) {
                        element = () => scrollPos = EditorGUILayout.BeginScrollView(scrollPos, style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    } else {
                        element = () => scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                    }
                } else {
                    if (style != null) {
                        element = () => scrollPos = EditorGUILayout.BeginScrollView(scrollPos, style, GUILayout.ExpandWidth(true), GUILayout.Height(height));
                    } else {
                        element = () => scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(height));
                    }
                }
            }

            public void Update() {
                element();
                scrollContent();
                EditorGUILayout.EndScrollView();
            }

        }

        /// <summary>
        /// Just a little helper to know which functions to implement when creating an EditorWindow
        /// </summary>
        public abstract class WindowBase : EditorWindow {
            protected virtual string WindowName {
                get {
                    return "Tools/" + GetType().ToString();
                }
            }

            protected GUIStyle boldLabelStyle {
                get {
                    return UI.boldLabelStyle;
                }
            }

            protected GUIStyle AlignStyle(GUIStyle style, TextAnchor anchor = TextAnchor.MiddleCenter) {
                return UI.AlignStyle(style, anchor);
            }

            protected virtual void DrawHeader() {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(titleContent, boldLabelStyle);
            }

            protected abstract void OnEnable();
            protected abstract void OnGUI();
        }

        public abstract class ScrollableWindowBase : WindowBase {
            private UI.ExpandedScrollView scrollView = null;

            protected override void OnEnable() {
                scrollView = new ExpandedScrollView(OnScrollView);
            }

            protected override void OnGUI() {
                scrollView.Update();
            }

            protected virtual void OnScrollView() {
                DrawHeader();
            }

        }

        public abstract class ScrollableWindowBaseWithMessageBox : ScrollableWindowBase {
            protected string errorMessage = null;
            private string lastErrorMessage = null;
            protected int screenTimeForErrorMessage = 3;
            private double time;

            protected virtual string DefaultMessage {
                get {
                    return "";
                }
            }

            protected override void OnGUI() {
                if (errorMessage != null) {
                    EditorGUILayout.HelpBox(errorMessage, MessageType.Warning);
                } else {
                    EditorGUILayout.HelpBox(DefaultMessage, MessageType.Info);
                }
                base.OnGUI();
            }

            private void Update() {

                if (errorMessage != null) {
                    if (lastErrorMessage != errorMessage) {
                        lastErrorMessage = errorMessage;
                        time = EditorApplication.timeSinceStartup + screenTimeForErrorMessage;
                    }

                    if (EditorApplication.timeSinceStartup > time) {
                        errorMessage = null;
                        lastErrorMessage = null;
                        Repaint();
                    }
                }

            }

        }

        /// <summary>
        /// Shorthand to get or create an Editor Window
        /// </summary>
        /// <typeparam name="T">The type of the editor window</typeparam>
        /// <param name="title">The title that should be displayed</param>
        /// <returns>Returns the window.</returns>
        public static T GetOrCreateWindowWithTitle<T>(string title) where T : EditorWindow {
            //Show existing window instance. If one doesn't exist, make one.
            T window = EditorWindow.GetWindow(typeof(T)) as T;
            window.titleContent = new GUIContent(title);
            window.Show();

            return window;
        }

        public static void GetOrCreateWindowWithTitle(System.Type type, string title) {
            // Show existing window instance. If one doesn't exist, make one.
            EditorWindow win = EditorWindow.GetWindow(type);
            win.titleContent = new GUIContent(title);
            win.Show();
        }
    }
}
#endif