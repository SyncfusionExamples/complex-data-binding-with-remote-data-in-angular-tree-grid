using Syncfusion.EJ2.Base;
using Syncfusion.EJ2.Gantt;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace SyncfusionAngularASPNETMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DataSource(DataManagerRequest dm)
        {
            List<TreeData> data = new List<TreeData>();
            data = TreeData.GetTree();
            DataOperations operation = new DataOperations();
            IEnumerable<TreeData> DataSource = data;
            
            if (!(dm.Where != null && dm.Where.Count > 1))
            {
                data = data.Where(p => p.ParentValue == null).ToList();
            }
            DataSource = data;
            if (dm.Search != null && dm.Search.Count > 0) // Searching
            {
                DataSource = operation.PerformSearching(DataSource, dm.Search);
            }
            if (dm.Sorted != null && dm.Sorted.Count > 0 && dm.Sorted[0].Name != null) // Sorting
            {
                DataSource = operation.PerformSorting(DataSource, dm.Sorted);
            }
            if (dm.Where != null && dm.Where.Count > 1) //filtering
            {
                DataSource = operation.PerformFiltering(DataSource, dm.Where, "and");
            }
           
           
            int count = data.Count;
            DataSource = data;
            if (dm.Skip != 0)
            {
                DataSource = operation.PerformSkip(DataSource, dm.Skip);   //Paging
            }
            if (dm.Take != 0)
            {
                DataSource = operation.PerformTake(DataSource, dm.Take);
            }
            return dm.RequiresCounts ? Json(new { result = DataSource, count = count }) : Json(DataSource);

        }

        private void AppendChildren(DataManagerRequest dm, List<TreeData> ChildRecords, TreeData ParentValue, Dictionary<string, List<TreeData>> GroupData, List<TreeData> data) // Getting child records for the respective parent
        {
            string TaskId = ParentValue.TaskID.ToString();
            var index = data.IndexOf(ParentValue);
            DataOperations operation = new DataOperations();
            foreach (var Child in ChildRecords)
            {
                //Based on the provided condition, the child records are retained and the value is passed from the server to the client.
                if (ParentValue.IsExpanded)
                {
                    string ParentId = Child.ParentValue.ToString();
                    if (TaskId == ParentId)
                    {
                        ((IList)data).Insert(++index, Child);
                        if (GroupData.ContainsKey(Child.TaskID.ToString()))
                        {
                            var DeepChildRecords = GroupData[Child.TaskID.ToString()];
                            if (DeepChildRecords?.Count > 0)
                            {
                                if (dm.Sorted != null && dm.Sorted.Count > 0 && dm.Sorted[0].Name != null) // sorting the child records
                                {
                                    IEnumerable ChildSort = DeepChildRecords;
                                    ChildSort = operation.PerformSorting(ChildSort, dm.Sorted);
                                    DeepChildRecords = new List<TreeData>();
                                    foreach (var rec in ChildSort)
                                    {
                                        DeepChildRecords.Add(rec as TreeData);
                                    }
                                }
                                if (dm.Search != null && dm.Search.Count > 0) // searching the child records
                                {
                                    IEnumerable ChildSearch = DeepChildRecords;
                                    ChildSearch = operation.PerformSearching(ChildSearch, dm.Search);
                                    DeepChildRecords = new List<TreeData>();
                                    foreach (var rec in ChildSearch)
                                    {
                                        DeepChildRecords.Add(rec as TreeData);
                                    }
                                }
                                AppendChildren(dm, DeepChildRecords, Child, GroupData, data);
                                if (Child.IsExpanded)
                                {
                                    index += DeepChildRecords.Count;
                                }
                            }
                        }
                    }
                }
            }

        }

        public ActionResult Update(CRUDModel<TreeData> value)
        {
            List<TreeData> data = new List<TreeData>();
            data = TreeData.GetTree();
            var val = data.Where(ds => ds.TaskID == value.Value.TaskID).FirstOrDefault();
            val.TaskName = value.Value.TaskName;
            val.Duration = value.Value.Duration;
            return Json(val);
        }

        public ActionResult Insert(CRUDModel<TreeData> value)
        {
            var c = 0;
            for (; c < TreeData.GetTree().Count; c++)
            {
                if (TreeData.GetTree()[c].TaskID == value.RelationalKey)
                {
                    if (TreeData.GetTree()[c].isParent == null)
                    {
                        TreeData.GetTree()[c].isParent = true;
                    }
                    break;
                }
            }
            c += FindChildRecords(value.RelationalKey);
            TreeData.GetTree().Insert(c + 1, value.Value);

            return Json(value.Value);
        }

        public int FindChildRecords(int? id)
        {
            var count = 0;
            for (var i = 0; i < TreeData.GetTree().Count; i++)
            {
                if (TreeData.GetTree()[i].ParentValue == id)
                {
                    count++;
                    count += FindChildRecords(TreeData.GetTree()[i].TaskID);
                }
            }
            return count;
        }

        public object Delete(CRUDModel<TreeData> value)
        {
            if (value.deleted != null)
            {
                for (var i = 0; i < value.deleted.Count; i++)
                {
                    TreeData.GetTree().Remove(TreeData.GetTree().Where(ds => ds.TaskID == value.deleted[i].TaskID).FirstOrDefault());
                }
            }
            else
            {
                TreeData.GetTree().Remove(TreeData.GetTree().Where(or => or.TaskID == int.Parse(value.Key.ToString())).FirstOrDefault());
            }
            return Json(value);
        }

        public class CRUDModel<T> where T : class
        {

            public TreeData Value;
            public int Key { get; set; }
            public int RelationalKey { get; set; }
            public List<T> added { get; set; }
            public List<T> changed { get; set; }
            public List<T> deleted { get; set; }
        }
        public class TaskDetails
        {
            public string Name { get; set; }
           
        }


        public class TreeData
        {
            public static List<TreeData> tree = new List<TreeData>();
            [System.ComponentModel.DataAnnotations.Key]
            public int TaskID { get; set; }
            public string TaskName { get; set; }
            public int Duration { get; set; }
            public int? ParentValue { get; set; }
            public bool? isParent { get; set; }
            public bool IsExpanded { get; set; }
            public TaskDetails Tasks { get; set; }
            public TreeData() { }
            public static List<TreeData> GetTree()
            {
                if (tree.Count == 0)
                {
                    int root = 0;
                    for (var t = 1; t <= 5; t++)
                    {
                        Random ran = new Random();
                        root++;
                        int rootItem = root;
                       
                        tree.Add(new TreeData() { TaskID = rootItem, TaskName = "Parent task " + rootItem.ToString(), isParent = true, IsExpanded = true, ParentValue = null, Duration = ran.Next(1, 50), Tasks = new TaskDetails { Name = "Parent" } });
                        int parent = root;
                          int subparent = root;
                            for (var c = 0; c < 3; c++)
                            {
                                root++;
                                int childID = root;
                                tree.Add(new TreeData() { TaskID = childID, TaskName = "sub Child task " + childID.ToString(), ParentValue = subparent, Duration = ran.Next(1, 50), Tasks = new TaskDetails { Name = "Sub child" } });
                            }
                        }
                    
                }
                return tree;
            }
        }
    }

}