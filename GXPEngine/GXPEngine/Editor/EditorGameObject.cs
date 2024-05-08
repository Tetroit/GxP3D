using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using GXPEngine.Core;

namespace GXPEngine.Editor.Exclusives
{
    public class EditorGameObject : DSCFSprite
    {
        ConstructorInfo Constructor;
        public Type ObjectType;
        public ParameterInfo[] ConstructorParams;
        public object[] ConstructorParameters;
        GameObject _EditorDisplayObject;
        public GameObject EditorDisplayObject {  get { return _EditorDisplayObject; } }
        public FieldInfo[] fields;
        public PropertyInfo[] properties;
        protected static Vector3[] _boxbounds;
        float radius = .25f;
        public EditorGameObject(Type objectType, ConstructorInfo constructor) : base("editor/ProxyLogo.png", addCollider:true)
        {
            width = 1;
            height = 1;
            SetOrigin(width * .5f,height * .5f);
            ObjectType = objectType;
            Constructor = constructor;
            ConstructorParams = Constructor.GetParameters();
            ConstructorParameters = new object[ConstructorParams.Length];
            properties = TypeHandler.GetPublicProperties(ObjectType);
            fields = TypeHandler.GetPublicVariables(ObjectType);
            BuildObject();
            if (_boxbounds == null) CreateBounds();
        }
        public EditorGameObject(GameObject builtObject, ConstructorInfo constructor, object[] builtParams) : base("editor/ProxyLogo.png", addCollider:true)
        {
            width = 1;
            height = 1;
            SetOrigin(width * .5f, height * .5f);
            ObjectType = builtObject.GetType();
            Constructor = constructor;
            ConstructorParams = Constructor.GetParameters();
            ConstructorParameters = builtParams;
            properties = TypeHandler.GetPublicProperties(ObjectType);
            fields = TypeHandler.GetPublicVariables(ObjectType);
            _EditorDisplayObject = builtObject;
            AddChild(EditorDisplayObject);
            if (_boxbounds == null) CreateBounds();
        }

        public EditorGameObject GetDuplicateWithChildren()
        {
            EditorGameObject res = GetDuplicate();
            foreach (GameObject go in GetChildren())
                if (go is EditorGameObject)
                    res.AddChild(((EditorGameObject)go).GetDuplicateWithChildren());
            return res;
        }
        public EditorGameObject GetDuplicate()
        {
            EditorGameObject res = new EditorGameObject(ObjectType, Constructor);
            for (int i = 0; i < ConstructorParameters.Length; i++)
                res.ConstructorParameters[i] = ConstructorParameters[i];
            res.BuildObject();
            res.position = position;
            res.rotation = rotation;
            res.scaleXYZ = scaleXYZ;
            if (EditorDisplayObject != null)
            {
                foreach (PropertyInfo property in properties)
                    property.SetValue(res.EditorDisplayObject, property.GetValue(EditorDisplayObject));
                foreach (FieldInfo field in fields)
                    field.SetValue(res.EditorDisplayObject, field.GetValue(EditorDisplayObject));
            }
            return res;
        }

        public object[] getPropertyValues()
        {
            object[] res = new object[properties.Length];
            if (_EditorDisplayObject == null) return null;

            for(int i = 0; i<properties.Length; i++)
            {
                res[i] = properties[i].GetValue(_EditorDisplayObject);
            }
            return res;
        }

        public object[] getFieldValues()
        {
            object[] res = new object[fields.Length]; 
            if( _EditorDisplayObject == null) return null;

            for (int i = 0; i < fields.Length; i++)
            {
                res[i] = fields[i].GetValue(_EditorDisplayObject);
            }
            return res;
        }

        public void BuildObject()
        {
            object[] propertyValues = getPropertyValues();
            object[] fieldValues = getFieldValues();
            bool exists = _EditorDisplayObject != null;

            _EditorDisplayObject?.Destroy();
            _EditorDisplayObject = TypeHandler.BuildFromConstructor(ConstructorParameters, ConstructorParams, ObjectType);
            if(exists)
            for(int i = 0; i<properties.Length; i++) 
                properties[i].SetValue(_EditorDisplayObject, propertyValues[i]);
            if(exists)
            for(int i = 0; i<fields.Length; i++) 
                fields[i].SetValue(_EditorDisplayObject, fieldValues[i]);

            if(_EditorDisplayObject != null) AddChild(_EditorDisplayObject);
        }
        public override void RenderDepthSorted(GLContext glContext, Vector3 slop)
        {
            if (this == ((SceneEditor)game).selectedGameobject)
                _texture = Texture2D.GetInstance("editor/SelectedMarker.png");
            else if(this == (game as SceneEditor).hoveredObject) _texture = Texture2D.GetInstance("editor/HoveredMarker.png");
            else _texture = Texture2D.GetInstance("editor/ProxyLogo.png");
            
            Vector3 baseScale = scaleXYZ;
            //scaleXYZ = radius*baseScale*(128.0f/game.width);
            base.RenderDepthSorted(glContext, slop);
            //scaleXYZ = baseScale;
        }
        protected override Collider createCollider()
        {
            return new BoxCollider(this);
        }
        public override Vector3[] GetExtents()
        {
            Vector3[] res = new Vector3[8];
            for (int i = 0; i < 8; i++)
                //dw abt it
                res[i] = TransformPoint(_boxbounds[i]*radius);
            return res;
        }
        void CreateBounds()
        {
            //cant just steal these from Box.cs. cant be fucked not to copypaste it either
            _boxbounds = new Vector3[8];
            for (int i = 0; i < 6; i++)
            {
                int sel = i % 3;
                int dir = i > 2 ? 1 : -1;
                Vector3 normal = new Vector3(sel == 1 ? dir : 0, sel == 0 ? dir : 0, sel == 2 ? dir : 0);
                Vector3 x = new Vector3(sel == 2 ? 1 : 0, 0, sel != 2 ? 1 : 0);
                Vector3 y = new Vector3(sel == 0 ? 1 : 0, sel != 0 ? 1 : 0, 0);
                Vector3 res;
                res = normal - x - y;
                if (sel == 1) _boxbounds[(dir == 1 ? 1 : 0) * 4 + 0] = res;
                res = normal + x - y;
                if (sel == 1) _boxbounds[(dir == 1 ? 1 : 0) * 4 + 1] = res;
                res = normal + x + y;
                if (sel == 1) _boxbounds[(dir == 1 ? 1 : 0) * 4 + 2] = res;
                res = normal - x + y;
                if (sel == 1) _boxbounds[(dir == 1 ? 1 : 0) * 4 + 3] = res;
            }
        }
    }
}
