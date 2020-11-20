
local buttons = {"Center Screen/Buttons/Start", "Center Screen/Buttons/Setting", "Center Screen/Buttons/About", "Center Screen/Buttons/Quit"}
local Texts = {"Center Screen/Headline/Description", "Center Screen/Headline/Title", "Center Screen/Buttons/Start/Text", "Center Screen/Buttons/Setting/Text", "Center Screen/Buttons/About/Text", "Center Screen/Buttons/Quit/Text"}
local UIFormId = { 1, 100, 101, 102}
local procedure = nil

local SETTING_INDEX = 101
local AHOUT_INDEX = 102

MenuForm = {}

function MenuForm.OnRecycle()
	print("mmmm....OnRecycle...")
end

local function AddButtonClick(button, invokeFunc)
	local event = CS.UnityEngine.Events.UnityEvent()
	event:AddListener(invokeFunc)
	button.m_OnClick = event
end

local function CheckButtonEvent(name, invokeFunc)
	local button = self.gameObject.transform:FindChild(name)
	if button then 
		print("button find...")
		local commonButton = button:GetComponent("StarForce.CommonButton")
		if commonButton then 
			AddButtonClick(commonButton, invokeFunc)
		else
			print("commonButton not find...")
		end
	else
		print("button not find...")
	end
end

local function asy_msg(index)
	if index == 1 then 
		procedure:StartGame()
	elseif index == 2 then 
		CS.StarForce.LuaBrige.UIOpenUIForm(SETTING_INDEX)
	elseif index == 3 then 
		CS.StarForce.LuaBrige.UIOpenUIForm(AHOUT_INDEX)
	elseif index == 4 then 
		CS.StarForce.LuaBrige.UIOpenDiagloge("AskQuitGame.Title", "AskQuitGame.Message")
	end
end

function MenuForm.OnInit(procedureMenu)
	if procedureMenu == nil then 
		print("mmmm....OnInit...failed ")
		return 
	end

	print("mmmm....OnInit...")
	procedure = procedureMenu
	if self.gameObject == nil then 
		print("gameObject is nil...")
		return 
	end

	for i = 1, 6 do
		local parent = self.gameObject.transform:FindChild(Texts[i])
		if parent then 
			--local texts = parent:FindChild("Text"):GetComponent("UnityEngine.UI.Text")
			local texts = parent:GetComponent("UnityEngine.UI.Text")
			local title = ConfigMgr.GetDataBy(texts.text)
			texts.text = title
		else
			print("check ["..arr[i].."] is nil")
		end
	end

	for j = 1, 4 do
		local name = buttons[j]
		CheckButtonEvent(name, function()
			print("click index : "..j)
			print("UIFormId : "..UIFormId[j])
			asy_msg(j)
		end)
	end

end

function MenuForm.OnOpen(vv)
	print("mmmm....OnOpen...")
end

function MenuForm.OnClose(bvalue)
	local v = "yes"
	if bvalue then 
		v = "no"
	end
	print("mmmm....OnClose... :  "..v.." ")
end

function MenuForm.OnPause()
	print("mmmm....OnPause...")
end

function MenuForm.OnResume()
	print("mmmm....OnResume...")
end

function MenuForm.OnCover()
	print("mmmm....OnCover...")
end

function MenuForm.OnReveal()
	print("mmmm....OnReveal...")
end

function MenuForm.OnRefocus(usedata)
	print("mmmm....OnRefocus...")
end

function MenuForm.OnUpdate(time, values)
	-- print("mmmm....OnUpdate...:"..time.." vv: "..values)
end

function MenuForm.OnDepthChanged(v1, v2)
	print("mmmm....OnDepthChanged... : ")
end

function MenuForm.InternalSetVisible()
	print("mmmm....InternalSetVisible...")
end

