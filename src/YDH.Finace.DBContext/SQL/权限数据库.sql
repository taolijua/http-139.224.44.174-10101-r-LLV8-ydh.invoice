
use port

create table [dbo].[sys_user](
	[id] [int] identity(1,1) not null,
	[user_code] [varchar](50) not null,
	[password] [varchar](32) null,
	[user_name] [varchar](50) null,
	[token_key] [varchar](100) null,
	[user_status] [int] null,
	[CreatePerson] [varchar](50) null,
	[CreateTime] [datetime] null,
	[UpdatePerson] [varchar](50) null,
	[UpdateTime] [datetime] null,
primary key clustered
(
	[id] asc
)with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [primary]
) on [primary]
go

exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'用户代码' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'user_code'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'密码' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'password'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'用户名称(姓名)' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'user_name'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'用户tokeN' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'token_key'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'用户状态，0：正常，1：失效' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'user_status'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'创建人' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'CreatePerson'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'创建时间' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'CreateTime'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'修改人' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'UpdatePerson'
go
exec sys.sp_addextendedproperty @name=N'ms_descriptioN', @value=N'修改时间' , @level0type=N'schema',@level0name=N'dbo', @level1type=N'table',@level1name=N'sys_user', @level2type=N'columN',@level2name=N'UpdateTime'
go


go
create table sys_menu(
   id   int    primary key  identity(1,1) not null  ,
   menu_name varchar (20)     not null,
   menu_link varchar (100)    not null,
   menu_image text  not null,
   parent_id int default 0  ,
   openwindowmode varchar (10) default '0' ,
   menu_remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_descriptioN','菜单表','user','dbo','table','sys_menu',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_descriptioN','菜单名字','user','dbo','table','sys_menu','columN','menu_name'
go
execute sys.sp_addextendedproperty 'ms_descriptioN','页面链接','user','dbo','table','sys_menu','columN','menu_link'
go
execute sys.sp_addextendedproperty 'ms_descriptioN','菜单图标','user','dbo','table','sys_menu','columN','menu_image'
go
execute sys.sp_addextendedproperty 'ms_descriptioN','上级菜单','user','dbo','table','sys_menu','columN','parent_id'
go
execute sys.sp_addextendedproperty 'ms_descriptioN','备注','user','dbo','table','sys_menu','columN','menu_remark'
go




-- 角色表
create table sys_role(
   id   int    primary key  identity(1,1) not null,
   role_name varchar (20)     not null,
   motion_remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_descriptioN','角色表','user','dbo','table','sys_role',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_descriptioN','角色名字','user','dbo','table','sys_role','columN','role_name'
go
execute sys.sp_addextendedproperty 'ms_descriptioN','备注','user','dbo','table','sys_role','columN','motion_remark'
go

-- 角色用户关联表
create table sys_role_user(
   role_user_id   int    primary key  identity(1,1) not null,
   role_id   int     not null,
   user_id   int     not null
)
go
execute sys.sp_addextendedproperty 'ms_descriptioN','角色用户关联表','user','dbo','table','sys_role_user',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_descriptioN','用户id','user','dbo','table','sys_role_user','columN','user_id'
go

-- 角色权限表
create table sys_role_power(
   role_user_id   int    primary key  identity(1,1) not null,
   menu_id   int     not null
)
go
execute sys.sp_addextendedproperty 'ms_descriptioN','角色权限表','user','dbo','table','sys_role_power',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_descriptioN','用户id','user','dbo','table','sys_role_power','columN','menu_id'
go