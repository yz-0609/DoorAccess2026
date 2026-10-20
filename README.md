# 学生考勤与人脸识别管理系统

一个使用 C#、Windows Forms 和 SQL Server 的桌面项目，解决方案名称为 `WindowsFormsApplication2`，目标框架为 **.NET Framework 4.5.2**，原始开发环境为 Visual Studio 2015。

## 当前功能

- 主界面显示当前时间，并提供管理界面入口。
- 管理界面包含注册、用户管理、考勤查询和设置页签。
- 通过 LINQ to SQL 查询学生信息并绑定到表格；选中学生后回填学号、姓名、班级和性别。
- 封装 SQL Server 查询、更新操作，并包含读取已注册学生与人脸信息的方法。
- 集成 `FacePlatformCtrl.BioFPCtrl` 人脸控件，包含模块加载和事件入口。
- 提供通过 Windows MCI 播放音频的辅助类。

项目仍在开发中。注册、识别回调、考勤查询和部分按钮逻辑尚未实现；目前“卸载”分支也调用 `ModuleLoad()`，需要结合 SDK 文档完善。界面上的设备、数据库状态文字不代表实际连接检测结果。

## 目录结构

```text
WindowsFormsApplication2.sln        Visual Studio 解决方案
WindowsFormsApplication2/
  WindowsFormsApplication2.csproj  .NET Framework WinForms 项目
  Program.cs                      程序入口，启动 frmMain
  frmMain.cs / frmMain.Designer.cs 主界面及控件布局
  frmManage.cs / *.Designer.cs     管理界面及控件布局
  common.cs                       窗体实例管理
  DBService.cs                    ADO.NET 数据访问封装
  student.dbml / student.designer.cs
                                  学生与人脸表的 LINQ to SQL 映射
  DataClasses1.*                  另一组 LINQ to SQL 文件
  ds_stu.* / ds_student.* / ds_stuface.*
                                  类型化 DataSet 及设计文件
  App.config                      运行时配置与 DoorAccess2026 连接串
  Properties/                     程序设置、资源及程序集信息
  bin/Debug/                      本机现有输出、SDK DLL 和音频（Git 忽略）
  obj/                            编译中间文件（Git 忽略）
```

## 开发与运行准备

### 1. 开发环境

- Windows。
- Visual Studio 2015，或支持该旧式 C# 工程的 Visual Studio / MSBuild 环境。
- .NET Framework 4.5.2 开发工具包（Targeting Pack）及相应运行环境。
- SQL Server，以及具有目标数据库访问权限的 Windows 账户。
- 人脸识别 SDK、其原生运行库，以及 SDK 所需的设备、驱动和其他运行条件。

### 2. 人脸识别与音频依赖

当前项目引用 `FacePlatformAx` 和 `FacePlatformCtrl`；后者的 `HintPath` 指向 `bin\Debug\FacePlatformCtrl.dll`。本机 `WindowsFormsApplication2/bin/Debug/` 中现有以下依赖：

```text
FacePlatformAx.dll
FacePlatformCtrl.dll
face_platform.dll
face_eye_core.dll
MNN.dll
opencv_core2413.dll
opencv_highgui2413.dll
opencv_imgproc2413.dll
Sound/*.wav
```

这些文件位于被忽略的 `bin/` 目录中，**不会随 Git 仓库分发**，也不会因初始化 Git 被删除。首次在其他机器上使用前，需要从原有开发环境或 SDK 安装包准备这些依赖。项目没有 NuGet 依赖清单可自动还原它们。

使用 Visual Studio 检查并重新定位两个托管程序集引用，将原生 DLL 和所需 `Sound/` 音频放到程序实际输出目录。Release 构建同样需要准备运行依赖。若 SDK 对进程位数有要求，应据其说明选择平台；项目当前配置为 `AnyCPU`。

### 3. 配置数据库

数据库名称为 `myspl`。当前有多处数据库配置，迁移环境时需要分别检查：

| 位置 | 用途 | 当前配置 |
| --- | --- | --- |
| `App.config` 中的 `DoorAccess2026` | `DBService` 使用的连接串 | 指向原开发机的 SQL Server，使用 Windows 身份验证 |
| `Properties/Settings.settings` 中的 `mysplConnectionString` | `studentDataContext` 使用的应用设置 | `Data Source=.;Initial Catalog=myspl;Integrated Security=True` |
| `student.dbml` | LINQ to SQL 设计时连接 | 本机默认实例的 `myspl` |
| `frmManage.Designer.cs` 中的 `sqlConnection1` | 管理界面的 DataAdapter 连接 | 本机默认实例的 `myspl` |

例如，连接本机默认实例时，`DoorAccess2026` 可配置为：

```xml
<add name="DoorAccess2026"
     connectionString="Data Source=.;Initial Catalog=myspl;Integrated Security=True"
     providerName="System.Data.SqlClient" />
```

可通过 Visual Studio 的设置和数据设计器修改相应配置并重新生成代码，避免只改生成的 `Settings.Designer.cs`。当前连接使用 Windows 身份验证；实际账户需要具备所需的读写权限。

`student.dbml` 定义了以下主要表映射：

- `TBL_student`：`ID`、`StudID`、`StuName`、`Class`、`Sex`、`CreateTime`。
- `TBL_face`：`ID`、`StuID`、`FaceCode`；通过 `StuID` 与学生的 `StudID` 关联。

仓库中没有数据库创建脚本或备份。运行数据功能前，需要准备匹配映射的数据库；字段类型与可空性可参考 `student.dbml` 和 DataSet 定义。

注意：`ds_stuface.xsd` 中的人脸表名称为 `tal_face`，与 `student.dbml` 和查询代码中的 `TBL_face` 不一致，后续使用该 DataSet 时需要核对实际表名。

### 4. 构建与启动

1. 打开根目录的 `WindowsFormsApplication2.sln`。
2. 完成 SDK 引用、数据库连接和输出目录运行依赖的准备。
3. 生成解决方案，然后按 F5 启动。

也可在已配置好对应工具的 Visual Studio 开发人员命令提示符中构建：

```bat
msbuild WindowsFormsApplication2.sln /p:Configuration=Debug /p:Platform="Any CPU"
```

默认输出为 `WindowsFormsApplication2/bin/Debug/WindowsFormsApplication2.exe`。项目没有自动化测试工程；涉及数据库、设备或识别流程的行为需要在完整运行环境下验证。

## Git 文件约定

`.gitignore` 排除 VS 本地状态、`bin/`、`obj/`、发布与测试输出、日志、临时文件、本地数据库及备份等文件。窗体设计器、资源、LINQ to SQL 和 DataSet 生成源码仍应纳入版本控制，因为工程直接引用了它们。

`App.config` 和 `Properties/Settings.settings` 保留在版本控制范围内，供说明项目配置。不要将真实数据库口令或私钥写入这些受跟踪文件；`.gitignore` 无法隐藏已跟踪文件中的敏感内容。
