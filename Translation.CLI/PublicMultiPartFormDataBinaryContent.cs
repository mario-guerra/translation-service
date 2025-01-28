using TranlsationService;

namespace Translation.CLI
{
    public class PublicMultiPartFormDataBinaryContent : MultiPartFormDataBinaryContent
    {
        public new string ContentType => base.ContentType;
    }
}