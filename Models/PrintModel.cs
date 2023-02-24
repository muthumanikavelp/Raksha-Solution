namespace Raksha.Models
{
    public class PrintModel
    {
        public string ClientName { get; set; }
        public string Client_Gid { get; set; }

        public string CategoryID { get; set; }
        public string CategoryName { get; set; }

        public string MemberId { get; set; }
        public string MemberName { get; set; }

        public string Policyno { get; set; }
        public string Relation { get; set; }
        public string Address { get; set; }

        public string ImageName { get; set; }
    }

    public class PrintPDFData
    {
        public IList<PrintPDFModel> Detail { get; set; }
    }
    public class PrintPDFModel
    {
        public string pClientName { get; set; }
        public string pMemberName { get; set; }
        public string pPolicyno { get; set; }
        public string pMemberId { get; set; }

        public string pImageName { get; set; }
    }
}
