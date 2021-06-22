# Mr. Resolution

```
Dark Helmet: What's the matter with this thing? What's all this churning and bubbling, you call that a radar screen?
Colonel Sandurz: No sir, we call it 'Mr. Resolution'. Care for some?
Dark Helmet: [pause] Yes. I always have a great resolution when I use virtualized Windows on Linux. You know that.
Colonel Sandurz: Of course, I do.
Dark Helmet: Everybody knows that!
Crewmen: Of course, we do, sir!
```

If you use Windows in a libvirt/qemu environment like me, you maybe also use looking glass to display the vm.

Unfortunately it does not adjust the clients desktop resolution based on the looking glass window which is a bit tedious to work with especially in tiling wms.

Mr. Resolution is basically a quick proof of concept that uses the Nvidia API to create custom resolutions. 

The goal is to integrate the virtual machine in a way, that it does not appear as one.

Pretty sure it's possible with AMD as well, unfortunately my AMD gpu is currently used somewhere else.

It creates a simple web server that can be called like so:

`curl -X POST http://192.168.122.237:9696/display/resolution  --verbose -d "width=1920&height=500"`

The service should change the resolution pixel perfect. It takes a few seconds to apply the custom resolution though. 

One thing to consider is, that it really actually changes the clients resolution with all drawbacks.

Windows sometimes behaves weirdly on resolution changes, shrinking and moving windows can happen. 

If you have the important applications maximized it should be fine - most of the time. 

# Usage

As this is just a web service you will also require a software that actually sends the window geometry to the guest. In AwesomeWM I do it that way:

```lua
local lookingGlassFilter = function (c)
    return awful.rules.match(c, {class = "looking-glass-client"})
end
              

function resizeLookingGlassWindow(c)
    clients = awful.client.iterate(lookingGlassFilter)

    for c in clients do
        print(c.name .. ' ' .. c.width .. 'x' .. c.height);
        
        queryString = "curl -X POST http://192.168.122.237:9696/display/resolution -d 'width=" .. c.width .. "&height=" .. c.height .. "' &"

        awful.spawn.with_shell(queryString)
    end
end

...
clientkeys = mytable.join(
    awful.key({modkey}, "o", resizeLookingGlassWindow, {description = "Test resize", group = "launcher"}), 
...
```

With other wms/dms you need to write a script or use something existing. Pretty sure those things exist.
