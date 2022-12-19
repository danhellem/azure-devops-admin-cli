# azure-devops-admin-cli
CLI to manage work item admin tasks in Azure DevOps

## Arguments

```
/org:{value}            azure devops organization name
/pat:{value}            personal access token
            
/action:{value}         listallfields, getfield, addfield, listfieldsforprocess, allpicklists, picklistswithnofield, emptyrecyclebin
/refname:{value}        refname of field getting or adding
/name:{value}           field friendly name
/type:{value}           type field creating  

/days:{value}           used with emptyrecyclebin action. Number of days in the past from today to destroy work items
```

## 📃Process

```
listallfields           lists all fields in the organization
getfield                get a specific field by refname
addfield                add a field
listfieldsforprocess    list of fields in a process
allpicklists            list all picklists and the field they are associated to
picklistswithnofield    picklists that are not being used
```

### Examples

```
/org:{organization name} /pat:{value} /action:listallfields
/org:{organization name} /pat:{value} /action:allpicklists
/org:{organization name} /pat:{value} /action:picklistswithnofield
/org:{organization name} /pat:{value} /action:listfieldsforprocess /process:Agile
/org:{organization name}/pat:{value} /action:getfield /refname:System.Title
```

## ♻️ Recyle bin

Used to delete work items forever out the recycle bin.

```
/action:emptyrecyclebin /org:{organization name} /pat:{value} /project:{project name} /days:0
```

### Examples

Empty everything from the recycle bin
```
/action:emptyrecyclebin /org:{organization name} /pat:{value} /project:{project name} /days:0
```

Empty work items that have not been updated in the last 365 days
```
/org:{organization name} /pat:{value} /project:{project name} /days:365
```

## 🏷️ Tags 

Find all the tags that are not used and can be deleted.

```
/action:listemptytags /org:{organization name} /pat:{value} /project:{project name}
```