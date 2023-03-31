var s=class{constructor(n,e,t){this.IsSucceeded=n,this.Data=e,this.Message=t}static Succeeded(n){return new s(!0,n,null)}static Fail(n){return new s(!1,null,n)}};var f=class{Get(n){let e;try{e=document.querySelector(n)}catch(t){return s.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${t.message})`)}return e==null?s.Fail("\u6307\u5B9A\u3055\u308C\u305F\u8981\u7D20\u304C\u898B\u3064\u304B\u308A\u307E\u305B\u3093\u3002"):s.Succeeded(e)}GetAll(n){let e;try{e=document.querySelectorAll(n)}catch(t){return s.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${t.message})`)}return s.Succeeded(e)}};var H=class{constructor(n){this._elmHandler=n}GetComputedStyle(n){let e=this._elmHandler.Get(n);if(!e.IsSucceeded||e.Data===null)return s.Fail(e.Message??"");try{let t=window.getComputedStyle(e.Data);return s.Succeeded(t)}catch{return s.Fail("\u30B9\u30BF\u30A4\u30EB\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002")}}};var g=class{constructor(n){this._elmHandler=n}getSelected(){let n=this._elmHandler.Get("#InputBox");if(!n.IsSucceeded||n.Data===null)return"";let e=n.Data;return!(e instanceof HTMLInputElement)||!this.IsValidIndex(e.value,e.selectionStart)||!this.IsValidIndex(e.value,e.selectionEnd,!0)?"":e.value.substring(e.selectionStart,e.selectionEnd)}IsValidIndex(n,e,t=!1){return!(e===null||t&&e>n.length||!t&&e>n.length-1)}};var r=class{};r.VideoListRow=".VideoListRow",r.VideoListRowClassName="VideoListRow",r.VideoListBodyClassName="VideoListBody",r.DropTargetClassName="DropTarget";var S=class{constructor(n,e){this._sourceNiconicoID=null;this._sourceID=null;this._lastOverElement=null;this._elmHandler=n,this._dotnetHelper=e}initialize(){let n=this._elmHandler.GetAll(r.VideoListRow);!n.IsSucceeded||n.Data===null||n.Data.forEach(e=>{e instanceof HTMLElement&&(e.addEventListener("dragstart",t=>{if(!(t.target instanceof HTMLElement))return;let l=this.GetParentByClassName(t.target,r.VideoListRowClassName);l!==null&&(this._sourceNiconicoID=l.dataset.niconicoid??null,this._sourceID=l.id)}),e.addEventListener("dragover",t=>{if(t.preventDefault(),!(t.target instanceof HTMLElement))return;let l=this.GetParentByClassName(t.target,r.VideoListRowClassName);l!==null&&(l.classList.contains(r.DropTargetClassName)||l.classList.add(r.DropTargetClassName),this._lastOverElement=l)}),e.addEventListener("dragleave",t=>{if(t.preventDefault(),!(t.target instanceof HTMLElement))return;let l=this.GetParentByClassName(t.target,r.VideoListRowClassName);l!==null&&l.classList.contains(r.DropTargetClassName)&&l.classList.remove(r.DropTargetClassName)}),e.addEventListener("drop",async t=>{if(t.preventDefault(),this._sourceNiconicoID===null)return;let l=this._elmHandler.Get(`#${this._sourceID}`);if(!l.IsSucceeded||l.Data===null||!(t.target instanceof HTMLElement))return;let a=t.target.parentNode,i=t.target;for(;a!==null;){if(!(a instanceof HTMLElement))return;if(!a.classList.contains(r.VideoListBodyClassName)){i=a,a=a.parentNode;continue}a.insertBefore(l.Data,i),await this._dotnetHelper.invokeMethodAsync("MoveVideo",this._sourceNiconicoID,i.dataset.niconicoid),a=null}this._lastOverElement!==null&&this._lastOverElement.classList.contains(r.DropTargetClassName)&&this._lastOverElement.classList.remove(r.DropTargetClassName)}))})}GetParentByClassName(n,e){let t=n;for(;t!==null;){if(!(t instanceof HTMLElement))return null;if(!t.classList.contains(e)){t=t.parentNode;continue}return t}return null}};var c=class{};c.PageContent=".PageContent",c.VideoListHeader="#VideoListHeader",c.Separator=".Separator";var y=class{constructor(n,e,t){this._isResizing=!1;this._elmHandler=n,this._styleHandler=e,this._dotnetHelper=t,this._columnIDs={0:"CheckBoxColumn",1:"ThumbnailColumn",2:"TitleColumn",3:"UploadedDateTimeColumn",4:"IsDownloadedColumn",5:"ViewCountColumn",6:"CommentCountColumn",7:"MylistCountColumn",8:"LikeCountColumn",9:"MessageColumn"},this._separatorIDs={0:"#CheckBoxColumnSeparator",1:"#ThumbnailColumnSeparator",2:"#TitleColumnSeparator",3:"#UploadedDateTimeColumnSeparator",4:"#IsDownloadedColumnSeparator",5:"#ViewCountColumnSeparator",6:"#CommentCountColumnSeparator",7:"#MylistCountColumnSeparator",8:"#LikeCountColumnSeparator"}}async initialize(){for(let t in this._separatorIDs){let l=this._elmHandler.Get(this._separatorIDs[t]);if(!l.IsSucceeded||l.Data===null)continue;let a=l.Data;if(!(a instanceof HTMLElement))continue;let i=a.dataset.index;i!=null&&a.addEventListener("mousedown",u=>this.OnMouseDown(i))}await this.setWidth();let n=this._elmHandler.Get(c.PageContent);if(!n.IsSucceeded||n.Data===null||!(n.Data instanceof HTMLElement))return;n.Data.addEventListener("mouseup",t=>this.OnMouseUp());let e=this._elmHandler.Get(c.VideoListHeader);!e.IsSucceeded||e.Data===null||!(e.Data instanceof HTMLElement)||e.Data.addEventListener("mousemove",t=>this.OnMouseMove(t))}async setWidth(){let n=0;for(let e in this._columnIDs){let t=null;if(e in this._separatorIDs){let a=this._elmHandler.Get(this._separatorIDs[e]);if(!a.IsSucceeded||a.Data===null||(t=a.Data,!(t instanceof HTMLElement)))continue}let l=this._styleHandler.GetComputedStyle(`#${this._columnIDs[e]}`);if(l.IsSucceeded&&l.Data!==null){let a=l.Data,i=this._elmHandler.GetAll(`.${this._columnIDs[e]}`);if(!i.IsSucceeded||i.Data===null)continue;if(a.display==="none"){t!==null&&(t.style.display="none"),i.Data.forEach(u=>{u instanceof HTMLElement&&(u.style.display="none")});continue}else{let u=await this._dotnetHelper.invokeMethodAsync("GetWidth",this._columnIDs[e]),m=u>0,p=m?u:Number(a.width.match(/\d+/));if(n+=p,t!==null&&(t.style.left=`${n}px`),m){let d=this._elmHandler.Get(`#${this._columnIDs[e]}`);d.IsSucceeded&&d.Data!==null&&d.Data instanceof HTMLElement&&(d.Data.style.width=`${p}px`)}i.Data.forEach(d=>{d instanceof HTMLElement&&(d.style.width=m?`${p}px`:a.width)})}}}}OnMouseDown(n){this._isResizing=!0,this._resizingIndex=n}OnMouseUp(){this._isResizing=!1,this._resizingIndex=null}async OnMouseMove(n){if(!this._isResizing||this._resizingIndex===null)return;let e=Number(this._resizingIndex)+1,t=this._columnIDs[this._resizingIndex],l=this._columnIDs[`${e}`],a=this._elmHandler.Get(`#${t}`),i=this._elmHandler.Get(`#${l}`),u=this._elmHandler.Get(c.VideoListHeader),m=this._elmHandler.GetAll(`.${t}`),p=this._elmHandler.GetAll(`.${l}`),d=this._elmHandler.Get(this._separatorIDs[this._resizingIndex]);if(!a.IsSucceeded||a.Data===null||!m.IsSucceeded||m.Data===null||!d.IsSucceeded||d.Data===null||!u.IsSucceeded||u.Data===null||!i.IsSucceeded||i.Data===null||!p.IsSucceeded||p.Data===null||!(a.Data instanceof HTMLElement)||!(i.Data instanceof HTMLElement))return;let _=a.Data.getBoundingClientRect(),L=u.Data.getBoundingClientRect(),h=n.clientX-_.left,R=h-a.Data.offsetWidth,E=i.Data.offsetWidth-R;if(a.Data.style.width=`${h}px`,i.Data.style.width=`${E}px`,m.Data.forEach(D=>{D instanceof HTMLElement&&(D.style.width=`${h}px`)}),p.Data.forEach(D=>{D instanceof HTMLElement&&(D.style.width=`${E}px`)}),await this._dotnetHelper.invokeMethodAsync("SetWidth",`${h}`,t),await this._dotnetHelper.invokeMethodAsync("SetWidth",`${E}`,l),!(u.Data instanceof HTMLElement)||!(d.Data instanceof HTMLElement))return;let C=_.left-L.left+h-10;d.Data.style.left=`${C}px`}};async function J(o){let n=new f,e=new H(n),t=new y(n,e,o),l=new S(n,o);await t.initialize(),l.initialize()}async function K(o){let n=new f,e=new H(n);await new y(n,e,o).setWidth()}function Q(){let o=new f;return new g(o).getSelected()}export{Q as getSelectedIOfInput,J as initialize,K as setWidth};
