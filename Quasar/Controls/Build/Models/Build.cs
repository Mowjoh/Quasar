using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quasar.Controls.Build.Models
{

    enum BuildModes { Comparative, WipeRecreate }

    public abstract class Builder
    {
        public abstract void CopyModLoader();
        public abstract void StartCheck();
        public abstract bool StartBuild();
        public abstract bool StartTransfer();
        public abstract void CompareDeleteDifferences();
        public abstract void DeleteAll();
        
    }

    public class SmashBuilder : Builder
    {
        FileWriter Writer { get; set; }
        int BuildMode { get; set; }

        public SmashBuilder(FileWriter _Writer, int _BuildMode)
        {
            Writer = _Writer;
            BuildMode = _BuildMode;
            if (Writer.VerifyOK())
            {
                CopyModLoader();
                StartCheck();
            }
        }
        public override void CopyModLoader()
        {
            
        }
        public override void StartCheck()
        {
            if (BuildMode == (int)BuildModes.Comparative)
            {
                CompareDeleteDifferences();
            }
            if (BuildMode == (int)BuildModes.WipeRecreate)
            {
                DeleteAll();
            }
        }
        public override bool StartBuild()
        {
            if (Writer.VerifyOK())
            {
                CopyModLoader();
                StartCheck();
                StartTransfer();
            }
            return false;
        }
        public override bool StartTransfer()
        {
            Writer.SendFile(@"C:\Users\Mowjoh\Desktop\drag.png", "drag.png");
            return false;
        }
        public override void CompareDeleteDifferences()
        {

        }
        public override void DeleteAll()
        {

        }
    }

    

}
