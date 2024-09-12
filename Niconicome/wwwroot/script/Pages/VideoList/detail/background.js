(() => {
  // NiconicomeWeb/src/watch/background/background.ts
  if (self.onmessage === null) {
    self.onmessage = async (e) => {
      if (e.data === "") {
        return;
      }
      const message = JSON.parse(e.data);
      if (message.messageType === "startCreate") {
        const url = message.url;
        const response = await fetch(url, {
          signal: AbortSignal.timeout(30 * 1e3)
        });
        if (response.ok) {
          const result = {
            isSucceeded: true,
            message: void 0,
            messageType: "createCompleted"
          };
          postMessage(JSON.stringify(result));
        } else {
          const result = {
            isSucceeded: false,
            message: await response.text(),
            messageType: "createCompleted"
          };
          postMessage(JSON.stringify(result));
        }
      }
    };
  }
})();
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsiLi4vLi4vLi4vLi4vLi4vLi4vTmljb25pY29tZVdlYi9zcmMvd2F0Y2gvYmFja2dyb3VuZC9iYWNrZ3JvdW5kLnRzIl0sCiAgInNvdXJjZXNDb250ZW50IjogWyJpbXBvcnQge1xyXG4gIENyZWF0ZUNvbXBsZXRlZCxcclxuICBNZXNzYWdlQmFzZSxcclxuICBTdGFydENyZWF0ZSxcclxufSBmcm9tIFwiLi4vc2hhcmVkL21lc3NhZ2UvbWVzc2FnZS50c1wiO1xyXG5cclxuaWYgKHNlbGYub25tZXNzYWdlID09PSBudWxsKSB7XHJcbiAgc2VsZi5vbm1lc3NhZ2UgPSBhc3luYyAoZTogTWVzc2FnZUV2ZW50PHN0cmluZz4pID0+IHtcclxuICAgIGlmIChlLmRhdGEgPT09IFwiXCIpIHtcclxuICAgICAgcmV0dXJuO1xyXG4gICAgfVxyXG5cclxuICAgIGNvbnN0IG1lc3NhZ2UgPSBKU09OLnBhcnNlKGUuZGF0YSkgYXMgTWVzc2FnZUJhc2U7XHJcblxyXG4gICAgaWYgKG1lc3NhZ2UubWVzc2FnZVR5cGUgPT09IFwic3RhcnRDcmVhdGVcIikge1xyXG4gICAgICBjb25zdCB1cmwgPSAobWVzc2FnZSBhcyBTdGFydENyZWF0ZSkudXJsO1xyXG5cclxuICAgICAgY29uc3QgcmVzcG9uc2UgPSBhd2FpdCBmZXRjaCh1cmwsIHtcclxuICAgICAgICBzaWduYWw6IEFib3J0U2lnbmFsLnRpbWVvdXQoMzAgKiAxMDAwKSxcclxuICAgICAgfSk7XHJcblxyXG4gICAgICBpZiAocmVzcG9uc2Uub2spIHtcclxuICAgICAgICBjb25zdCByZXN1bHQ6IENyZWF0ZUNvbXBsZXRlZCA9IHtcclxuICAgICAgICAgIGlzU3VjY2VlZGVkOiB0cnVlLFxyXG4gICAgICAgICAgbWVzc2FnZTogdW5kZWZpbmVkLFxyXG4gICAgICAgICAgbWVzc2FnZVR5cGU6IFwiY3JlYXRlQ29tcGxldGVkXCIsXHJcbiAgICAgICAgfTtcclxuICAgICAgICBwb3N0TWVzc2FnZShKU09OLnN0cmluZ2lmeShyZXN1bHQpKTtcclxuICAgICAgfSBlbHNlIHtcclxuICAgICAgICBjb25zdCByZXN1bHQ6IENyZWF0ZUNvbXBsZXRlZCA9IHtcclxuICAgICAgICAgIGlzU3VjY2VlZGVkOiBmYWxzZSxcclxuICAgICAgICAgIG1lc3NhZ2U6IGF3YWl0IHJlc3BvbnNlLnRleHQoKSxcclxuICAgICAgICAgIG1lc3NhZ2VUeXBlOiBcImNyZWF0ZUNvbXBsZXRlZFwiLFxyXG4gICAgICAgIH07XHJcbiAgICAgICAgcG9zdE1lc3NhZ2UoSlNPTi5zdHJpbmdpZnkocmVzdWx0KSk7XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9O1xyXG59XHJcbiJdLAogICJtYXBwaW5ncyI6ICI7O0FBTUEsTUFBSSxLQUFLLGNBQWMsTUFBTTtBQUMzQixTQUFLLFlBQVksT0FBTyxNQUE0QjtBQUNsRCxVQUFJLEVBQUUsU0FBUyxJQUFJO0FBQ2pCO0FBQUEsTUFDRjtBQUVBLFlBQU0sVUFBVSxLQUFLLE1BQU0sRUFBRSxJQUFJO0FBRWpDLFVBQUksUUFBUSxnQkFBZ0IsZUFBZTtBQUN6QyxjQUFNLE1BQU8sUUFBd0I7QUFFckMsY0FBTSxXQUFXLE1BQU0sTUFBTSxLQUFLO0FBQUEsVUFDaEMsUUFBUSxZQUFZLFFBQVEsS0FBSyxHQUFJO0FBQUEsUUFDdkMsQ0FBQztBQUVELFlBQUksU0FBUyxJQUFJO0FBQ2YsZ0JBQU0sU0FBMEI7QUFBQSxZQUM5QixhQUFhO0FBQUEsWUFDYixTQUFTO0FBQUEsWUFDVCxhQUFhO0FBQUEsVUFDZjtBQUNBLHNCQUFZLEtBQUssVUFBVSxNQUFNLENBQUM7QUFBQSxRQUNwQyxPQUFPO0FBQ0wsZ0JBQU0sU0FBMEI7QUFBQSxZQUM5QixhQUFhO0FBQUEsWUFDYixTQUFTLE1BQU0sU0FBUyxLQUFLO0FBQUEsWUFDN0IsYUFBYTtBQUFBLFVBQ2Y7QUFDQSxzQkFBWSxLQUFLLFVBQVUsTUFBTSxDQUFDO0FBQUEsUUFDcEM7QUFBQSxNQUNGO0FBQUEsSUFDRjtBQUFBLEVBQ0Y7IiwKICAibmFtZXMiOiBbXQp9Cg==
