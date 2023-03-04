var i=class{constructor(n,t,e){this.IsSucceeded=n,this.Data=t,this.Message=e}static Succeeded(n){return new i(!0,n,null)}static Fail(n){return new i(!1,null,n)}};var D=class{Get(n){let t;try{t=document.querySelector(n)}catch(e){return i.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`)}return t==null?i.Fail("\u6307\u5B9A\u3055\u308C\u305F\u8981\u7D20\u304C\u898B\u3064\u304B\u308A\u307E\u305B\u3093\u3002"):i.Succeeded(t)}GetAll(n){let t;try{t=document.querySelectorAll(n)}catch(e){return i.Fail(`\u8981\u7D20\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002(\u8A73\u7D30\uFF1A${e.message})`)}return i.Succeeded(t)}};var H=class{constructor(n){this._elmHandler=n}GetComputedStyle(n){let t=this._elmHandler.Get(n);if(!t.IsSucceeded||t.Data===null)return i.Fail(t.Message??"");try{let e=window.getComputedStyle(t.Data);return i.Succeeded(e)}catch{return i.Fail("\u30B9\u30BF\u30A4\u30EB\u3092\u53D6\u5F97\u3067\u304D\u307E\u305B\u3093\u3067\u3057\u305F\u3002")}}};var a=class{};a.VideoListRow=".VideoListRow",a.VideoListRowClassName="VideoListRow",a.VideoListBodyClassName="VideoListBody",a.DropTargetClassName="DropTarget";var y=class{constructor(n,t){this._sourceNiconicoID=null;this._sourceID=null;this._lastOverElement=null;this._elmHandler=n,this._dotnetHelper=t}Initialize(){let n=this._elmHandler.GetAll(a.VideoListRow);!n.IsSucceeded||n.Data===null||n.Data.forEach(t=>{t instanceof HTMLElement&&(t.addEventListener("dragstart",e=>{if(!(e.target instanceof HTMLElement))return;let l=this.GetParentByClassName(e.target,a.VideoListRowClassName);l!==null&&(this._sourceNiconicoID=l.dataset.niconicoid??null,this._sourceID=l.id)}),t.addEventListener("dragover",e=>{if(e.preventDefault(),!(e.target instanceof HTMLElement))return;let l=this.GetParentByClassName(e.target,a.VideoListRowClassName);l!==null&&(l.classList.contains(a.DropTargetClassName)||l.classList.add(a.DropTargetClassName),this._lastOverElement=l)}),t.addEventListener("dragleave",e=>{if(e.preventDefault(),!(e.target instanceof HTMLElement))return;let l=this.GetParentByClassName(e.target,a.VideoListRowClassName);l!==null&&l.classList.contains(a.DropTargetClassName)&&l.classList.remove(a.DropTargetClassName)}),t.addEventListener("drop",async e=>{if(e.preventDefault(),this._sourceNiconicoID===null)return;let l=this._elmHandler.Get(`#${this._sourceID}`);if(!l.IsSucceeded||l.Data===null||!(e.target instanceof HTMLElement))return;let s=e.target.parentNode,r=e.target;for(;s!==null;){if(!(s instanceof HTMLElement))return;if(!s.classList.contains(a.VideoListBodyClassName)){r=s,s=s.parentNode;continue}s.insertBefore(l.Data,r),await this._dotnetHelper.invokeMethodAsync("MoveVideo",this._sourceNiconicoID,r.dataset.niconicoid),s=null}this._lastOverElement!==null&&this._lastOverElement.classList.contains(a.DropTargetClassName)&&this._lastOverElement.classList.remove(a.DropTargetClassName)}))})}GetParentByClassName(n,t){let e=n;for(;e!==null;){if(!(e instanceof HTMLElement))return null;if(!e.classList.contains(t)){e=e.parentNode;continue}return e}return null}};var d=class{};d.PageContent=".PageContent",d.VideoListHeader="#VideoListHeader",d.Separator=".Separator";var g=class{constructor(n,t){this._isResizing=!1;this._elmHandler=n,this._styleHandler=t,this._columnIDs={0:"CheckBoxColumn",1:"ThumbnailColumn",2:"TitleColumn",3:"UploadedDateTimeColumn",4:"IsDownloadedColumn",5:"ViewCountColumn",6:"CommentCountColumn",7:"MylistCountColumn",8:"LikeCountColumn",9:"MessageColumn"},this._separatorIDs={0:"#CheckBoxColumnSeparator",1:"#ThumbnailColumnSeparator",2:"#TitleColumnSeparator",3:"#UploadedDateTimeColumnSeparator",4:"#IsDownloadedColumnSeparator",5:"#ViewCountColumnSeparator",6:"#CommentCountColumnSeparator",7:"#MylistCountColumnSeparator",8:"#LikeCountColumnSeparator"}}Initialize(){let n=0;for(let l in this._separatorIDs){let s=this._elmHandler.Get(this._separatorIDs[l]);if(!s.IsSucceeded||s.Data===null)continue;let r=s.Data;if(!(r instanceof HTMLElement))continue;let c=this._styleHandler.GetComputedStyle(`#${this._columnIDs[l]}`);if(c.IsSucceeded&&c.Data!==null){let p=c.Data,f=this._elmHandler.GetAll(`.${this._columnIDs[l]}`);if(!f.IsSucceeded||f.Data===null)continue;if(p.display==="none"){r.style.display="none",f.Data.forEach(o=>{o instanceof HTMLElement&&(o.style.display="none")});continue}else n+=Number(p.width.match(/\d+/)),r.style.left=`${n}px`,f.Data.forEach(o=>{o instanceof HTMLElement&&(o.style.width=p.width)})}let m=r.dataset.index;m!=null&&r.addEventListener("mousedown",p=>this.OnMouseDown(m))}let t=this._elmHandler.Get(d.PageContent);if(!t.IsSucceeded||t.Data===null||!(t.Data instanceof HTMLElement))return;t.Data.addEventListener("mouseup",l=>this.OnMouseUp());let e=this._elmHandler.Get(d.VideoListHeader);!e.IsSucceeded||e.Data===null||!(e.Data instanceof HTMLElement)||e.Data.addEventListener("mousemove",l=>this.OnMouseMove(l))}OnMouseDown(n){this._isResizing=!0,this._resizingIndex=n}OnMouseUp(){this._isResizing=!1,this._resizingIndex=null}OnMouseMove(n){if(!this._isResizing||this._resizingIndex===null)return;let t=Number(this._resizingIndex)+1,e=this._elmHandler.Get(`#${this._columnIDs[this._resizingIndex]}`),l=this._elmHandler.Get(`#${this._columnIDs[`${t}`]}`),s=this._elmHandler.Get(d.VideoListHeader),r=this._elmHandler.GetAll(`.${this._columnIDs[this._resizingIndex]}`),c=this._elmHandler.GetAll(`.${this._columnIDs[`${t}`]}`),m=this._elmHandler.Get(this._separatorIDs[this._resizingIndex]);if(!e.IsSucceeded||e.Data===null||!r.IsSucceeded||r.Data===null||!m.IsSucceeded||m.Data===null||!s.IsSucceeded||s.Data===null||!l.IsSucceeded||l.Data===null||!c.IsSucceeded||c.Data===null||!(e.Data instanceof HTMLElement)||!(l.Data instanceof HTMLElement))return;let p=e.Data.getBoundingClientRect(),f=s.Data.getBoundingClientRect(),o=n.clientX-p.left,S=o-e.Data.offsetWidth,C=l.Data.offsetWidth-S;if(e.Data.style.width=`${o}px`,l.Data.style.width=`${C}px`,r.Data.forEach(h=>{h instanceof HTMLElement&&(h.style.width=`${o}px`)}),c.Data.forEach(h=>{h instanceof HTMLElement&&(h.style.width=`${C}px`)}),!(s.Data instanceof HTMLElement)||!(m.Data instanceof HTMLElement))return;let L=p.left-f.left+o-10;m.Data.style.left=`${L}px`}};function k(u){let n=new D,t=new H(n),e=new g(n,t),l=new y(n,u);e.Initialize(),l.Initialize()}export{k as main};
