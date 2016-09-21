namespace MB.Model
{
    public class LinkFormulaModel
    {
        //序列
        public int? serialNumber { get; set; }

        //条件公式ID
        public int? formulaId { get; set; }

        //节点出口ID
        public int? linkId { get; set; }

        //流程条件ID
        public int? conditionId { get; set; }

        //表示名
        public string displayText { get; set; }

        //操作符 |,&,(,)
        public string operate { get; set; }

        //排序
        public int? orderNum { get; set; }
    }
}