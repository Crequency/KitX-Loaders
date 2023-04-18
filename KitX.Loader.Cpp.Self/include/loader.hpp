#ifndef KITX_LOADER_CPP_SELF_LOADER_HPP
#define KITX_LOADER_CPP_SELF_LOADER_HPP

#include <kitx_contract.hpp>
#include <ip_address.hpp>

namespace KitX::Loaders::Cpp::Self{

    Contract::Cpp::IIdentifyInterface *identifyInterface;

    void Init(Contract::Cpp::IIdentifyInterface *iidi);

    int BeginConnect(ip_address address, short port);
}

#endif //KITX_LOADER_CPP_SELF_LOADER_HPP
