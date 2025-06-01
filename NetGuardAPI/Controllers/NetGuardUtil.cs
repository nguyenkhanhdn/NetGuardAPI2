namespace NetGuardAPI.Controllers
{
    public class NetGuardUtil
    {
        public static string GetUploadDir()
        {
            var upload_Dir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "wwwroot/uploads"));
            return upload_Dir;
        }
    }
}
