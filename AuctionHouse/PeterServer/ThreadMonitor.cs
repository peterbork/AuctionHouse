
namespace PeterServer {
    class ThreadMonitor {
        public delegate void ThreadEventType(string message);
        public event ThreadEventType ThreadEvent;

        public void ThreadAction(string message) {
            ThreadEvent(message);
        }
    }
}
