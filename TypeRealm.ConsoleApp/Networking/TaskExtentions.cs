using System.Threading.Tasks;

namespace TypeRealm.ConsoleApp.Networking
{
    public static class TaskExtentions
    {
        /// <summary>
        /// Waits for task and returns IsCompleted status. Swallows exceptions.
        /// If instance is NULL, returns true.
        /// </summary>
        public static bool TryWait(this Task task)
        {
            if (task == null)
                return true;

            try
            {
                task.Wait();
            }
            catch
            {
                // Swallow exception.
            }

            return task.IsCompleted && !task.IsFaulted;
        }
    }
}
