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
                    for (var t = 1; t <= 25; t++)
                    {
                        Random ran = new Random();
                        root++;
                        int rootItem = root;
                       
                        tree.Add(new TreeData() { TaskID = rootItem, TaskName = "Parent task " + rootItem.ToString(), isParent = true, IsExpanded = true, ParentValue = null, Duration = ran.Next(1, 50), Tasks = new TaskDetails { Name = "Parent"+ rootItem.ToString() } });
                        int parent = root;
                          int subparent = root;
                            for (var c = 0; c < 3; c++)
                            {
                                root++;
                                int childID = root;
                                tree.Add(new TreeData() { TaskID = childID, TaskName = "sub Child task " + childID.ToString(), ParentValue = subparent, Duration = ran.Next(1, 50), Tasks = new TaskDetails { Name = "Sub child" + childID.ToString()} });
                            }
                        }
                    
                }
                return tree;
            }
        }
    }

}
