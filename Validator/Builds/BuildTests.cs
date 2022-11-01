using System.Collections.Generic;
using Workshop.Builder;
using Xunit;

namespace Validator.Builds
{
    public class BuildTests
    {
        //[Fact]
        //public void Comparing_AssignmentsAreProperlyTreated()
        //{
        //    List<FileReference> TestLibrary = new List<FileReference>()
        //    {
        //        new FileReference()
        //        {
        //            SourceFilePath = @"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\6e84fb72-eb56-41a9-985e-829822df2bb1\Enbiroth\fighter\edge\model\body\c04\alp_edge_003_col.nutexb",
        //            OutputFilePath = @"fighter\edge\model\body\c04\alp_edge_003_col.nutexb",
        //            Status = FileStatus.Copy
        //        },
        //        new FileReference()
        //        {
        //            SourceFilePath = @"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\6e84fb72-eb56-41a9-985e-829822df2bb1\Enbiroth\fighter\edge\model\body\c04\alp_edge_003_col.numatb",
        //            OutputFilePath = @"fighter\edge\model\body\c04\alp_edge_003_col.numatb",
        //            Status = FileStatus.Copy
        //        },
        //    };
        //    List<FileReference> TestAssignments = new List<FileReference>()
        //    {
        //        new FileReference()
        //        {
        //            SourceFilePath = @"C:\Users\Mowjoh\Documents\Quasar\Library\Mods\6e84fb72-eb56-41a9-985e-829822df2bb1\Enbiroth\fighter\edge\model\body\c04\alp_edge_003_col.nutexb",
        //            OutputFilePath = @"fighter\edge\model\body\c06\alp_edge_003_col.nutexb",
        //            Status = FileStatus.Copy
        //        }
        //    };

        //    List<FileReference> Result = Builder.CompareAssignments(TestLibrary, TestAssignments);
        //    Assert.Contains(Result, r => r.Status == FileStatus.CopyEdited);
        //    Assert.Contains(Result, r => r.Status == FileStatus.Copy);
        //}
    }
}
