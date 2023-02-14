#ifndef KITX_LOADER_CPP_SELF_LOADER_H
#define KITX_LOADER_CPP_SELF_LOADER_H

#include "modules/ip_address/ip_address.h"
#include "kitx_contract.h"

namespace KitX::Loaders::Cpp::Self{
    void Init(Contract::Cpp::IIdentifyInterface *iidi);

    int BeginConnect(Self::Include::ip_address address, short port);
}

#endif //KITX_LOADER_CPP_SELF_LOADER_H
