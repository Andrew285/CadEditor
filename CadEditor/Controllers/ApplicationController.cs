
using CadEditor.MeshObjects;
using CadEditor.Models.Commands;
using CadEditor.Models.Scene;
using CadEditor.Models.Scene.MeshObjects;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CadEditor.Controllers
{
    public class ApplicationController
    {
        public CubeController CubeController;
        public SceneCollection SceneCollection;
        public CommandsHistory CommandsHistory;
        public AttachingController AttachingController;
        public UIController UIController;
        public RenderController RenderController;
        public Scene Scene;

        public ApplicationController()
        {
            INameProvider nameProvider = new ModelNameProvider();
            CubeController = new CubeController(nameProvider);
            AttachingController = new AttachingController();
            UIController = new UIController(this);
            RenderController = new RenderController();
        }

        public void SetScene(Scene scene)
        {
            Scene = scene;
        }

        public void SetSceneCollection(SceneCollection sceneCollection)
        {
            SceneCollection = sceneCollection;
        }

        public void SetCommandHistory(CommandsHistory commandsHistory)
        {
            CommandsHistory = commandsHistory;
        }

        public void SetCamera(Camera camera)
        {
            RenderController.SetCamera(camera);
        }

        public ComplexCube AddNewCubeElement()
        {
            ComplexCube cube = CubeController.Create(new Point3D(0, 0, 0), new Vector(1, 1, 1));
            Scene.AddObject(cube);
            SceneCollection.AddCube(cube);
            return cube;
        }

        public void DivideElement()
        {
            if (Scene.SelectedObject != null && Scene.SelectedObject is IDivideable)
            {
                DividingCubeForm form = InitializeDividingForm();
                DialogResult result = form.ShowDialog();

                if (result == DialogResult.OK)
                {
                    Vector nValues = form.nValues;
                    DivisionCommand divCommand = new DivisionCommand(Scene, Scene.SelectedObject as IDivideable, nValues);
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
            (Scene.SelectedObject as IDivideable).Unite();
        }

        public void DeleteElement()
        {
            //TODO: DELETE element
            Scene.DeleteCompletely(Scene.SelectedObject);
            SceneCollection.RemoveCube((IUniqueable)Scene.SelectedObject);
        }

        public void MakeAttachableElement()
        {
            AttachingController.SetAttachingCube(Scene.SelectedObject);
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Green);
        }

        public void MakeNonAttachableElement()
        {
            AttachingController.ClearAttachingCube();
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Red, ModelColorMode.SELECTED);
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Black, ModelColorMode.NON_SELECTED);
        }

        public void MakeTargetableElement()
        {
            AttachingController.SetTargetCube(Scene.SelectedObject);
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Blue);

            // TODO: Finish Targetable method
            //InitializeAttachingAxes((MeshObject3D)scene.SelectedObject);
        }

        public void MakeNonTargetableElement()
        {
            AttachingController.ClearTargetCube();
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Red, ModelColorMode.SELECTED);
            ColorPartOf((MeshObject3D)Scene.SelectedObject, ModelPartTypes.EDGE, Color.Black, ModelColorMode.NON_SELECTED);
        }

        public void SetCameraTarget()
        {
            RenderController.SetCameraTarget(Scene.SelectedObject);
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
            SelectionCommand selectionCommand = new SelectionCommand(Scene, Scene.SelectedObject);
            selectionCommand.Execute();
            CommandsHistory.Push(selectionCommand);
        }

        public void DeselectElement()
        {
            DeselectionCommand deselectionCommand = new DeselectionCommand(Scene, Scene.SelectedObject);
            deselectionCommand.Execute();
            CommandsHistory.Push(deselectionCommand);
        }
    }
}
