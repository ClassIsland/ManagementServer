const roles = [
  {
    id: "ObjectsWrite",
    name: "对象写入",
    description: "允许修改和创建对象（如课表、时间表、策略、对象组等）。"
  },
  {
    id: "ObjectsDelete",
    name: "对象删除",
    description: "允许删除对象（如课表、时间表、策略、对象组等）。"
  },
  {
    id: "ClientsWrite",
    name: "实例写入",
    description: "允许修改和创建实例和实例分组。"
  },
  {
    id: "ClientsDelete",
    name: "实例删除",
    description: "允许删除实例和实例分组。"
  },
  {
    id: "CommandsUser",
    name: "使用命令",
    description: "允许向客户端发送命令，如广播消息和重启客户端等。"
  },
  {
    id: "UsersManager",
    name: "用户管理员",
    description: "允许创建用户、删除用户和修改用户的基本信息、角色和密码。"
  },
  {
    id: "Admin",
    name: "管理员",
    description: "允许查看和修改系统关键设置。"
  },
]

export default roles;
