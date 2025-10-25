LIVE_THREADS = []
THREADS_IN_MONITOR_MODE = []

def add_thread(thread):
    global LIVE_THREADS
    LIVE_THREADS.append(thread)

def remove_thread(thread):
    global LIVE_THREADS
    LIVE_THREADS.remove(thread)

def add_monitor_thread(thread):
    global THREADS_IN_MONITOR_MODE
    THREADS_IN_MONITOR_MODE.append(thread)

def remove_monitor_thread(thread):
    global THREADS_IN_MONITOR_MODE

def _are_monitored_threads():
    global THREADS_IN_MONITOR_MODE
    if len(THREADS_IN_MONITOR_MODE) > 0:
        return True
    else:
        return False