using CadEditor.MeshObjects;
using CadEditor.Models.Commands;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using CadEditor.Settings;
using CadEditor.Tools.Localization;
using CadEditor.View.Forms;
using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public class ApplicationController
    {
        public CubeController CubeController { get; private set; }
        public SceneCollection SceneCollection { get; private set; }
        public CommandsHistory CommandsHistory { get; private set; }
        public AttachingController AttachingController { get; private set; }
        public MovingController MovingController { get; private set; }
        public UIController UIController { get; private set; }
        public RenderController RenderController { get; private set; }
        public SceneController SceneController { get; private set; }
        public SettingsController SettingsController { get; private set; }
        public ProjectController ProjectController { get; private set; }
        public Localization Localization { get; private set; }
        public Library Library { get; private set; }
        private Scene _scene;

        public ApplicationController(Form1 mainForm, OpenGLControl glControl)
        {
            INameProvider nameProvider = new ModelNameProvider();
            RenderController = new RenderController(this, glControl);
            SceneController = new SceneController();
            CubeController = new CubeController(this, nameProvider);
            AttachingController = new AttachingController();
            MovingController = new MovingController();
            UIController = new UIController(this, mainForm);
            CommandsHistory = new CommandsHistory();
            SettingsController = new SettingsController(this);
            ProjectController = new ProjectController();
            Localization = new Localization();
            Library = new Library();
            _scene = SceneController.Scene;
        }

        public void Initialize()
        {
            RenderController.Initialize();
            SettingsController.LoadData(MainSettings.FilePath);
            UIController.Initialize();
            Localization.LoadTranslations();
            InitializeScene();
        }

        public void InitializeScene()
        {
            AddNewCubeElement(new Point3D(6, 0, 5));
            AddNewCubeElement(new Point3D(5, 5, 8));
        }

        public void OpenSettingsForm()
        {
            ProjectSettingsForm form = new ProjectSettingsForm(this);
            Form1 mainForm = UIController.MainForm;
            form.LanguageSettingsControl.LanguageChanged += mainForm.SettingsForm_LanguageChanged;
            form.Show();
            //SubscribeToEvents();
        }

        public void SetSceneCollection(SceneCollection sceneCollection)
        {
            SceneCollection = sceneCollection;
        }

        public void HandleLeftMouseButtonDown()
        {
            GraphicsGL.DisableContexMenu();

            CommandList selectionCommandList = new CommandList();
            ISceneObject selectedObject = SceneController.GetSelectedObject();
            ISceneObject prevObject = SceneController.GetPreviousSelectedObject();
            AxisCube axisCube;
            Point3D startMovePoint;

            if (selectedObject != null)
            {
                if (selectedObject is AxisCube)
                {
                    SelectionCommand selectionCommand = new SelectionCommand(this, selectedObject);
                    DeselectionCommand deselectionCommand = new DeselectionCommand(this, prevObject);
                    DeleteAxesCommand deleteAxesCommand = new DeleteAxesCommand(this, prevObject);
                    selectionCommand.Execute();
                    deselectionCommand.Execute();
                    CommandsHistory.Push(new CommandList(new List<ICommand> { deselectionCommand, deleteAxesCommand }));

                    axisCube = selectedObject as AxisCube;
                    SceneController.SetAxisCube(axisCube);
                    startMovePoint = (Point3D)prevObject.GetCenterPoint().Clone();
                    MovingController.SetStartMovingPoint(startMovePoint);
                }
                else
                {
                    SceneController.DeleteSelectingCoordAxes();

                    if (prevObject != null && selectedObject != prevObject)
                    {
                        CommandList commandList = new CommandList(
                            new List<ICommand> {
                                new DeselectionCommand(this, prevObject),
                                new DeleteAxesCommand(this, prevObject)
                            });
                        commandList.Execute();
                        CommandsHistory.Push(commandList);
                    }

                    selectionCommandList.AddRange(new List<ICommand>
                        {
                            new SelectionCommand(this, selectedObject),
                            new InitAxesCommand(this, selectedObject)
                        });
                }
            }
            else
            {
                if (prevObject != null)
                {
                    selectionCommandList.AddRange(new List<ICommand>
                        {
                            new DeselectionCommand(this, prevObject),
                            new DeleteAxesCommand(this, prevObject)
                        });
                }
                else
                {
                    _scene.DeselectAll();
                }
            }

            if (!selectionCommandList.IsEmpty())
            {
                ISceneObject obj = ((UnaryCommand)selectionCommandList[0]).GetSceneObject();
                selectionCommandList.Execute();

                if (obj is ComplexCube || obj is ComplexStructure)
                {
                    CommandsHistory.Push(selectionCommandList);
                }
            }
        }

        public bool HandleRightMouseButtonDown()
        {
            GraphicsGL.DisableContexMenu();
            ISceneObject selectedObject = SceneController.GetSelectedObject();

            if (selectedObject != null)
            {
                SelectionCommand selectionCommand = new SelectionCommand(this, selectedObject);
                selectionCommand.Execute();
                return true;
            }

            return false;
        }

        public void InitializeContextMenu(int x, int y)
        {
            UIController.InitContextMenu(x, y);
        }

        public void HandleMouseDown(int mouseX, int mouseY)
        {
            ISceneObject selectedObject = SceneController.GetSelectedObject();
            ISceneObject prevObject;

            prevObject = selectedObject;
            SceneController.SetPreviousSelectedObject(prevObject);
            selectedObject = _scene.Select(mouseX, mouseY);
            SceneController.SetSelectedObject(selectedObject);
        }

        public void HandleMouseMove(double horizontalAngle, double verticalAngle, double rollAngle, bool isMiddleBtnPressed)
        {
            RenderController.UpdateRotation(horizontalAngle, verticalAngle, rollAngle, isMiddleBtnPressed);
            MoveBy(horizontalAngle, verticalAngle);
        }

        public void HandleMouseUp()
        {
            AxisCube axisCube = SceneController.GetAxisCube();
            ISceneObject prevObject = SceneController.GetPreviousSelectedObject();
            Point3D startMovingPoint = MovingController.GetStartMovingPoint();
            Point3D endMovingPoint;

            if (axisCube != null)
            {
                DeselectionCommand deselectionCommand = new DeselectionCommand(this, axisCube);
                deselectionCommand.Execute();
                axisCube = null;
                SceneController.SetAxisCube(axisCube);

                endMovingPoint = (Point3D)prevObject.GetCenterPoint().Clone();
                MovingController.SetEndMovingPoint(endMovingPoint);
                Vector moveResult = endMovingPoint - startMovingPoint;
                MoveCommand moveCommand = new MoveCommand(this, prevObject, moveResult);
                CommandsHistory.Push(moveCommand);
            }
        }

        public void HandleMouseWheel(int value)
        {
            RenderController.Camera.ZoomBy(value);
        }

        public int HandlePressTab(int index)
        {
            if (index == 1)
            {
                SceneController.Scene.SceneMode = SceneMode.EDIT;
                return 0;
            }
            else if (index == 0)
            {
                SceneController.Scene.SceneMode = SceneMode.VIEW;
                return 1;
            }
            SceneController.UpdateScene();
            return 0;
        }

        public void HandlePressCtrlZ()
        {
            if (!CommandsHistory.IsEmpty())
            {
                CommandsHistory.Peek()?.Undo();
                CommandsHistory.StepBackward();
            }
        }

        public void HandlePressCtrlShiftZ()
        {
            if (!CommandsHistory.IsEmpty())
            {
                CommandsHistory.StepForward();
                CommandsHistory.Peek()?.Redo();
            }
        }

        public int SetObjectToAxis(CoordinateAxis axis, int clicks)
        {
            if (AttachingController.AttachingAxisSystem != null)
            {
                return ClickKeyAxes(axis, clicks);
            }
            return 0;
        }

        public ComplexCube AddNewCubeElement(Point3D position, Vector size = null, string name = null)
        {
            ComplexCube cube = CubeController.Create(position, size, name);
            RegisterObject(cube);
            return cube;
        }

        public void RegisterObject(ISceneObject obj)
        {
            SceneController.Scene.AddObject(obj);
            if (obj is ComplexCube)
            {
                SceneCollection.AddCube(obj as ComplexCube);
            }
            else if (obj is ComplexStructure)
            {
                SceneCollection.AddComplexStructure(obj as ComplexStructure);
            }
        }

        public void MoveBy(double horizontalAngle, double verticalAngle)
        {
            AxisCube axisCube = SceneController.GetAxisCube();
            ISceneObject prevObject = SceneController.GetPreviousSelectedObject();

            if (axisCube != null)
            {
                double sensitivityLevel = 0.01;
                double value = horizontalAngle * sensitivityLevel;
                double valueY = verticalAngle * sensitivityLevel;
                Vector coords = new Vector(3);

                if (axisCube.Axis == CoordinateAxis.X)
                {
                    MovingController.ActiveMovingAxis = CoordinateAxis.X;
                    coords = new Vector(value, 0, 0);
                }
                else if (axisCube.Axis == CoordinateAxis.Y)
                {
                    MovingController.ActiveMovingAxis = CoordinateAxis.Y;
                    coords = new Vector(0, -valueY, 0);
                }
                else if (axisCube.Axis == CoordinateAxis.Z)
                {
                    MovingController.ActiveMovingAxis = CoordinateAxis.Z;
                    coords = new Vector(0, 0, value);
                }

                MovingController.MovingVector = coords;

                prevObject.Move(coords);
                SceneController.MoveCoordinateAxes(coords);
            }
        }

        public void DivideElement()
        {
            if (_scene.SelectedObject != null && _scene.SelectedObject is IDivideable)
            {
                DividingCubeForm form = UIController.CreateDividingForm();
                DialogResult result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Vector nValues = form.nValues;
                    DivisionCommand divCommand = new DivisionCommand(this, _scene.SelectedObject as IDivideable, nValues);
                    divCommand.Execute();
                    CommandsHistory.Push(divCommand);
                }
            }
            else
            {
                Output.ShowMessageBox("Warning", "There is no selected cube");
            }
        }

        public void UniteElement()
        {
            (_scene.SelectedObject as IDivideable).Unite();
        }

        public void DeleteElement()
        {
            SceneController.DeleteCompletely(_scene.SelectedObject);
            SceneCollection.RemoveCube((IUniqueable)_scene.SelectedObject);
        }

        public void AttachElements()
        {
            Scene scene = _scene;
            ComplexCube targetCube = AttachingController.GetTargetCube();
            ComplexCube attachingCube = AttachingController.GetAttachingCube();
            CoordinateAxisType targetFacetAxis = AttachingController.GetTargetAxisType();
            CoordinateAxisType attachingFacetAxis = AttachingController.GetAttachingAxisType();
            AxisSystem attachingAxisSystem = AttachingController.AttachingAxisSystem;
            
            AttachingCommand attachingCommand = new AttachingCommand(this, targetCube, attachingCube, targetFacetAxis, attachingFacetAxis);
            attachingCommand.Execute();
            CommandsHistory.Push(attachingCommand);
            ComplexStructure complexStructure = attachingCommand.GetComplexStructure();
            scene.Remove(attachingAxisSystem);

            if (complexStructure != null && !scene.Contains(complexStructure))
            {
                scene.Remove(targetCube);
                scene.Remove(attachingCube);
                MakeNonAttachableElement();
                MakeNonTargetableElement();
                scene.Add(complexStructure);
                SceneCollection.AddComplexStructure(complexStructure);
            }
            else
            {
                SceneCollection.RemoveCube(attachingCube);
                SceneCollection.AddCube(attachingCube, complexStructure);
                ComplexCube attachingCubeCopy = attachingCube;
                MakeNonAttachableElement();
                MakeNonTargetableElement();
                scene.Remove(targetCube);
                scene.Remove(attachingCubeCopy);
            }
            AttachingController.AttachingAxisSystem = null;
        }

        public void CopySelectedElement()
        {
            ProjectController.Clipboard = SceneController.GetSelectedObject().Clone();
        }

        public void PasteElement()
        {
            ISceneObject pastedObject = ProjectController.Clipboard as ISceneObject;
            if (pastedObject != null)
            {

                pastedObject.Move(new Vector(2, 2, 2));
                if (pastedObject is ComplexCube)
                {
                    ComplexCube cube = CubeController.Create(pastedObject as ComplexCube);
                    RegisterObject(cube);
                }
                else if (pastedObject is ComplexStructure) 
                {
                    // TODO: implement copying of ComplexStructure
                }
            }
        }

        private int ClickKeyAxes(CoordinateAxis axis, int clicks)
        {
            List<Axis> axes = AttachingController.AttachingAxisSystem.GetAxes(axis);
            if (axes.Count > 0)
            {
                SetAttachingObjectToAxis(axes[clicks % axes.Count]);
            }
            clicks = clicks == 1 ? 0 : 1;
            return clicks;
        }

        public void SetAttachingObjectToAxis(Axis targetAxis)
        {
            ComplexCube targetCube = AttachingController.GetTargetCube();
            ComplexCube attachingCube = AttachingController.GetAttachingCube();
            CoordinateAxisType targetFacetAxis = AttachingController.GetTargetAxisType();
            Point3D pointToMove = targetAxis.P2;

            //Create AttachingFacetsPair
            foreach (Plane facet in targetCube.Mesh.Facets)
            {
                if (facet.GetCenterPoint() == targetAxis.P1)
                {
                    //facet.IsAttached = true;
                    AttachingController.SetTargetAxisType(facet.AxisType);
                    break;
                }
            }

            CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(targetFacetAxis);
            foreach (Plane facet in attachingCube.Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    //facet.IsAttached = true;
                    AttachingController.SetAttachingAxisType(facet.AxisType);
                    break;
                }
            }

            Vector distanceVector = attachingCube.GetCenterPoint() - pointToMove;
            ComplexStructure structure = SceneController.Scene.GetComplexStructureByCube(attachingCube);
            if (structure != null)
            {
                structure.Move(distanceVector * (-1));
            }
            else
            {
                attachingCube.Move(distanceVector * (-1));
            }
        }

        public void MakeAttachableElement()
        {
            AttachingController.SetAttachingCube(_scene.SelectedObject);
            ColorPartOf((MeshObject3D)_scene.SelectedObject, ModelPartTypes.EDGE, Color.Green);
        }

        public void MakeNonAttachableElement()
        {
            ComplexCube attachingCube = AttachingController.GetAttachingCube();
            ColorPartOf(attachingCube, ModelPartTypes.EDGE, Color.Red, ModelColorMode.SELECTED);
            ColorPartOf(attachingCube, ModelPartTypes.EDGE, Color.Black, ModelColorMode.NON_SELECTED);
            AttachingController.ClearAttachingCube();
        }

        public void MakeTargetableElement()
        {
            AttachingController.SetTargetCube(_scene.SelectedObject);
            ColorPartOf((MeshObject3D)_scene.SelectedObject, ModelPartTypes.EDGE, Color.Blue);

            // TODO: Finish Targetable method
            AttachingController.InitializeAttachingAxes((MeshObject3D)_scene.SelectedObject);
            SceneController.Scene.AddObject(AttachingController.AttachingAxisSystem);
        }

        public void MakeNonTargetableElement()
        {
            ComplexCube targetCube = AttachingController.GetTargetCube();
            ColorPartOf(targetCube, ModelPartTypes.EDGE, Color.Red, ModelColorMode.SELECTED);
            ColorPartOf(targetCube, ModelPartTypes.EDGE, Color.Black, ModelColorMode.NON_SELECTED);
            AttachingController.ClearTargetCube();
            _scene.ObjectCollection.Remove(AttachingController.AttachingAxisSystem);
        }

        public void RotateSelectedObject()
        {
            ISceneObject selectedObject = SceneController.GetSelectedObject();
            if (selectedObject is IRotateable)
            {

                RenderController.UpdateRotation(selectedObject as IRotateable, 90, 0);
            }
        }

        public void ColorPartOf(MeshObject3D meshObject, ModelPartTypes partType, Color color, ModelColorMode? mode = null)
        {
            if (partType == ModelPartTypes.EDGE)
            {
                if (mode == null)
                {
                    meshObject.EdgeSelectedColor = color;
                    meshObject.EdgeNonSelectedColor = color;
                }
                else if (mode == ModelColorMode.SELECTED)
                {
                    meshObject.EdgeSelectedColor = color;
                }
                else
                {
                    meshObject.EdgeNonSelectedColor = color;
                }
            }
        }

        public void SelectElement()
        {
            SelectionCommand selectionCommand = new SelectionCommand(this, _scene.SelectedObject);
            selectionCommand.Execute();
            CommandsHistory.Push(selectionCommand);
        }

        public void DeselectElement()
        {
            DeselectionCommand deselectionCommand = new DeselectionCommand(this, _scene.SelectedObject);
            deselectionCommand.Execute();
            CommandsHistory.Push(deselectionCommand);
        }

        public void OpenLibrary()
        {
            ProjectLibraryForm libraryForm = new ProjectLibraryForm(Library.GetAllSaves());
            DialogResult result = libraryForm.ShowDialog();

            if (result == DialogResult.OK)
            {
                SaveData saveData = libraryForm.GetSelectedProject();
                string[] lines = File.ReadAllLines(saveData.GetFilePath());
                SceneController.Scene.Import(lines);
                SceneCollection.Import(SceneController.Scene.ObjectCollection);
            }
        }

        public void SaveProject()
        {
            Bitmap bmp = RenderController.CaptureScreen();
            SaveForm form = new SaveForm(bmp);
            form.StartPosition = FormStartPosition.CenterScreen;
            DialogResult result = form.ShowDialog();

            if (result == DialogResult.OK)
            {
                string date = form.SaveData.GetDate().ToString();
                string convertedDate = date.Replace('.', 'd').Replace(":", "td").Replace(" ", "sp");
                string nameOfSave = form.SaveData.GetTitle() + " " + convertedDate;
                string exportString = SceneController.Scene.Export();

                bmp.Save(@"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Screenshots\" + nameOfSave + ".jpeg", ImageFormat.Jpeg);
                string filePath = @"D:\Projects\VisualStudio\CadEditor\CadEditor\LibrarySaves\Scene\" + nameOfSave + ".txt";

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine(exportString);
                    writer.Close(); // Close the writer to flush and release resources
                }
                Library.AddSave(bmp, filePath, nameOfSave, DateTime.Now);
            }
        }

        public void OpenDocumentation()
        {
            DocumentationForm form = new DocumentationForm();
            form.Show();
        }

        public void OpenGitHubRepo()
        {
            string githubUrl = "https://github.com/Andrew285/CadEditor";
            System.Diagnostics.Process.Start(githubUrl);
        }
    }
}
