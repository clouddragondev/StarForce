--
-- 配置表管理器
--
--
ConfigMgr = {}

ConfigMgr.data = 
{
	[1] = "第一个",
	[2] = "第二个",
	[3] = "第三个",
	[4] = "第四个",
}

function ConfigMgr.Init()
	print("ConfigMgr init ... ok !")
end

function ConfigMgr.GetDataBy(key)
	--
	-- xlua 导出静态类方法的时候 访问方法使用的是. 
	-- xlua 到处的类 生成的对象  访问方法使用的是:
	-- 这玩意儿 我觉得没什么实际作用 一般都会导出lua文件 访问还方便  ^_^; 
	--
	return CS.StarForce.LuaBrige.LocalizationGetString(key)
end