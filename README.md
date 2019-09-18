> I am leaving Azure DevOps so this code will not be actively worked on. If you would like to take ownership of the repo please let us know.

# azure-devops-process-cli
CLI to manage some actions for inherited process in Azure DevOps

### Supported actions are 
- list all fields
- get field
- add field (with defined refname)

### Arguments

```
/org:{value}           azure devops organization name
/pat:{value}           personal access token
            
/action:{value}        listallfields, getfield, addfield
/refname:{value}       refname of field getting or adding
/name:{value}          field friendly name
/type:{value}          type field creating        
```
       
### Examples

```
/org:fabrikam /pat:{value} /action:listallfields
/org:fabrikam /pat:{value} /action:getfield /refname:System.Title
```
