using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LIBII.CustomEditor
{
    public class InspectorGroup : InspectorSorter, IDisposable
    {
        public enum EStyle
        {
            Layout,
            Box,
        }

        public GroupAttribute attribute;

        public EStyle style = EStyle.Layout;

        public GroupAttribute.ELayout layout = GroupAttribute.ELayout.Vertical;

        public string name;

        public int depth;

        public InspectorObject inspectorObject;

        public InspectorGroup parent;

        public List<InspectorGroup> children = new List<InspectorGroup>();

        public List<InspectorSorter> sorters = new List<InspectorSorter>();

        public GroupHandler layoutHandler = new GroupHandler();

        public bool isFoldout = true;

        private List<InspectorSorter> _visibleSorters = new List<InspectorSorter>();

        public List<InspectorSorter> visibleSorters
        {
            get
            {
                _visibleSorters.Clear();
                foreach (var sorter in sorters)
                {
                    if (sorter is InspectorElement element && element.shouldShow) _visibleSorters.Add(sorter);
                    if (sorter is InspectorGroup subGroup && subGroup.shouldShow) _visibleSorters.Add(sorter);
                }
                return _visibleSorters;
            }
        }

        public bool shouldShow => visibleSorters.Count > 0;

        public void Dispose()
        {
            parent = null;
            foreach (var child in children)
            {
                child.Dispose();
            }

            children = null;
            sorters.Clear();
            sorters = null;
            layoutHandler = null;
        }

        ~InspectorGroup()
        {
            Dispose();
        }

        public static void JoinSortersWithCheckGroup(InspectorElement inspectorElement, ref List<InspectorGroup> groups,
            ref List<InspectorSorter> sorters)
        {
            GroupAttribute groupAttribute = null;
            if (inspectorElement.propertyInfo != null)
            {
                groupAttribute = inspectorElement.propertyInfo.GetCustomAttribute<GroupAttribute>();
            }
            else if (inspectorElement.fieldInfo != null)
            {
                groupAttribute = inspectorElement.fieldInfo.GetCustomAttribute<GroupAttribute>();
            }
            else if (inspectorElement.methodInfo != null)
            {
                groupAttribute = inspectorElement.methodInfo.GetCustomAttribute<GroupAttribute>();
            }
            if (groupAttribute == null)
            {
                sorters.Add(inspectorElement);
                return;
            }

            var groupLayout = groupAttribute.layout;
            var groupStyle = EStyle.Layout;
            if (groupAttribute is BoxAttribute)
            {
                groupStyle = EStyle.Box;
            }

            var inspectorObject = inspectorElement.inspectorObject;
            string[] pathSplits = groupAttribute.path.Split('/');
            InspectorGroup parent = null;
            for (int i = 0; i < pathSplits.Length; i++)
            {
                string groupName = pathSplits[i];
                var group = i == 0
                    ? groups.FirstOrDefault(x => x.name == groupName)
                    : parent.children.FirstOrDefault(x => x.name == groupName);
                if (group == default)
                {
                    group = new InspectorGroup()
                    {
                        name = groupName,
                        depth = i,
                        order = inspectorElement.order,
                        parent = parent,
                        inspectorObject = inspectorObject,
                    };
                    if (parent == null)
                    {
                        groups.Add(group);
                        sorters.Add(group);
                    }
                    else
                    {
                        parent.children.Add(group);
                        parent.sorters.Add(group);
                    }
                }

                parent = group;

                if (i == pathSplits.Length - 1)
                {
                    group.attribute = groupAttribute;
                    group.layout = groupLayout;
                    group.style = groupStyle;
                    if (group.style == EStyle.Box) group.isFoldout = ((BoxAttribute)groupAttribute).isFoldoutByDefault;
                    group.Add(inspectorElement);
                }
            }
        }

        private void Add(InspectorElement inspectorElement)
        {
            inspectorElement.inspectorGroup = this;
            order = Mathf.Min(order, inspectorElement.order);
            sorters.Add(inspectorElement);
        }

        public void Sort()
        {
            children = children.OrderBy(x => x.order).ToList();
        }

        public override string ToString()
        {
            string str = "{ name:" + name;
            str += "| depth:" + depth;
            str += "| order:" + order;
            str += "| sortersCount:" + sorters.Count;
            str += "| children:[";
            foreach (var child in children)
            {
                str += "| child:" + child;
            }

            str += "] }";
            return str;
        }
    }
}