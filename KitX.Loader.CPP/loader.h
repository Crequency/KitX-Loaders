#pragma once
#include <string>
#include <iostream>
#include <vector>
#include <winsock2.h>
#include "configor/json.hpp"
using namespace configor;
namespace KitX {
    class Plugin {
    private:
        json::value pluginStruct;
    public:
        Plugin(std::string myPluginName,std::string myAuthName){
            pluginStruct["pluginName"] = myPluginName;
            pluginStruct["authName"] = myAuthName;
        }
        int beginConnect(std::string address, int port, std::string myPluginStruct) {
            //cpp socket 通信,参数有地址,端口,插件信息(pluginStruct序列化之后的字符串),地址和端口插件启动时会以命令行参数的形式传入(在文档里约定)
            return 0;
        }
    };
}

