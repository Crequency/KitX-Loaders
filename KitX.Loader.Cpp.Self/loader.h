#ifndef KITX_LOADER_CPP_SELF_LOADER_H
#define KITX_LOADER_CPP_SELF_LOADER_H
#include "kitx_contract.h"

namespace KitX::Loaders::Cpp::Self{
    class Plugin
    {
    private:
        KitX::Contract::Cpp::IIdentifyInterface pluginInfo;
    public:
        Plugin(KitX::Contract::Cpp::IIdentifyInterface myPluginInfo);
    };
}

#endif //KITX_LOADER_CPP_SELF_LOADER_H
