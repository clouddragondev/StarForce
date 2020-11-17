function OnRecycle()
	print("mmmm....OnRecycle...")
end

function OnInit()
	print("mmmm....OnInit...")
	if self.gameObject == nil then 
		print("gameObject is nil...")
		return 
	end
	local arr = {"Center Screen/Buttons/Start/Text", "Center Screen/Buttons/Setting/Text", "Center Screen/Buttons/About/Text", "Center Screen/Buttons/Quit/Text"}
	for i = 1, 4 do
		local parent = self.gameObject.transform:FindChild(arr[i])
		if parent then 
			--local texts = parent:FindChild("Text"):GetComponent("UnityEngine.UI.Text")
			local texts = parent:GetComponent("UnityEngine.UI.Text")
			texts.text = "index : "..i
		else
			print("check ["..arr[i].."] is nil")
		end
	end

	local button = self.gameObject.transform:FindChild("Center Screen/Buttons/Start")
	if button then 
		print("button find...")
		local commonButton = button:GetComponent("StarForce.CommonButton")
		if commonButton then 
			print("commonButton find...")
			-- commonButton.commonButtonClick:AddListener(function() 
			-- 	print("commonButton click...")
			-- end)
			local event = CS.UnityEngine.Events.UnityEvent()
			local invokeFunc = function() print('commonButton inoking') end
			event:AddListener(invokeFunc)
			commonButton.m_OnClick = event
			
			-- event:Invoke()
			-- event:RemoveListener(invokeFunc)
		else
			print("commonButton not find...")
		end
	else
		print("button not find...")
	end

	-- local mbutton = button:GetComponent("UnityEngine.UI.Button")
	-- if mbutton then 
	-- 	print("mbutton find....")
	-- 	mbutton.onClick:AddListener(function()
	-- 		print("Start click 1111")
	-- 	end)
	-- else
	-- 	print("mbutton not find....")
	-- end

	-- local size = #texts -1
	-- for i = 0 , size do
	-- 	print("index : "..i)
	-- 	-- texts[i].text = "index : "..i
	-- end

end

function OnOpen(vv)
	print("mmmm....OnOpen...")
end

function OnClose(bvalue)
	local v = "yes"
	if bvalue then 
		v = "no"
	end
	print("mmmm....OnClose... :  "..v.." ")
end

function OnPause()
	print("mmmm....OnPause...")
end

function OnResume()
	print("mmmm....OnResume...")
end

function OnCover()
	print("mmmm....OnCover...")
end

function OnReveal()
	print("mmmm....OnReveal...")
end

function OnRefocus(usedata)
	print("mmmm....OnRefocus...")
end

function OnUpdate(time, values)
	-- print("mmmm....OnUpdate...:"..time.." vv: "..values)
end

function OnDepthChanged(v1, v2)
	print("mmmm....OnDepthChanged... : ")
end

function InternalSetVisible()
	print("mmmm....InternalSetVisible...")
end

