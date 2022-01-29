using System.ComponentModel;

namespace AmbroBlogProject.Enums
{
    public enum ModerationType
    {
        [Description("Political propoganda")]
        Political,
        [Description("Offensive language")]
        Language,
        [Description("Drug references")]
        Drugs,
        [Description("Threatening language")]
        Threatening,
        [Description("Sexual content")]
        Sexual,
        [Description("Hate speech")]
        HateSpeech,
        [Description("Targeted shaming")]
        Shaming,
        [Description("Spam content")]
        Spamming,
    }
}
