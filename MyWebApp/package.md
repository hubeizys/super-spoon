# MyWebApp 项目依赖说明

## 项目信息

- 项目类型：ASP.NET Core Web 应用
- 目标框架：.NET 7.0
- 语言：C#

## NuGet 包依赖

### Microsoft 基础包
- `Microsoft.AspNetCore.OpenApi` - 版本 7.0.14
  - ASP.NET Core OpenAPI 支持
- `Microsoft.EntityFrameworkCore` - 版本 7.0.14
  - Entity Framework Core ORM 框架
- `Microsoft.EntityFrameworkCore.SqlServer` - 版本 7.0.14
  - SQL Server 数据库提供程序

### 开发工具
- `Microsoft.VisualStudio.Web.CodeGeneration.Design` - 版本 7.0.11
  - ASP.NET Core 代码生成工具

### API 文档
- `Swashbuckle.AspNetCore` - 版本 6.5.0
  - Swagger API 文档生成工具

### 本地化支持
- `Microsoft.AspNetCore.Localization` - 版本 2.2.0
  - ASP.NET Core 本地化支持

## 前端依赖

### CSS 框架
- Bootstrap v5.1.0
  - 位置：`~/lib/bootstrap/`
  - 用途：响应式布局和UI组件

### JavaScript 库
- jQuery v3.7.1
  - 位置：`~/lib/jquery/`
  - 用途：DOM操作和AJAX请求
- Bootstrap Bundle v5.1.0
  - 位置：`~/lib/bootstrap/dist/js/`
  - 用途：Bootstrap JavaScript 组件支持

### 图标
- Bootstrap Icons v1.11.3
  - 来源：CDN
  - 用途：UI图标

## 开发环境要求

- Visual Studio 2022 或更高版本
- .NET SDK 7.0 或更高版本
- Node.js（用于前端开发）

## 项目特性

- MVC 架构
- API 支持
- 多语言支持（中文、英文）
- 实时日志分析
- 响应式设计

## 配置说明

主要配置文件：
- `appsettings.json`：应用程序主配置
- `appsettings.Development.json`：开发环境配置

## 发布说明

发布目录：`[Pp]ublish/`

### 发布方式

1. 框架依赖方式（Framework-Dependent）
   - 需要目标机器安装 .NET Runtime
   - 打包体积较小
```bash
dotnet publish -c Release -o ./publish
```

2. 独立部署方式（Self-Contained）
   - 包含完整运行时，无需安装 .NET Runtime
   - 可以在没有安装 .NET 的机器上运行
   - 打包体积较大
```bash
dotnet publish -c Release -r win-x64 --self-contained -o ./publish
```

3. 单文件发布（Single-File）
   - 将所有依赖打包成单个可执行文件
   - 最简单的部署方式
   - 包含完整运行时
   - 打包体积最大
```bash
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish
```

### 支持的运行时标识符（RID）
- Windows x64: `win-x64`
- Windows x86: `win-x86`
- Windows ARM64: `win-arm64`
- Linux x64: `linux-x64`
- Linux ARM64: `linux-arm64`
- macOS x64: `osx-x64`
- macOS ARM64: `osx-arm64`

### 发布后的运行方式

1. 框架依赖方式：
```bash
dotnet MyWebApp.dll
```

2. 独立部署和单文件方式：
```bash
# Windows
MyWebApp.exe

# Linux/macOS
./MyWebApp
```

### 环境变量配置
可以通过环境变量配置运行参数：
```bash
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:ASPNETCORE_URLS = "http://localhost:5000"

# Linux/macOS
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://localhost:5000
```