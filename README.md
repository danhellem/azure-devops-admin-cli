# azure-devops-admin-cli
CLI to manage work item admin tasks in Azure DevOps

## Building and running

 1. Open solution in Visual Studio and build the project.
 2. Open bin > release folder (or debug) to find witadmin.exe
 3. Run command line ```adoadmin.exe```
 4. Add arguments below to run specific commands
 
 #### Example
 
 ```
 adoadmin.exe /org:myorgname /pat:1461fe40a1074619b1b29438ad19c71b /action:listallfields
 ```

## Arguments

```
/org:{value}            azure devops organization name
/pat:{value}            personal access token
            
/action:{value}         listallfields, getfield, addfield, listfieldsforprocess, allpicklists, picklistswithnofield, emptyrecyclebin
/refname:{value}        refname of field getting or adding
/name:{value}           field friendly name
/type:{value}           type field creating  

/days:{value}           used with emptyrecyclebin and list-delete-plans action. Number of days in the past from today
```

## 📃Process

```
listallfields           lists all fields in the organization
getfield                get a specific field by refname
addfield                add a field
listfieldsforprocess    list of fields in a process
allpicklists            list all picklists and the field they are associated to
picklistswithnofield    picklists that are not being used
list-delete-plans       list out and delete (optional) delivery plans that have not been accessed in x number of days
```

### Examples

```
adoadmin.exe /org:{organization name} /pat:{value} /action:listallfields
adoadmin.exe /org:{organization name} /pat:{value} /action:allpicklists
adoadmin.exe /org:{organization name} /pat:{value} /action:picklistswithnofield
adoadmin.exe /org:{organization name} /pat:{value} /action:listfieldsforprocess /process:Agile
adoadmin.exe /org:{organization name} /pat:{value} /action:getfield /refname:System.Title
```

## 📅 Delivery Plans

Clean up Delivery Plans that have not been accessed over a number of days. This can be helpful when trying to stay under the 1,000 plan limit per project. We recommend deleting plans that have not had any activity in the last 3-6 months.

```
list-delete-plans       list out and delete (optional) delivery plans that have not been accessed in x number of days
```

### Example
```
adoadmin.exe /org:{organization name} /pat:{value} /action:list-delete-plans /days:182
```

## ♻️ Recyle bin

Used to delete work items forever out the recycle bin.

```
adoadmin.exe /org:{organization name} /pat:{value} /action:emptyrecyclebin /project:{project name} /days:0
```

### Examples

Empty everything from the recycle bin
```
adoadmin.exe /org:{organization name} /pat:{value} /action:emptyrecyclebin /project:{project name} /days:0
```

Empty work items that have not been updated in the last 365 days
```
adoadmin.exe /org:{organization name} /pat:{value} /project:{project name} /days:365
```

## 🏷️ Tags 

Find all the tags that are not used and can be deleted.

```
adoadmin.exe /org:{organization name} /pat:{value} /action:listemptytags /project:{project name}
```

Delete a specific tag in a project.
```
adoadmin.exe /org:{organization name} /pat:{value} /action:deletetag /project:{project name} /name:{tag name}
```