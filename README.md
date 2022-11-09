# azure-devops-admin-cli
CLI to manage work item admin tasks in Azure DevOps

### Arguments

```
/org:{value}            azure devops organization name
/pat:{value}            personal access token
            
/action:{value}         listallfields, getfield, addfield, listfieldsforprocess, allpicklists, picklistswithnofield
/refname:{value}        refname of field getting or adding
/name:{value}           field friendly name
/type:{value}           type field creating        
```

### Supported Actions

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
/org:fabrikam /pat:{value} /action:listallfields
/org:fabrikam /pat:{value} /action:allpicklists
/org:fabrikam /pat:{value} /action:picklistswithnofield
/org:fabrikam /pat:{value} /action:listfieldsforprocess /process:Agile
/org:fabrikam /pat:{value} /action:getfield /refname:System.Title
```
