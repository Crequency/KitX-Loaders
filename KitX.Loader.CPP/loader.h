#pragma once
#include <string>
#include <iostream>
#include <vector>
#include <winsock2.h>
#include <windows.h>
#include "configor/json.hpp"
#include "configor/configor_wrapper.hpp"
using namespace configor;
namespace KitX
{
    class Plugin {
    private:
        json::value pluginStruct;
    public:
        Plugin(std::string myName, std::string myAuthorName, std::string Version, std::string myPluginDescription, std::string myPluginAuthor, std::string myPluginWebsite, std::string myPluginPath)
        {
            pluginStruct["Name"] = myName;
            pluginStruct["AuthorName"] = myAuthorName;
            pluginStruct["Version"] = Version;
            pluginStruct["pluginDescription"] = myPluginDescription;
            pluginStruct["pluginAuthor"] = myPluginAuthor;
            pluginStruct["pluginWebsite"] = myPluginWebsite;
            pluginStruct["pluginPath"] = myPluginPath;
        }
        int beginConnect(std::string address, int port)
        {
            //cpp socket 通信,参数有地址,端口,插件信息(pluginStruct序列化之后的字符串),地址和端口插件启动时会以命令行参数的形式传入(在文档里约定)
            //std::string pluginstr = json::dump(pluginStruct);
            std::string pluginstr = "{\"Name\":\"\u63D2\u4EF6\u540D\u79F0\",\"Version\":\"\u63D2\u4EF6\u7248\u672C\",\"DisplayName\":{\"zh-cn\":\"\u663E\u793A\u540D\u79F0\",\"zh-cnt\":\"\u986F\u793A\u540D\u7A31\",\"en-us\":\"Display Name\",\"ja-jp\":\"\u756A\u7D44\u540D\"},\"AuthorName\":\"\u4F5C\u8005\u540D\u79F0\",\"PublisherName\":\"\u53D1\u884C\u8005\u540D\u79F0\",\"AuthorLink\":\"\u4F5C\u8005\u94FE\u63A5\",\"PublisherLink\":\"\u53D1\u884C\u8005\u94FE\u63A5\",\"SimpleDescription\":{\"zh-cn\":\"\u7B80\u5355\u63CF\u8FF0\",\"zh-cnt\":\"\u7C21\u55AE\u63CF\u8FF0\",\"en-us\":\"Simple Description\",\"ja-jp\":\"\u7C21\u5358\u306A\u8AAC\u660E\"},\"ComplexDescription\":{\"zh-cn\":\"\u590D\u6742\u63CF\u8FF0\",\"zh-cnt\":\"\u8907\u96DC\u63CF\u8FF0\",\"en-us\":\"Complex Description\",\"ja-jp\":\"\u8907\u96D1\u306A\u8AAC\u660E\"},\"TotalDescriptionInMarkdown\":{\"zh-cn\":\"\u5B8C\u6574\u63CF\u8FF0\",\"zh-cnt\":\"\u5B8C\u6574\u63CF\u8FF0\",\"en-us\":\"Total Description\",\"ja-jp\":\"\u5B8C\u5168\u306A\u8AAC\u660E\"},\"IconInBase64\":\"\u56FE\u6807\",\"PublishDate\":\"2022-12-12T17:45:02.7415232+08:00\",\"LastUpdateDate\":\"2022-12-12T17:45:02.7434836+08:00\",\"IsMarketVersion\":false,\"Tags\":{},\"Functions\":[{\"Name\":null,\"DisplayNames\":{\"zh-cn\":\"\u4F60\u597D, \u4E16\u754C!\",\"en-us\":\"Hello, World!\"},\"Parameters\":{\"par1\":{\"zh-cn\":\"\u53C2\u65701\",\"en-us\":\"Parameter1\"}},\"ParametersType\":[\"void\"],\"HasAppendParameters\":false,\"ReturnValueType\":\"void\"}],\"RootStartupFileName\":\"TestPlugin.WPF.Core.dll\"}";
            //通过winsocket库进行通信
            WSADATA wsaData;
            WSAStartup(MAKEWORD(2, 2), &wsaData);
            SOCKET sock = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
            sockaddr_in addr;
            addr.sin_family = AF_INET;
            addr.sin_port = htons(port);
            addr.sin_addr.S_un.S_addr = inet_addr(address.c_str());
            connect(sock, (SOCKADDR*)&addr, sizeof(SOCKADDR));
            send(sock, pluginstr.c_str(), pluginstr.length(), 0);
            char buf[1024];
            recv(sock, buf, 1024, 0);
            std::string result(buf);
            std::cout << result << std::endl;
            Sleep(500);
            closesocket(sock);
            WSACleanup();            
            return 0;
        }
    };
}


