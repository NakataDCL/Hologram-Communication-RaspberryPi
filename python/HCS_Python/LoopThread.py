# 引数として関数(func)と待ち時間[sec](interavl)を与える
# start()が実行されると、stop()で停止されるまで、intervalfuncを実行し続ける

import time
import threading


class LoopThread:
    def __init__(self, interval, func, args=()):
        self._interval = interval
        self._func = func
        self._args = args
        # 停止させるかのフラグ
        self._stop_event = threading.Event()
        # スレッドの作成
        self._thread = threading.Thread(target=self.target)

    def target(self):
        while not self._stop_event.is_set():
            self._func(*self._args)
            time.sleep(self._interval)

    # スレッドの開始
    def start(self):
        self._thread.start()

    # スレッドの停止
    def stop(self):
        self._stop_event.set()
        # スレッドが停止するのを待つ
        self._thread.join()

    def is_alive(self):
        return self._thread.is_alive()
