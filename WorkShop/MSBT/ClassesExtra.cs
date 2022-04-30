using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsbtEditor;

namespace MSBTEditorCli
{
    public class MainJSON
    {
        public Header header = new Header();
        public LBL1 LBL1 = new LBL1();
        public NLI1 NLI1 = new NLI1();
        public ATO1 ATO1 = new ATO1();
        public ATR1 ATR1 = new ATR1();
        public TSY1 TSY1 = new TSY1();
        public TXT2 TXT2 = new TXT2();
        public List<string> SectionOrder = new List<string>();

        public List<JsonMSBT> strings;
    }
    public class JsonMSBT
    {
        public string label;
        public string value;
    }
    
    public class MSBTList
    {
        public bool Sorted;
        public List<IEntry> Items = new List<IEntry>();
    }


}
    