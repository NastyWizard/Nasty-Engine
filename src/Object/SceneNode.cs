using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NastyEngine
{
    public class SceneNode
    {

        public enum TagTypes { NONE = 0, PLAYER = 1, FLOOR = 2 }

        public string Name = "";
        public TagTypes Tags = TagTypes.NONE;
        public Guid ID = Guid.NewGuid();
        private bool isEnabled = true;
        public bool Enabled
        {
            get { return isEnabled; }
            set { if (isEnabled = value) OnEnable(); }
        }

        public bool HasInit { get; private set; } = false;

        private int depth;
        public int Depth
        {
            get { return depth; }

            set
            {
                if (Parent != null)
                    Parent.UpdateRenderOrder();
                depth = value;
            }
        }
        public SceneNode Parent;
        private Dictionary<Guid, SceneNode> children;
        private int numChildren { get { return children.Count; } }


        public SceneNode()
        {
            Enabled = true;
            children = new Dictionary<Guid, SceneNode>();
            Depth = 0;
        }

        public virtual void OnEnable()
        {
        }

        public virtual void Init()
        {
            HasInit = true;
            if (numChildren != 0)
            {
                foreach (var k in children.Keys)
                {
                    if (children[k].Enabled)
                        children[k].Init();
                }
            }
        }

        public virtual void Update()
        {
            if (numChildren != 0)
            {

                var childList = children.ToList();

                childList = childList.OrderBy(pair => pair.Value.depth).ToList();
                for (int i = 0; i < childList.Count; i++)
                {
                    if (childList[i].Value.Enabled)
                        childList[i].Value.Update();
                }
            }
        }

        public virtual void Render()
        {
            if (numChildren > 0)
            {
                var childList = children.ToList();

                childList = childList.OrderBy(pair => pair.Value.depth).ToList();
                for (int i = 0; i < childList.Count; i++)
                {
                    if (childList[i].Value.Enabled)
                        childList[i].Value.Render();
                }
            }
        }

        public void AddChild(SceneNode child)
        {
            if (!children.ContainsValue(child))
            {
                child.Parent = this;
                children.Add(child.ID, child);
            }
            else
                Debug.WriteLine("ERROR: Adding duplicate child of name '" + child.Name + "' to SceneNode '" + Name + "'");

        }

        public void RemoveChild(Guid ID)
        {
            if (children.ContainsKey(ID))
                children.Remove(ID);
            else
                Debug.WriteLine("ERROR: Tried removing non existant child ID '" + ID + "' from SceneNode '" + Name + "' ");
        }

        public void RemoveChild(SceneNode obj)
        {
            RemoveChild(obj.ID);
        }

        /// <summary>
        /// Find a child of this node
        /// </summary>
        public SceneNode GetChild(string name)
        {

            var childList = children.ToList();

            childList = childList.OrderBy(pair => pair.Value.depth).ToList();
            for (int i = 0; i < childList.Count; i++)
            {
                if (childList[i].Value.Name == name)
                    return childList[i].Value;
            }
            return null;
        }

        public GameObject GetChildGO(string name)
        {

            var childList = children.ToList();

            childList = childList.OrderBy(pair => pair.Value.depth).ToList();
            for (int i = 0; i < childList.Count; i++)
            {
                if (childList[i].Value.Name == name)
                    return (GameObject)childList[i].Value;
            }
            return null;
        }

        public T GetChild<T>(string name) where T : SceneNode
        {

            var childList = children.ToList();

            childList = childList.OrderBy(pair => pair.Value.depth).ToList();
            for (int i = 0; i < childList.Count; i++)
            {
                if (childList[i].Value.Name == name)
                    return (T)childList[i].Value;
            }
            return null;
        }

        public List<KeyValuePair<Guid, SceneNode>> GetAllChildren() { return children.ToList(); }

        public void UpdateRenderOrder()
        {

        }

    }
}
