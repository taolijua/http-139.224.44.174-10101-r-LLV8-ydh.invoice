
use port

create table [dbo].[sys_user](
	[id] [int] identity(1,1) not null,
	[user_code] [varchar](50) not null,
	[password] [varchar](32) null,
	[user_name] [varchar](50) null,
	[token_key] [varchar](100) null,
	[user_status] [int] null,
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

create table ccp_mawb(
   id   int  primary key  identity(1,1) not null  ,
   mawb_code varchar (50)     not null,
   voyage_no   varchar (20)     not null,
   create_time DATETIME  not null,
   destination_port VARCHAR(10) not null,
   note VARCHAR(10) not null,
   port_loading VARCHAR(10) not null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','主单表','user','dbo','table','ccp_mawb',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','主单单号','user','dbo','table','ccp_mawb','column','mawb_code'
go
execute sys.sp_addextendedproperty 'ms_description','航线号','user','dbo','table','ccp_mawb','column','voyage_no'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_mawb','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','目的地4字代码','user','dbo','table','ccp_mawb','column','destinationPort'
go
execute sys.sp_addextendedproperty 'ms_description','目的港','user','dbo','table','ccp_mawb','column','note'
go
execute sys.sp_addextendedproperty 'ms_description','起运港','user','dbo','table','ccp_mawb','column','port_loading'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_mawb','column','remark'
go

create table ccp_bag(
   id   int  primary key  identity(1,1) not null  ,
   bag_code varchar (50)     not null,
   bag_status   varchar (10)     not null,
   operate_status   varchar (10)     not null,
   create_time DATETIME  not null,
   mawb_id int   not null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','袋子表','user','dbo','table','ccp_bag',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','袋子单号','user','dbo','table','ccp_bag','column','bag_code'
go
execute sys.sp_addextendedproperty 'ms_description','状态','user','dbo','table','ccp_bag','column','bag_status'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_bag','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','主单id','user','dbo','table','ccp_bag','column','mawb_id'
go
execute sys.sp_addextendedproperty 'ms_description','操作作态','user','dbo','table','ccp_bag','column','operate_status'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_bag','column','remark'
go

create table ccp_bag_log(
   id   int  primary key  identity(1,1) not null  ,
   bag_id int    not null,
   create_time DATETIME  not null,
   user_id int   not null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','袋子日记表','user','dbo','table','ccp_bag_log',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','袋子id','user','dbo','table','ccp_bag_log','column','bag_id'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_bag_log','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','用户id','user','dbo','table','ccp_bag_log','column','user_id'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_bag_log','column','remark'
go

go
create table ccp_order(
   id   int  primary key  identity(1,1) not null  ,
   server_hawbcode varchar (50)     not null,
   order_status   varchar (10)     not null,
   create_time DATETIME  not null,
   mawb_id int null  ,
   bag_id int  null  ,
   lrsp_code varchar (10) null,
   lrsp_time varchar (10) null,
   lrsp_msg varchar (1000) null,
   ersp_code varchar (10) null,
   ersp_time varchar (10) null,
   ersp_msg varchar (1000) null,
   data_source varchar (10) null,
   own_source varchar (10) null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','订单表','user','dbo','table','ccp_order',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','订单单号','user','dbo','table','ccp_order','column','serve_hawbcode'
go
execute sys.sp_addextendedproperty 'ms_description','订单状态','user','dbo','table','ccp_order','column','order_status'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_order','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','主单id','user','dbo','table','ccp_order','column','mawb_id'
go
execute sys.sp_addextendedproperty 'ms_description','袋子id','user','dbo','table','ccp_order','column','bag_id'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_order','column','remark'
go
execute sys.sp_addextendedproperty 'ms_description','分单反馈代码','user','dbo','table','ccp_order','column','lrsp_code'
go
execute sys.sp_addextendedproperty 'ms_description','分单反馈审核时间','user','dbo','table','ccp_order','column','lrsp_time'
go
execute sys.sp_addextendedproperty 'ms_description','反馈信息','user','dbo','table','ccp_order','column','lrsp_msg'
go
execute sys.sp_addextendedproperty 'ms_description','清单反馈代码','user','dbo','table','ccp_order','column','ersp_code'
go
execute sys.sp_addextendedproperty 'ms_description','清单反馈审核时间','user','dbo','table','ccp_order','column','ersp_time'
go
execute sys.sp_addextendedproperty 'ms_description','清单反馈信息','user','dbo','table','ccp_order','column','ersp_msg'
go
execute sys.sp_addextendedproperty 'ms_description','接口或者导入','user','dbo','table','ccp_order','column','data_source'
go
execute sys.sp_addextendedproperty 'ms_description','是否公司的单','user','dbo','table','ccp_order','column','own_source'
go

create table ccp_electricity(
   id   int  primary key  identity(1,1) not null  ,
   order_id int not null,
   ebc_code varchar (50)   not null,
   ebc_name varchar (50)   not null,
   ebp_code   varchar (50) not null,
   ebp_name   varchar (50) not null,
   merchant_code varchar (50) not null,
   pay_code   varchar (50) not null,
   pay_name   varchar (50) not null,
   create_time DATETIME  not null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','电商表','user','dbo','table','ccp_electricity',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','电商代码','user','dbo','table','ccp_electricity','column','ebc_code'
go
execute sys.sp_addextendedproperty 'ms_description','电商姓名','user','dbo','table','ccp_electricity','column','ebc_name'
go
execute sys.sp_addextendedproperty 'ms_description','电商平台','user','dbo','table','ccp_electricity','column','ebp_code'
go
execute sys.sp_addextendedproperty 'ms_description','电商平台名称','user','dbo','table','ccp_electricity','column','ebp_name'
go
execute sys.sp_addextendedproperty 'ms_description','支付企业代码','user','dbo','table','ccp_electricity','column','pay_code'
go
execute sys.sp_addextendedproperty 'ms_description','支付企业名称','user','dbo','table','ccp_electricity','column','pay_name'
go
execute sys.sp_addextendedproperty 'ms_description','电商企业平台代码','user','dbo','table','ccp_electricity','column','merchant_code'
go
execute sys.sp_addextendedproperty 'ms_description','袋子id','user','dbo','table','ccp_electricity','column','bag_id'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_order','column','remark'
go


create table ccp_consignee(
   id   int  primary key  identity(1,1) not null  ,
   order_id   int not null  ,
   name varchar (50)     not null,
   phone   varchar (20)     not null,
   address   varchar (50)     not null,
   create_time DATETIME  not null,
   country varchar (20) null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','收件人','user','dbo','table','ccp_consignee',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','姓名','user','dbo','table','ccp_consignee','column','name'
go
execute sys.sp_addextendedproperty 'ms_description','电话','user','dbo','table','ccp_consignee','column','phone'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_consignee','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','地址','user','dbo','table','ccp_consignee','column','address'
go
execute sys.sp_addextendedproperty 'ms_description','国家','user','dbo','table','ccp_consignee','column','country'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_consignee','column','remark'
go

create table ccp_order_log(
   id   int  primary key  identity(1,1) not null  ,
   order_id int    not null,
   create_time DATETIME  not null,
   user_id int   not null,
   remark varchar (1000) null
)
go
execute sys.sp_addextendedproperty 'ms_description','订单日记表','user','dbo','table','ccp_order_log',null,null   -- 添加表的备注
go
execute sys.sp_addextendedproperty 'ms_description','订单id','user','dbo','table','ccp_order_log','column','order_id'
go
execute sys.sp_addextendedproperty 'ms_description','创建时间','user','dbo','table','ccp_order_log','column','create_time'
go
execute sys.sp_addextendedproperty 'ms_description','用户id','user','dbo','table','ccp_order_log','column','user_id'
go
execute sys.sp_addextendedproperty 'ms_description','备注','user','dbo','table','ccp_order_log','column','remark'
go