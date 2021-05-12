#include "connector.h"
#include <main.h>

#include <sdk_version.h>
#include <cellstatus.h>
#include <cell/cell_fs.h>
#include <cell/rtc.h>
#include <cell/gcm.h>
#include <cell/pad.h>
#include <sys/vm.h>
#include <sysutil/sysutil_common.h>

#include <sys/prx.h>
#include <sys/ppu_thread.h>
#include <sys/event.h>
#include <sys/syscall.h>
#include <sys/socket.h>
#include <sys/types.h>
#include <sys/memory.h>
#include <sys/timer.h>
#include <sys/process.h>
SYS_MODULE_INFO("NetStubClient", 0, 1, 1);
SYS_MODULE_START(main);
SYS_MODULE_STOP(stopitgetsomehelp);
SYS_MODULE_EXIT(stopitgetsomehelp);
SYS_LIB_DECLARE_WITH_STUB( NetStubLib, SYS_LIB_AUTO_EXPORT, NetStubClient );
SYS_LIB_EXPORT( Connector_Connect, NetStubLib );
sys_ppu_thread_t tid;
int main()
{
    //Connector_Connect();
    sys_ppu_thread_create(&tid, Connector_Connect, 0, 1194, 24576, 0, "NetStub Client");
    return SYS_PRX_RESIDENT;
}
int stopitgetsomehelp()
{
    return SYS_PRX_STOP_OK;
}