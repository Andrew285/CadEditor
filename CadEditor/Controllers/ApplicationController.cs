using CadEditor.MeshObjects;
using CadEditor.Models.Commands;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public class ApplicationController
    {
        public CubeController CubeController;
        public SceneCollection SceneCollection;
        public CommandsHistory CommandsHistory;
        public Library Library;
        public AttachingController AttachingController;
        public MovingController MovingController;
        public UIController UIController;
        public RenderController RenderController;
        public SceneController SceneController;
        public MouseController MouseController;
        private Scene _scene;

        public ApplicationController()
        {
            INameProvider nameProvider = new ModelNameProvider();
            SceneController = new SceneController();
            CubeController = new CubeController(this, nameProvider);
            AttachingController = new AttachingController();
            MovingController = new MovingController();
            UIController = new UIController(this);
            RenderController = new RenderController(this);
            MouseController = new MouseController();
            CommandsHistory = new CommandsHistory();
            Library = new Library();
            _scene = SceneController.Scene;
        }

        public void Initialize()
        {
            RenderController.Initialize();
            InitializeScene();
        }

        public void InitializeScene()
        {
            AddNewCubeElement(new Point3D(6, 0, 5));
            AddNewCubeElement(new Point3D(5, 5, 8));
        }

        public void SetSceneCollection(SceneCollection sceneCollection)
        {
            SceneCollection = sceneCollection;
        }

        public void HandleMouseDown(MouseButtons mouseButton)
        {
            ISceneObject selectedObject = SceneController.GetSelectedObject();
            ISceneObject prevObject;
            AxisCube axisCube;
            Point3D startMovePoint;

            prevObject = selectedObject;
            SceneController.SetPreviousSelectedObject(prevObject);
            selectedObject = _scene.Select(MouseController.X, MouseController.Y);
            SceneController.SetSelectedObject(selectedObject);
            if (mouseButton == MouseButtons.Left)
            {
                GraphicsGL.DisableContexMenu();

                CommandList selectionCommandList = new CommandList();

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
            else if (mouseButton == MouseButtons.Right)
            {
                GraphicsGL.DisableContexMenu();
                //scene.Select();

                if (selectedObject != null)
                {
                    SelectionCommand selectionCommand = new SelectionCommand(this, selectedObject);
                    selectionCommand.Execute();
                    UIController.InitContextMenu(MouseController.X, MouseController.Y);
                }
            }
            else if (mouseButton == MouseButtons.Middle)
            {
                MouseController.IsMiddleButtonPressed = true;
            }
        }

        public void HandleMouseMove(int x, int y)
        {
            RenderController.UpdateRotation(x, y);
            MoveBy(x, y);
            MouseController.UpdateMousePosition(x, y);
        }

        public void HandleMouseUp()
        {
            AxisCube axisCube = SceneController.GetAxisCube();
            ISceneObject prevObject = SceneController.GetPreviousSelectedObject();
            Point3D startMovingPoint = MovingController.GetStartMovingPoint();
            Point3D endMovingPoint;

            MouseController.IsMiddleButtonPressed = false;

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

        public void HandleProcessCmdKey()
        {

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
            SceneController.Scene.AddObject(cube);
            SceneCollection.AddCube(cube);
            return cube;
        }

        public void MoveBy(int x, int y)
        {
            AxisCube axisCube = SceneController.GetAxisCube();
            ISceneObject prevObject = SceneController.GetPreviousSelectedObject();

            if (axisCube != null)
            {
                double sensitivityLevel = 0.01;
                double value = MouseController.GetHorizontalAngle(x) * sensitivityLevel;
                double valueY = MouseController.GetVerticalAngle(y) * sensitivityLevel;
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
                DividingCubeForm form = InitializeDividingForm();
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
                    facet.IsAttached = true;
                    AttachingController.SetTargetAxisType(facet.AxisType);
                    break;
                }
            }

            CoordinateAxisType oppositeType = AxisSystem.GetOppositeAxisType(targetFacetAxis);
            foreach (Plane facet in attachingCube.Mesh.Facets)
            {
                if (facet.AxisType == oppositeType)
                {
                    facet.IsAttached = true;
                    AttachingController.SetAttachingAxisType(facet.AxisType);
                    break;
                }
            }

            Vector distanceVector = attachingCube.GetCenterPoint() - pointToMove;
            attachingCube.Move(distanceVector * (-1));
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

        public void SetCameraTarget()
        {
            RenderController.SetCameraTarget(_scene.SelectedObject);
        }

        public void UpdateRotation(IRotateable rotateable, int x, int y)
        {
            float xDelta = (float)MouseController.GetHorizontalAngle(x);
            float yDelta = (float)MouseController.GetVerticalAngle(y);

            rotateable.xRotation += xDelta * 1f;
            rotateable.yRotation += yDelta * 1f;

            GraphicsGL.Control.Invalidate();
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

        private DividingCubeForm InitializeDividingForm()
        {
            DividingCubeForm form = new DividingCubeForm();
            form.TopMost = true;
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            return form;
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
    }
}
