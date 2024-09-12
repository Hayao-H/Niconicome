var __defProp = Object.defineProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};

// https://esm.sh/v135/react-dom@18.2.0/denonext/react-dom.mjs
var react_dom_exports = {};
__export(react_dom_exports, {
  __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: () => Tf,
  createPortal: () => Mf,
  createRoot: () => Df,
  default: () => Wf,
  findDOMNode: () => Of,
  flushSync: () => Rf,
  hydrate: () => Ff,
  hydrateRoot: () => If,
  render: () => Uf,
  unmountComponentAtNode: () => jf,
  unstable_batchedUpdates: () => Vf,
  unstable_renderSubtreeIntoContainer: () => Af,
  version: () => Bf
});

// https://esm.sh/stable/react@18.2.0/denonext/react.mjs
var react_exports = {};
__export(react_exports, {
  Children: () => le,
  Component: () => ae,
  Fragment: () => pe,
  Profiler: () => ye,
  PureComponent: () => de,
  StrictMode: () => _e,
  Suspense: () => me,
  __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: () => he,
  cloneElement: () => ve,
  createContext: () => Se,
  createElement: () => Ee,
  createFactory: () => Re,
  createRef: () => Ce,
  default: () => We,
  forwardRef: () => ke,
  isValidElement: () => we,
  lazy: () => be,
  memo: () => $e,
  startTransition: () => xe,
  unstable_act: () => Oe,
  useCallback: () => je,
  useContext: () => Ie,
  useDebugValue: () => ge,
  useDeferredValue: () => Pe,
  useEffect: () => Te,
  useId: () => De,
  useImperativeHandle: () => Ve,
  useInsertionEffect: () => Le,
  useLayoutEffect: () => Ne,
  useMemo: () => Fe,
  useReducer: () => Ue,
  useRef: () => qe,
  useState: () => Ae,
  useSyncExternalStore: () => Me,
  useTransition: () => ze,
  version: () => Be
});
var z = Object.create;
var E = Object.defineProperty;
var B = Object.getOwnPropertyDescriptor;
var H = Object.getOwnPropertyNames;
var W = Object.getPrototypeOf;
var Y = Object.prototype.hasOwnProperty;
var x = (e, t) => () => (t || e((t = { exports: {} }).exports, t), t.exports);
var G = (e, t) => {
  for (var r2 in t)
    E(e, r2, { get: t[r2], enumerable: true });
};
var S = (e, t, r2, u2) => {
  if (t && typeof t == "object" || typeof t == "function")
    for (let o of H(t))
      !Y.call(e, o) && o !== r2 && E(e, o, { get: () => t[o], enumerable: !(u2 = B(t, o)) || u2.enumerable });
  return e;
};
var y = (e, t, r2) => (S(e, t, "default"), r2 && S(r2, t, "default"));
var O = (e, t, r2) => (r2 = e != null ? z(W(e)) : {}, S(t || !e || !e.__esModule ? E(r2, "default", { value: e, enumerable: true }) : r2, e));
var U = x((n12) => {
  "use strict";
  var _ = Symbol.for("react.element"), J2 = Symbol.for("react.portal"), K = Symbol.for("react.fragment"), Q2 = Symbol.for("react.strict_mode"), X = Symbol.for("react.profiler"), Z3 = Symbol.for("react.provider"), ee2 = Symbol.for("react.context"), te3 = Symbol.for("react.forward_ref"), re2 = Symbol.for("react.suspense"), ne3 = Symbol.for("react.memo"), oe3 = Symbol.for("react.lazy"), j = Symbol.iterator;
  function ue2(e) {
    return e === null || typeof e != "object" ? null : (e = j && e[j] || e["@@iterator"], typeof e == "function" ? e : null);
  }
  var P2 = { isMounted: function() {
    return false;
  }, enqueueForceUpdate: function() {
  }, enqueueReplaceState: function() {
  }, enqueueSetState: function() {
  } }, T2 = Object.assign, D2 = {};
  function d3(e, t, r2) {
    this.props = e, this.context = t, this.refs = D2, this.updater = r2 || P2;
  }
  d3.prototype.isReactComponent = {};
  d3.prototype.setState = function(e, t) {
    if (typeof e != "object" && typeof e != "function" && e != null)
      throw Error("setState(...): takes an object of state variables to update or a function which returns an object of state variables.");
    this.updater.enqueueSetState(this, e, t, "setState");
  };
  d3.prototype.forceUpdate = function(e) {
    this.updater.enqueueForceUpdate(this, e, "forceUpdate");
  };
  function V() {
  }
  V.prototype = d3.prototype;
  function C2(e, t, r2) {
    this.props = e, this.context = t, this.refs = D2, this.updater = r2 || P2;
  }
  var k = C2.prototype = new V();
  k.constructor = C2;
  T2(k, d3.prototype);
  k.isPureReactComponent = true;
  var I = Array.isArray, L = Object.prototype.hasOwnProperty, w = { current: null }, N3 = { key: true, ref: true, __self: true, __source: true };
  function F2(e, t, r2) {
    var u2, o = {}, c = null, f3 = null;
    if (t != null)
      for (u2 in t.ref !== void 0 && (f3 = t.ref), t.key !== void 0 && (c = "" + t.key), t)
        L.call(t, u2) && !N3.hasOwnProperty(u2) && (o[u2] = t[u2]);
    var i = arguments.length - 2;
    if (i === 1)
      o.children = r2;
    else if (1 < i) {
      for (var s2 = Array(i), a2 = 0; a2 < i; a2++)
        s2[a2] = arguments[a2 + 2];
      o.children = s2;
    }
    if (e && e.defaultProps)
      for (u2 in i = e.defaultProps, i)
        o[u2] === void 0 && (o[u2] = i[u2]);
    return { $$typeof: _, type: e, key: c, ref: f3, props: o, _owner: w.current };
  }
  function se3(e, t) {
    return { $$typeof: _, type: e.type, key: t, ref: e.ref, props: e.props, _owner: e._owner };
  }
  function b(e) {
    return typeof e == "object" && e !== null && e.$$typeof === _;
  }
  function ce2(e) {
    var t = { "=": "=0", ":": "=2" };
    return "$" + e.replace(/[=:]/g, function(r2) {
      return t[r2];
    });
  }
  var g2 = /\/+/g;
  function R(e, t) {
    return typeof e == "object" && e !== null && e.key != null ? ce2("" + e.key) : t.toString(36);
  }
  function h2(e, t, r2, u2, o) {
    var c = typeof e;
    (c === "undefined" || c === "boolean") && (e = null);
    var f3 = false;
    if (e === null)
      f3 = true;
    else
      switch (c) {
        case "string":
        case "number":
          f3 = true;
          break;
        case "object":
          switch (e.$$typeof) {
            case _:
            case J2:
              f3 = true;
          }
      }
    if (f3)
      return f3 = e, o = o(f3), e = u2 === "" ? "." + R(f3, 0) : u2, I(o) ? (r2 = "", e != null && (r2 = e.replace(g2, "$&/") + "/"), h2(o, t, r2, "", function(a2) {
        return a2;
      })) : o != null && (b(o) && (o = se3(o, r2 + (!o.key || f3 && f3.key === o.key ? "" : ("" + o.key).replace(g2, "$&/") + "/") + e)), t.push(o)), 1;
    if (f3 = 0, u2 = u2 === "" ? "." : u2 + ":", I(e))
      for (var i = 0; i < e.length; i++) {
        c = e[i];
        var s2 = u2 + R(c, i);
        f3 += h2(c, t, r2, s2, o);
      }
    else if (s2 = ue2(e), typeof s2 == "function")
      for (e = s2.call(e), i = 0; !(c = e.next()).done; )
        c = c.value, s2 = u2 + R(c, i++), f3 += h2(c, t, r2, s2, o);
    else if (c === "object")
      throw t = String(e), Error("Objects are not valid as a React child (found: " + (t === "[object Object]" ? "object with keys {" + Object.keys(e).join(", ") + "}" : t) + "). If you meant to render a collection of children, use an array instead.");
    return f3;
  }
  function m3(e, t, r2) {
    if (e == null)
      return e;
    var u2 = [], o = 0;
    return h2(e, u2, "", "", function(c) {
      return t.call(r2, c, o++);
    }), u2;
  }
  function ie3(e) {
    if (e._status === -1) {
      var t = e._result;
      t = t(), t.then(function(r2) {
        (e._status === 0 || e._status === -1) && (e._status = 1, e._result = r2);
      }, function(r2) {
        (e._status === 0 || e._status === -1) && (e._status = 2, e._result = r2);
      }), e._status === -1 && (e._status = 0, e._result = t);
    }
    if (e._status === 1)
      return e._result.default;
    throw e._result;
  }
  var l2 = { current: null }, v3 = { transition: null }, fe3 = { ReactCurrentDispatcher: l2, ReactCurrentBatchConfig: v3, ReactCurrentOwner: w };
  n12.Children = { map: m3, forEach: function(e, t, r2) {
    m3(e, function() {
      t.apply(this, arguments);
    }, r2);
  }, count: function(e) {
    var t = 0;
    return m3(e, function() {
      t++;
    }), t;
  }, toArray: function(e) {
    return m3(e, function(t) {
      return t;
    }) || [];
  }, only: function(e) {
    if (!b(e))
      throw Error("React.Children.only expected to receive a single React element child.");
    return e;
  } };
  n12.Component = d3;
  n12.Fragment = K;
  n12.Profiler = X;
  n12.PureComponent = C2;
  n12.StrictMode = Q2;
  n12.Suspense = re2;
  n12.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED = fe3;
  n12.cloneElement = function(e, t, r2) {
    if (e == null)
      throw Error("React.cloneElement(...): The argument must be a React element, but you passed " + e + ".");
    var u2 = T2({}, e.props), o = e.key, c = e.ref, f3 = e._owner;
    if (t != null) {
      if (t.ref !== void 0 && (c = t.ref, f3 = w.current), t.key !== void 0 && (o = "" + t.key), e.type && e.type.defaultProps)
        var i = e.type.defaultProps;
      for (s2 in t)
        L.call(t, s2) && !N3.hasOwnProperty(s2) && (u2[s2] = t[s2] === void 0 && i !== void 0 ? i[s2] : t[s2]);
    }
    var s2 = arguments.length - 2;
    if (s2 === 1)
      u2.children = r2;
    else if (1 < s2) {
      i = Array(s2);
      for (var a2 = 0; a2 < s2; a2++)
        i[a2] = arguments[a2 + 2];
      u2.children = i;
    }
    return { $$typeof: _, type: e.type, key: o, ref: c, props: u2, _owner: f3 };
  };
  n12.createContext = function(e) {
    return e = { $$typeof: ee2, _currentValue: e, _currentValue2: e, _threadCount: 0, Provider: null, Consumer: null, _defaultValue: null, _globalName: null }, e.Provider = { $$typeof: Z3, _context: e }, e.Consumer = e;
  };
  n12.createElement = F2;
  n12.createFactory = function(e) {
    var t = F2.bind(null, e);
    return t.type = e, t;
  };
  n12.createRef = function() {
    return { current: null };
  };
  n12.forwardRef = function(e) {
    return { $$typeof: te3, render: e };
  };
  n12.isValidElement = b;
  n12.lazy = function(e) {
    return { $$typeof: oe3, _payload: { _status: -1, _result: e }, _init: ie3 };
  };
  n12.memo = function(e, t) {
    return { $$typeof: ne3, type: e, compare: t === void 0 ? null : t };
  };
  n12.startTransition = function(e) {
    var t = v3.transition;
    v3.transition = {};
    try {
      e();
    } finally {
      v3.transition = t;
    }
  };
  n12.unstable_act = function() {
    throw Error("act(...) is not supported in production builds of React.");
  };
  n12.useCallback = function(e, t) {
    return l2.current.useCallback(e, t);
  };
  n12.useContext = function(e) {
    return l2.current.useContext(e);
  };
  n12.useDebugValue = function() {
  };
  n12.useDeferredValue = function(e) {
    return l2.current.useDeferredValue(e);
  };
  n12.useEffect = function(e, t) {
    return l2.current.useEffect(e, t);
  };
  n12.useId = function() {
    return l2.current.useId();
  };
  n12.useImperativeHandle = function(e, t, r2) {
    return l2.current.useImperativeHandle(e, t, r2);
  };
  n12.useInsertionEffect = function(e, t) {
    return l2.current.useInsertionEffect(e, t);
  };
  n12.useLayoutEffect = function(e, t) {
    return l2.current.useLayoutEffect(e, t);
  };
  n12.useMemo = function(e, t) {
    return l2.current.useMemo(e, t);
  };
  n12.useReducer = function(e, t, r2) {
    return l2.current.useReducer(e, t, r2);
  };
  n12.useRef = function(e) {
    return l2.current.useRef(e);
  };
  n12.useState = function(e) {
    return l2.current.useState(e);
  };
  n12.useSyncExternalStore = function(e, t, r2) {
    return l2.current.useSyncExternalStore(e, t, r2);
  };
  n12.useTransition = function() {
    return l2.current.useTransition();
  };
  n12.version = "18.2.0";
});
var $ = x((Je2, q) => {
  "use strict";
  q.exports = U();
});
var p = {};
G(p, { Children: () => le, Component: () => ae, Fragment: () => pe, Profiler: () => ye, PureComponent: () => de, StrictMode: () => _e, Suspense: () => me, __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: () => he, cloneElement: () => ve, createContext: () => Se, createElement: () => Ee, createFactory: () => Re, createRef: () => Ce, default: () => We, forwardRef: () => ke, isValidElement: () => we, lazy: () => be, memo: () => $e, startTransition: () => xe, unstable_act: () => Oe, useCallback: () => je, useContext: () => Ie, useDebugValue: () => ge, useDeferredValue: () => Pe, useEffect: () => Te, useId: () => De, useImperativeHandle: () => Ve, useInsertionEffect: () => Le, useLayoutEffect: () => Ne, useMemo: () => Fe, useReducer: () => Ue, useRef: () => qe, useState: () => Ae, useSyncExternalStore: () => Me, useTransition: () => ze, version: () => Be });
var M = O($());
y(p, O($()));
var { Children: le, Component: ae, Fragment: pe, Profiler: ye, PureComponent: de, StrictMode: _e, Suspense: me, __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: he, cloneElement: ve, createContext: Se, createElement: Ee, createFactory: Re, createRef: Ce, forwardRef: ke, isValidElement: we, lazy: be, memo: $e, startTransition: xe, unstable_act: Oe, useCallback: je, useContext: Ie, useDebugValue: ge, useDeferredValue: Pe, useEffect: Te, useId: De, useImperativeHandle: Ve, useInsertionEffect: Le, useLayoutEffect: Ne, useMemo: Fe, useReducer: Ue, useRef: qe, useState: Ae, useSyncExternalStore: Me, useTransition: ze, version: Be } = M;
var { default: A, ...He } = M;
var We = A !== void 0 ? A : He;

// https://esm.sh/v135/scheduler@0.23.0/denonext/scheduler.mjs
var scheduler_exports = {};
__export(scheduler_exports, {
  default: () => Ee2,
  unstable_IdlePriority: () => oe,
  unstable_ImmediatePriority: () => se,
  unstable_LowPriority: () => ce,
  unstable_NormalPriority: () => fe,
  unstable_Profiling: () => be2,
  unstable_UserBlockingPriority: () => _e2,
  unstable_cancelCallback: () => de2,
  unstable_continueExecution: () => pe2,
  unstable_forceFrameRate: () => ve2,
  unstable_getCurrentPriorityLevel: () => ye2,
  unstable_getFirstCallbackNode: () => me2,
  unstable_next: () => ge2,
  unstable_now: () => ae2,
  unstable_pauseExecution: () => he2,
  unstable_requestPaint: () => ke2,
  unstable_runWithPriority: () => Pe2,
  unstable_scheduleCallback: () => we2,
  unstable_shouldYield: () => xe2,
  unstable_wrapCallback: () => Ie2
});
var __setImmediate$ = (cb, ...args) => setTimeout(cb, 0, ...args);
var ee = Object.create;
var T = Object.defineProperty;
var ne = Object.getOwnPropertyDescriptor;
var te = Object.getOwnPropertyNames;
var re = Object.getPrototypeOf;
var le2 = Object.prototype.hasOwnProperty;
var W2 = (e, n12) => () => (n12 || e((n12 = { exports: {} }).exports, n12), n12.exports);
var ie = (e, n12) => {
  for (var t in n12)
    T(e, t, { get: n12[t], enumerable: true });
};
var E2 = (e, n12, t, l2) => {
  if (n12 && typeof n12 == "object" || typeof n12 == "function")
    for (let i of te(n12))
      !le2.call(e, i) && i !== t && T(e, i, { get: () => n12[i], enumerable: !(l2 = ne(n12, i)) || l2.enumerable });
  return e;
};
var d = (e, n12, t) => (E2(e, n12, "default"), t && E2(t, n12, "default"));
var Y2 = (e, n12, t) => (t = e != null ? ee(re(e)) : {}, E2(n12 || !e || !e.__esModule ? T(t, "default", { value: e, enumerable: true }) : t, e));
var U2 = W2((r2) => {
  "use strict";
  function M2(e, n12) {
    var t = e.length;
    e.push(n12);
    e:
      for (; 0 < t; ) {
        var l2 = t - 1 >>> 1, i = e[l2];
        if (0 < k(i, n12))
          e[l2] = n12, e[t] = i, t = l2;
        else
          break e;
      }
  }
  function o(e) {
    return e.length === 0 ? null : e[0];
  }
  function w(e) {
    if (e.length === 0)
      return null;
    var n12 = e[0], t = e.pop();
    if (t !== n12) {
      e[0] = t;
      e:
        for (var l2 = 0, i = e.length, g2 = i >>> 1; l2 < g2; ) {
          var b = 2 * (l2 + 1) - 1, C2 = e[b], _ = b + 1, h2 = e[_];
          if (0 > k(C2, t))
            _ < i && 0 > k(h2, C2) ? (e[l2] = h2, e[_] = t, l2 = _) : (e[l2] = C2, e[b] = t, l2 = b);
          else if (_ < i && 0 > k(h2, t))
            e[l2] = h2, e[_] = t, l2 = _;
          else
            break e;
        }
    }
    return n12;
  }
  function k(e, n12) {
    var t = e.sortIndex - n12.sortIndex;
    return t !== 0 ? t : e.id - n12.id;
  }
  typeof performance == "object" && typeof performance.now == "function" ? (z3 = performance, r2.unstable_now = function() {
    return z3.now();
  }) : (L = Date, A3 = L.now(), r2.unstable_now = function() {
    return L.now() - A3;
  });
  var z3, L, A3, s2 = [], c = [], ue2 = 1, a2 = null, u2 = 3, x3 = false, p3 = false, y3 = false, J2 = typeof setTimeout == "function" ? setTimeout : null, K = typeof clearTimeout == "function" ? clearTimeout : null, G2 = typeof __setImmediate$ < "u" ? __setImmediate$ : null;
  typeof navigator < "u" && navigator.scheduling !== void 0 && navigator.scheduling.isInputPending !== void 0 && navigator.scheduling.isInputPending.bind(navigator.scheduling);
  function j(e) {
    for (var n12 = o(c); n12 !== null; ) {
      if (n12.callback === null)
        w(c);
      else if (n12.startTime <= e)
        w(c), n12.sortIndex = n12.expirationTime, M2(s2, n12);
      else
        break;
      n12 = o(c);
    }
  }
  function R(e) {
    if (y3 = false, j(e), !p3)
      if (o(s2) !== null)
        p3 = true, D2(B3);
      else {
        var n12 = o(c);
        n12 !== null && q(R, n12.startTime - e);
      }
  }
  function B3(e, n12) {
    p3 = false, y3 && (y3 = false, K(m3), m3 = -1), x3 = true;
    var t = u2;
    try {
      for (j(n12), a2 = o(s2); a2 !== null && (!(a2.expirationTime > n12) || e && !V()); ) {
        var l2 = a2.callback;
        if (typeof l2 == "function") {
          a2.callback = null, u2 = a2.priorityLevel;
          var i = l2(a2.expirationTime <= n12);
          n12 = r2.unstable_now(), typeof i == "function" ? a2.callback = i : a2 === o(s2) && w(s2), j(n12);
        } else
          w(s2);
        a2 = o(s2);
      }
      if (a2 !== null)
        var g2 = true;
      else {
        var b = o(c);
        b !== null && q(R, b.startTime - n12), g2 = false;
      }
      return g2;
    } finally {
      a2 = null, u2 = t, x3 = false;
    }
  }
  var I = false, P2 = null, m3 = -1, Q2 = 5, S2 = -1;
  function V() {
    return !(r2.unstable_now() - S2 < Q2);
  }
  function N3() {
    if (P2 !== null) {
      var e = r2.unstable_now();
      S2 = e;
      var n12 = true;
      try {
        n12 = P2(true, e);
      } finally {
        n12 ? v3() : (I = false, P2 = null);
      }
    } else
      I = false;
  }
  var v3;
  typeof G2 == "function" ? v3 = function() {
    G2(N3);
  } : typeof MessageChannel < "u" ? (F2 = new MessageChannel(), H3 = F2.port2, F2.port1.onmessage = N3, v3 = function() {
    H3.postMessage(null);
  }) : v3 = function() {
    J2(N3, 0);
  };
  var F2, H3;
  function D2(e) {
    P2 = e, I || (I = true, v3());
  }
  function q(e, n12) {
    m3 = J2(function() {
      e(r2.unstable_now());
    }, n12);
  }
  r2.unstable_IdlePriority = 5;
  r2.unstable_ImmediatePriority = 1;
  r2.unstable_LowPriority = 4;
  r2.unstable_NormalPriority = 3;
  r2.unstable_Profiling = null;
  r2.unstable_UserBlockingPriority = 2;
  r2.unstable_cancelCallback = function(e) {
    e.callback = null;
  };
  r2.unstable_continueExecution = function() {
    p3 || x3 || (p3 = true, D2(B3));
  };
  r2.unstable_forceFrameRate = function(e) {
    0 > e || 125 < e ? console.error("forceFrameRate takes a positive int between 0 and 125, forcing frame rates higher than 125 fps is not supported") : Q2 = 0 < e ? Math.floor(1e3 / e) : 5;
  };
  r2.unstable_getCurrentPriorityLevel = function() {
    return u2;
  };
  r2.unstable_getFirstCallbackNode = function() {
    return o(s2);
  };
  r2.unstable_next = function(e) {
    switch (u2) {
      case 1:
      case 2:
      case 3:
        var n12 = 3;
        break;
      default:
        n12 = u2;
    }
    var t = u2;
    u2 = n12;
    try {
      return e();
    } finally {
      u2 = t;
    }
  };
  r2.unstable_pauseExecution = function() {
  };
  r2.unstable_requestPaint = function() {
  };
  r2.unstable_runWithPriority = function(e, n12) {
    switch (e) {
      case 1:
      case 2:
      case 3:
      case 4:
      case 5:
        break;
      default:
        e = 3;
    }
    var t = u2;
    u2 = e;
    try {
      return n12();
    } finally {
      u2 = t;
    }
  };
  r2.unstable_scheduleCallback = function(e, n12, t) {
    var l2 = r2.unstable_now();
    switch (typeof t == "object" && t !== null ? (t = t.delay, t = typeof t == "number" && 0 < t ? l2 + t : l2) : t = l2, e) {
      case 1:
        var i = -1;
        break;
      case 2:
        i = 250;
        break;
      case 5:
        i = 1073741823;
        break;
      case 4:
        i = 1e4;
        break;
      default:
        i = 5e3;
    }
    return i = t + i, e = { id: ue2++, callback: n12, priorityLevel: e, startTime: t, expirationTime: i, sortIndex: -1 }, t > l2 ? (e.sortIndex = t, M2(c, e), o(s2) === null && e === o(c) && (y3 ? (K(m3), m3 = -1) : y3 = true, q(R, t - l2))) : (e.sortIndex = i, M2(s2, e), p3 || x3 || (p3 = true, D2(B3))), e;
  };
  r2.unstable_shouldYield = V;
  r2.unstable_wrapCallback = function(e) {
    var n12 = u2;
    return function() {
      var t = u2;
      u2 = n12;
      try {
        return e.apply(this, arguments);
      } finally {
        u2 = t;
      }
    };
  };
});
var O2 = W2((Ne3, X) => {
  "use strict";
  X.exports = U2();
});
var f = {};
ie(f, { default: () => Ee2, unstable_IdlePriority: () => oe, unstable_ImmediatePriority: () => se, unstable_LowPriority: () => ce, unstable_NormalPriority: () => fe, unstable_Profiling: () => be2, unstable_UserBlockingPriority: () => _e2, unstable_cancelCallback: () => de2, unstable_continueExecution: () => pe2, unstable_forceFrameRate: () => ve2, unstable_getCurrentPriorityLevel: () => ye2, unstable_getFirstCallbackNode: () => me2, unstable_next: () => ge2, unstable_now: () => ae2, unstable_pauseExecution: () => he2, unstable_requestPaint: () => ke2, unstable_runWithPriority: () => Pe2, unstable_scheduleCallback: () => we2, unstable_shouldYield: () => xe2, unstable_wrapCallback: () => Ie2 });
var $2 = Y2(O2());
d(f, Y2(O2()));
var { unstable_now: ae2, unstable_IdlePriority: oe, unstable_ImmediatePriority: se, unstable_LowPriority: ce, unstable_NormalPriority: fe, unstable_Profiling: be2, unstable_UserBlockingPriority: _e2, unstable_cancelCallback: de2, unstable_continueExecution: pe2, unstable_forceFrameRate: ve2, unstable_getCurrentPriorityLevel: ye2, unstable_getFirstCallbackNode: me2, unstable_next: ge2, unstable_pauseExecution: he2, unstable_requestPaint: ke2, unstable_runWithPriority: Pe2, unstable_scheduleCallback: we2, unstable_shouldYield: xe2, unstable_wrapCallback: Ie2 } = $2;
var { default: Z, ...Ce2 } = $2;
var Ee2 = Z !== void 0 ? Z : Ce2;

// https://esm.sh/v135/react-dom@18.2.0/denonext/react-dom.mjs
var require2 = (n12) => {
  const e = (m3) => typeof m3.default < "u" ? m3.default : m3, c = (m3) => Object.assign({}, m3);
  switch (n12) {
    case "react":
      return e(react_exports);
    case "scheduler":
      return e(scheduler_exports);
    default:
      throw new Error('module "' + n12 + '" not found');
  }
};
var Ca = Object.create;
var tl = Object.defineProperty;
var xa = Object.getOwnPropertyDescriptor;
var Na = Object.getOwnPropertyNames;
var _a = Object.getPrototypeOf;
var za = Object.prototype.hasOwnProperty;
var su = ((e) => typeof require2 < "u" ? require2 : typeof Proxy < "u" ? new Proxy(e, { get: (n12, t) => (typeof require2 < "u" ? require2 : n12)[t] }) : e)(function(e) {
  if (typeof require2 < "u")
    return require2.apply(this, arguments);
  throw Error('Dynamic require of "' + e + '" is not supported');
});
var au = (e, n12) => () => (n12 || e((n12 = { exports: {} }).exports, n12), n12.exports);
var Pa = (e, n12) => {
  for (var t in n12)
    tl(e, t, { get: n12[t], enumerable: true });
};
var nl = (e, n12, t, r2) => {
  if (n12 && typeof n12 == "object" || typeof n12 == "function")
    for (let l2 of Na(n12))
      !za.call(e, l2) && l2 !== t && tl(e, l2, { get: () => n12[l2], enumerable: !(r2 = xa(n12, l2)) || r2.enumerable });
  return e;
};
var an = (e, n12, t) => (nl(e, n12, "default"), t && nl(t, n12, "default"));
var cu = (e, n12, t) => (t = e != null ? Ca(_a(e)) : {}, nl(n12 || !e || !e.__esModule ? tl(t, "default", { value: e, enumerable: true }) : t, e));
var ya = au((fe3) => {
  "use strict";
  var go2 = su("react"), ae4 = su("scheduler");
  function v3(e) {
    for (var n12 = "https://reactjs.org/docs/error-decoder.html?invariant=" + e, t = 1; t < arguments.length; t++)
      n12 += "&args[]=" + encodeURIComponent(arguments[t]);
    return "Minified React error #" + e + "; visit " + n12 + " for the full message or use the non-minified dev environment for full errors and additional helpful warnings.";
  }
  var wo2 = /* @__PURE__ */ new Set(), St2 = {};
  function En2(e, n12) {
    Qn2(e, n12), Qn2(e + "Capture", n12);
  }
  function Qn2(e, n12) {
    for (St2[e] = n12, e = 0; e < n12.length; e++)
      wo2.add(n12[e]);
  }
  var Fe3 = !(typeof window > "u" || typeof window.document > "u" || typeof window.document.createElement > "u"), Nl2 = Object.prototype.hasOwnProperty, La2 = /^[:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD][:A-Z_a-z\u00C0-\u00D6\u00D8-\u00F6\u00F8-\u02FF\u0370-\u037D\u037F-\u1FFF\u200C-\u200D\u2070-\u218F\u2C00-\u2FEF\u3001-\uD7FF\uF900-\uFDCF\uFDF0-\uFFFD\-.0-9\u00B7\u0300-\u036F\u203F-\u2040]*$/, fu = {}, du = {};
  function Ta2(e) {
    return Nl2.call(du, e) ? true : Nl2.call(fu, e) ? false : La2.test(e) ? du[e] = true : (fu[e] = true, false);
  }
  function Ma2(e, n12, t, r2) {
    if (t !== null && t.type === 0)
      return false;
    switch (typeof n12) {
      case "function":
      case "symbol":
        return true;
      case "boolean":
        return r2 ? false : t !== null ? !t.acceptsBooleans : (e = e.toLowerCase().slice(0, 5), e !== "data-" && e !== "aria-");
      default:
        return false;
    }
  }
  function Da2(e, n12, t, r2) {
    if (n12 === null || typeof n12 > "u" || Ma2(e, n12, t, r2))
      return true;
    if (r2)
      return false;
    if (t !== null)
      switch (t.type) {
        case 3:
          return !n12;
        case 4:
          return n12 === false;
        case 5:
          return isNaN(n12);
        case 6:
          return isNaN(n12) || 1 > n12;
      }
    return false;
  }
  function ee2(e, n12, t, r2, l2, i, u2) {
    this.acceptsBooleans = n12 === 2 || n12 === 3 || n12 === 4, this.attributeName = r2, this.attributeNamespace = l2, this.mustUseProperty = t, this.propertyName = e, this.type = n12, this.sanitizeURL = i, this.removeEmptyString = u2;
  }
  var Y3 = {};
  "children dangerouslySetInnerHTML defaultValue defaultChecked innerHTML suppressContentEditableWarning suppressHydrationWarning style".split(" ").forEach(function(e) {
    Y3[e] = new ee2(e, 0, false, e, null, false, false);
  });
  [["acceptCharset", "accept-charset"], ["className", "class"], ["htmlFor", "for"], ["httpEquiv", "http-equiv"]].forEach(function(e) {
    var n12 = e[0];
    Y3[n12] = new ee2(n12, 1, false, e[1], null, false, false);
  });
  ["contentEditable", "draggable", "spellCheck", "value"].forEach(function(e) {
    Y3[e] = new ee2(e, 2, false, e.toLowerCase(), null, false, false);
  });
  ["autoReverse", "externalResourcesRequired", "focusable", "preserveAlpha"].forEach(function(e) {
    Y3[e] = new ee2(e, 2, false, e, null, false, false);
  });
  "allowFullScreen async autoFocus autoPlay controls default defer disabled disablePictureInPicture disableRemotePlayback formNoValidate hidden loop noModule noValidate open playsInline readOnly required reversed scoped seamless itemScope".split(" ").forEach(function(e) {
    Y3[e] = new ee2(e, 3, false, e.toLowerCase(), null, false, false);
  });
  ["checked", "multiple", "muted", "selected"].forEach(function(e) {
    Y3[e] = new ee2(e, 3, true, e, null, false, false);
  });
  ["capture", "download"].forEach(function(e) {
    Y3[e] = new ee2(e, 4, false, e, null, false, false);
  });
  ["cols", "rows", "size", "span"].forEach(function(e) {
    Y3[e] = new ee2(e, 6, false, e, null, false, false);
  });
  ["rowSpan", "start"].forEach(function(e) {
    Y3[e] = new ee2(e, 5, false, e.toLowerCase(), null, false, false);
  });
  var yi2 = /[\-:]([a-z])/g;
  function gi2(e) {
    return e[1].toUpperCase();
  }
  "accent-height alignment-baseline arabic-form baseline-shift cap-height clip-path clip-rule color-interpolation color-interpolation-filters color-profile color-rendering dominant-baseline enable-background fill-opacity fill-rule flood-color flood-opacity font-family font-size font-size-adjust font-stretch font-style font-variant font-weight glyph-name glyph-orientation-horizontal glyph-orientation-vertical horiz-adv-x horiz-origin-x image-rendering letter-spacing lighting-color marker-end marker-mid marker-start overline-position overline-thickness paint-order panose-1 pointer-events rendering-intent shape-rendering stop-color stop-opacity strikethrough-position strikethrough-thickness stroke-dasharray stroke-dashoffset stroke-linecap stroke-linejoin stroke-miterlimit stroke-opacity stroke-width text-anchor text-decoration text-rendering underline-position underline-thickness unicode-bidi unicode-range units-per-em v-alphabetic v-hanging v-ideographic v-mathematical vector-effect vert-adv-y vert-origin-x vert-origin-y word-spacing writing-mode xmlns:xlink x-height".split(" ").forEach(function(e) {
    var n12 = e.replace(yi2, gi2);
    Y3[n12] = new ee2(n12, 1, false, e, null, false, false);
  });
  "xlink:actuate xlink:arcrole xlink:role xlink:show xlink:title xlink:type".split(" ").forEach(function(e) {
    var n12 = e.replace(yi2, gi2);
    Y3[n12] = new ee2(n12, 1, false, e, "http://www.w3.org/1999/xlink", false, false);
  });
  ["xml:base", "xml:lang", "xml:space"].forEach(function(e) {
    var n12 = e.replace(yi2, gi2);
    Y3[n12] = new ee2(n12, 1, false, e, "http://www.w3.org/XML/1998/namespace", false, false);
  });
  ["tabIndex", "crossOrigin"].forEach(function(e) {
    Y3[e] = new ee2(e, 1, false, e.toLowerCase(), null, false, false);
  });
  Y3.xlinkHref = new ee2("xlinkHref", 1, false, "xlink:href", "http://www.w3.org/1999/xlink", true, false);
  ["src", "href", "action", "formAction"].forEach(function(e) {
    Y3[e] = new ee2(e, 1, false, e.toLowerCase(), null, true, true);
  });
  function wi2(e, n12, t, r2) {
    var l2 = Y3.hasOwnProperty(n12) ? Y3[n12] : null;
    (l2 !== null ? l2.type !== 0 : r2 || !(2 < n12.length) || n12[0] !== "o" && n12[0] !== "O" || n12[1] !== "n" && n12[1] !== "N") && (Da2(n12, t, l2, r2) && (t = null), r2 || l2 === null ? Ta2(n12) && (t === null ? e.removeAttribute(n12) : e.setAttribute(n12, "" + t)) : l2.mustUseProperty ? e[l2.propertyName] = t === null ? l2.type === 3 ? false : "" : t : (n12 = l2.attributeName, r2 = l2.attributeNamespace, t === null ? e.removeAttribute(n12) : (l2 = l2.type, t = l2 === 3 || l2 === 4 && t === true ? "" : "" + t, r2 ? e.setAttributeNS(r2, n12, t) : e.setAttribute(n12, t))));
  }
  var Ve3 = go2.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED, Bt2 = Symbol.for("react.element"), _n2 = Symbol.for("react.portal"), zn2 = Symbol.for("react.fragment"), Si2 = Symbol.for("react.strict_mode"), _l2 = Symbol.for("react.profiler"), So2 = Symbol.for("react.provider"), ko2 = Symbol.for("react.context"), ki2 = Symbol.for("react.forward_ref"), zl2 = Symbol.for("react.suspense"), Pl2 = Symbol.for("react.suspense_list"), Ei2 = Symbol.for("react.memo"), He3 = Symbol.for("react.lazy");
  Symbol.for("react.scope");
  Symbol.for("react.debug_trace_mode");
  var Eo2 = Symbol.for("react.offscreen");
  Symbol.for("react.legacy_hidden");
  Symbol.for("react.cache");
  Symbol.for("react.tracing_marker");
  var pu = Symbol.iterator;
  function bn2(e) {
    return e === null || typeof e != "object" ? null : (e = pu && e[pu] || e["@@iterator"], typeof e == "function" ? e : null);
  }
  var F2 = Object.assign, rl2;
  function ot2(e) {
    if (rl2 === void 0)
      try {
        throw Error();
      } catch (t) {
        var n12 = t.stack.trim().match(/\n( *(at )?)/);
        rl2 = n12 && n12[1] || "";
      }
    return `
` + rl2 + e;
  }
  var ll2 = false;
  function il2(e, n12) {
    if (!e || ll2)
      return "";
    ll2 = true;
    var t = Error.prepareStackTrace;
    Error.prepareStackTrace = void 0;
    try {
      if (n12)
        if (n12 = function() {
          throw Error();
        }, Object.defineProperty(n12.prototype, "props", { set: function() {
          throw Error();
        } }), typeof Reflect == "object" && Reflect.construct) {
          try {
            Reflect.construct(n12, []);
          } catch (d3) {
            var r2 = d3;
          }
          Reflect.construct(e, [], n12);
        } else {
          try {
            n12.call();
          } catch (d3) {
            r2 = d3;
          }
          e.call(n12.prototype);
        }
      else {
        try {
          throw Error();
        } catch (d3) {
          r2 = d3;
        }
        e();
      }
    } catch (d3) {
      if (d3 && r2 && typeof d3.stack == "string") {
        for (var l2 = d3.stack.split(`
`), i = r2.stack.split(`
`), u2 = l2.length - 1, o = i.length - 1; 1 <= u2 && 0 <= o && l2[u2] !== i[o]; )
          o--;
        for (; 1 <= u2 && 0 <= o; u2--, o--)
          if (l2[u2] !== i[o]) {
            if (u2 !== 1 || o !== 1)
              do
                if (u2--, o--, 0 > o || l2[u2] !== i[o]) {
                  var s2 = `
` + l2[u2].replace(" at new ", " at ");
                  return e.displayName && s2.includes("<anonymous>") && (s2 = s2.replace("<anonymous>", e.displayName)), s2;
                }
              while (1 <= u2 && 0 <= o);
            break;
          }
      }
    } finally {
      ll2 = false, Error.prepareStackTrace = t;
    }
    return (e = e ? e.displayName || e.name : "") ? ot2(e) : "";
  }
  function Oa2(e) {
    switch (e.tag) {
      case 5:
        return ot2(e.type);
      case 16:
        return ot2("Lazy");
      case 13:
        return ot2("Suspense");
      case 19:
        return ot2("SuspenseList");
      case 0:
      case 2:
      case 15:
        return e = il2(e.type, false), e;
      case 11:
        return e = il2(e.type.render, false), e;
      case 1:
        return e = il2(e.type, true), e;
      default:
        return "";
    }
  }
  function Ll2(e) {
    if (e == null)
      return null;
    if (typeof e == "function")
      return e.displayName || e.name || null;
    if (typeof e == "string")
      return e;
    switch (e) {
      case zn2:
        return "Fragment";
      case _n2:
        return "Portal";
      case _l2:
        return "Profiler";
      case Si2:
        return "StrictMode";
      case zl2:
        return "Suspense";
      case Pl2:
        return "SuspenseList";
    }
    if (typeof e == "object")
      switch (e.$$typeof) {
        case ko2:
          return (e.displayName || "Context") + ".Consumer";
        case So2:
          return (e._context.displayName || "Context") + ".Provider";
        case ki2:
          var n12 = e.render;
          return e = e.displayName, e || (e = n12.displayName || n12.name || "", e = e !== "" ? "ForwardRef(" + e + ")" : "ForwardRef"), e;
        case Ei2:
          return n12 = e.displayName || null, n12 !== null ? n12 : Ll2(e.type) || "Memo";
        case He3:
          n12 = e._payload, e = e._init;
          try {
            return Ll2(e(n12));
          } catch {
          }
      }
    return null;
  }
  function Ra2(e) {
    var n12 = e.type;
    switch (e.tag) {
      case 24:
        return "Cache";
      case 9:
        return (n12.displayName || "Context") + ".Consumer";
      case 10:
        return (n12._context.displayName || "Context") + ".Provider";
      case 18:
        return "DehydratedFragment";
      case 11:
        return e = n12.render, e = e.displayName || e.name || "", n12.displayName || (e !== "" ? "ForwardRef(" + e + ")" : "ForwardRef");
      case 7:
        return "Fragment";
      case 5:
        return n12;
      case 4:
        return "Portal";
      case 3:
        return "Root";
      case 6:
        return "Text";
      case 16:
        return Ll2(n12);
      case 8:
        return n12 === Si2 ? "StrictMode" : "Mode";
      case 22:
        return "Offscreen";
      case 12:
        return "Profiler";
      case 21:
        return "Scope";
      case 13:
        return "Suspense";
      case 19:
        return "SuspenseList";
      case 25:
        return "TracingMarker";
      case 1:
      case 0:
      case 17:
      case 2:
      case 14:
      case 15:
        if (typeof n12 == "function")
          return n12.displayName || n12.name || null;
        if (typeof n12 == "string")
          return n12;
    }
    return null;
  }
  function tn2(e) {
    switch (typeof e) {
      case "boolean":
      case "number":
      case "string":
      case "undefined":
        return e;
      case "object":
        return e;
      default:
        return "";
    }
  }
  function Co2(e) {
    var n12 = e.type;
    return (e = e.nodeName) && e.toLowerCase() === "input" && (n12 === "checkbox" || n12 === "radio");
  }
  function Fa2(e) {
    var n12 = Co2(e) ? "checked" : "value", t = Object.getOwnPropertyDescriptor(e.constructor.prototype, n12), r2 = "" + e[n12];
    if (!e.hasOwnProperty(n12) && typeof t < "u" && typeof t.get == "function" && typeof t.set == "function") {
      var l2 = t.get, i = t.set;
      return Object.defineProperty(e, n12, { configurable: true, get: function() {
        return l2.call(this);
      }, set: function(u2) {
        r2 = "" + u2, i.call(this, u2);
      } }), Object.defineProperty(e, n12, { enumerable: t.enumerable }), { getValue: function() {
        return r2;
      }, setValue: function(u2) {
        r2 = "" + u2;
      }, stopTracking: function() {
        e._valueTracker = null, delete e[n12];
      } };
    }
  }
  function Ht2(e) {
    e._valueTracker || (e._valueTracker = Fa2(e));
  }
  function xo2(e) {
    if (!e)
      return false;
    var n12 = e._valueTracker;
    if (!n12)
      return true;
    var t = n12.getValue(), r2 = "";
    return e && (r2 = Co2(e) ? e.checked ? "true" : "false" : e.value), e = r2, e !== t ? (n12.setValue(e), true) : false;
  }
  function vr2(e) {
    if (e = e || (typeof document < "u" ? document : void 0), typeof e > "u")
      return null;
    try {
      return e.activeElement || e.body;
    } catch {
      return e.body;
    }
  }
  function Tl2(e, n12) {
    var t = n12.checked;
    return F2({}, n12, { defaultChecked: void 0, defaultValue: void 0, value: void 0, checked: t ?? e._wrapperState.initialChecked });
  }
  function mu(e, n12) {
    var t = n12.defaultValue == null ? "" : n12.defaultValue, r2 = n12.checked != null ? n12.checked : n12.defaultChecked;
    t = tn2(n12.value != null ? n12.value : t), e._wrapperState = { initialChecked: r2, initialValue: t, controlled: n12.type === "checkbox" || n12.type === "radio" ? n12.checked != null : n12.value != null };
  }
  function No2(e, n12) {
    n12 = n12.checked, n12 != null && wi2(e, "checked", n12, false);
  }
  function Ml2(e, n12) {
    No2(e, n12);
    var t = tn2(n12.value), r2 = n12.type;
    if (t != null)
      r2 === "number" ? (t === 0 && e.value === "" || e.value != t) && (e.value = "" + t) : e.value !== "" + t && (e.value = "" + t);
    else if (r2 === "submit" || r2 === "reset") {
      e.removeAttribute("value");
      return;
    }
    n12.hasOwnProperty("value") ? Dl2(e, n12.type, t) : n12.hasOwnProperty("defaultValue") && Dl2(e, n12.type, tn2(n12.defaultValue)), n12.checked == null && n12.defaultChecked != null && (e.defaultChecked = !!n12.defaultChecked);
  }
  function hu(e, n12, t) {
    if (n12.hasOwnProperty("value") || n12.hasOwnProperty("defaultValue")) {
      var r2 = n12.type;
      if (!(r2 !== "submit" && r2 !== "reset" || n12.value !== void 0 && n12.value !== null))
        return;
      n12 = "" + e._wrapperState.initialValue, t || n12 === e.value || (e.value = n12), e.defaultValue = n12;
    }
    t = e.name, t !== "" && (e.name = ""), e.defaultChecked = !!e._wrapperState.initialChecked, t !== "" && (e.name = t);
  }
  function Dl2(e, n12, t) {
    (n12 !== "number" || vr2(e.ownerDocument) !== e) && (t == null ? e.defaultValue = "" + e._wrapperState.initialValue : e.defaultValue !== "" + t && (e.defaultValue = "" + t));
  }
  var st2 = Array.isArray;
  function jn2(e, n12, t, r2) {
    if (e = e.options, n12) {
      n12 = {};
      for (var l2 = 0; l2 < t.length; l2++)
        n12["$" + t[l2]] = true;
      for (t = 0; t < e.length; t++)
        l2 = n12.hasOwnProperty("$" + e[t].value), e[t].selected !== l2 && (e[t].selected = l2), l2 && r2 && (e[t].defaultSelected = true);
    } else {
      for (t = "" + tn2(t), n12 = null, l2 = 0; l2 < e.length; l2++) {
        if (e[l2].value === t) {
          e[l2].selected = true, r2 && (e[l2].defaultSelected = true);
          return;
        }
        n12 !== null || e[l2].disabled || (n12 = e[l2]);
      }
      n12 !== null && (n12.selected = true);
    }
  }
  function Ol2(e, n12) {
    if (n12.dangerouslySetInnerHTML != null)
      throw Error(v3(91));
    return F2({}, n12, { value: void 0, defaultValue: void 0, children: "" + e._wrapperState.initialValue });
  }
  function vu(e, n12) {
    var t = n12.value;
    if (t == null) {
      if (t = n12.children, n12 = n12.defaultValue, t != null) {
        if (n12 != null)
          throw Error(v3(92));
        if (st2(t)) {
          if (1 < t.length)
            throw Error(v3(93));
          t = t[0];
        }
        n12 = t;
      }
      n12 == null && (n12 = ""), t = n12;
    }
    e._wrapperState = { initialValue: tn2(t) };
  }
  function _o2(e, n12) {
    var t = tn2(n12.value), r2 = tn2(n12.defaultValue);
    t != null && (t = "" + t, t !== e.value && (e.value = t), n12.defaultValue == null && e.defaultValue !== t && (e.defaultValue = t)), r2 != null && (e.defaultValue = "" + r2);
  }
  function yu(e) {
    var n12 = e.textContent;
    n12 === e._wrapperState.initialValue && n12 !== "" && n12 !== null && (e.value = n12);
  }
  function zo2(e) {
    switch (e) {
      case "svg":
        return "http://www.w3.org/2000/svg";
      case "math":
        return "http://www.w3.org/1998/Math/MathML";
      default:
        return "http://www.w3.org/1999/xhtml";
    }
  }
  function Rl2(e, n12) {
    return e == null || e === "http://www.w3.org/1999/xhtml" ? zo2(n12) : e === "http://www.w3.org/2000/svg" && n12 === "foreignObject" ? "http://www.w3.org/1999/xhtml" : e;
  }
  var Wt2, Po2 = function(e) {
    return typeof MSApp < "u" && MSApp.execUnsafeLocalFunction ? function(n12, t, r2, l2) {
      MSApp.execUnsafeLocalFunction(function() {
        return e(n12, t, r2, l2);
      });
    } : e;
  }(function(e, n12) {
    if (e.namespaceURI !== "http://www.w3.org/2000/svg" || "innerHTML" in e)
      e.innerHTML = n12;
    else {
      for (Wt2 = Wt2 || document.createElement("div"), Wt2.innerHTML = "<svg>" + n12.valueOf().toString() + "</svg>", n12 = Wt2.firstChild; e.firstChild; )
        e.removeChild(e.firstChild);
      for (; n12.firstChild; )
        e.appendChild(n12.firstChild);
    }
  });
  function kt2(e, n12) {
    if (n12) {
      var t = e.firstChild;
      if (t && t === e.lastChild && t.nodeType === 3) {
        t.nodeValue = n12;
        return;
      }
    }
    e.textContent = n12;
  }
  var ft2 = { animationIterationCount: true, aspectRatio: true, borderImageOutset: true, borderImageSlice: true, borderImageWidth: true, boxFlex: true, boxFlexGroup: true, boxOrdinalGroup: true, columnCount: true, columns: true, flex: true, flexGrow: true, flexPositive: true, flexShrink: true, flexNegative: true, flexOrder: true, gridArea: true, gridRow: true, gridRowEnd: true, gridRowSpan: true, gridRowStart: true, gridColumn: true, gridColumnEnd: true, gridColumnSpan: true, gridColumnStart: true, fontWeight: true, lineClamp: true, lineHeight: true, opacity: true, order: true, orphans: true, tabSize: true, widows: true, zIndex: true, zoom: true, fillOpacity: true, floodOpacity: true, stopOpacity: true, strokeDasharray: true, strokeDashoffset: true, strokeMiterlimit: true, strokeOpacity: true, strokeWidth: true }, Ia2 = ["Webkit", "ms", "Moz", "O"];
  Object.keys(ft2).forEach(function(e) {
    Ia2.forEach(function(n12) {
      n12 = n12 + e.charAt(0).toUpperCase() + e.substring(1), ft2[n12] = ft2[e];
    });
  });
  function Lo2(e, n12, t) {
    return n12 == null || typeof n12 == "boolean" || n12 === "" ? "" : t || typeof n12 != "number" || n12 === 0 || ft2.hasOwnProperty(e) && ft2[e] ? ("" + n12).trim() : n12 + "px";
  }
  function To2(e, n12) {
    e = e.style;
    for (var t in n12)
      if (n12.hasOwnProperty(t)) {
        var r2 = t.indexOf("--") === 0, l2 = Lo2(t, n12[t], r2);
        t === "float" && (t = "cssFloat"), r2 ? e.setProperty(t, l2) : e[t] = l2;
      }
  }
  var Ua2 = F2({ menuitem: true }, { area: true, base: true, br: true, col: true, embed: true, hr: true, img: true, input: true, keygen: true, link: true, meta: true, param: true, source: true, track: true, wbr: true });
  function Fl2(e, n12) {
    if (n12) {
      if (Ua2[e] && (n12.children != null || n12.dangerouslySetInnerHTML != null))
        throw Error(v3(137, e));
      if (n12.dangerouslySetInnerHTML != null) {
        if (n12.children != null)
          throw Error(v3(60));
        if (typeof n12.dangerouslySetInnerHTML != "object" || !("__html" in n12.dangerouslySetInnerHTML))
          throw Error(v3(61));
      }
      if (n12.style != null && typeof n12.style != "object")
        throw Error(v3(62));
    }
  }
  function Il2(e, n12) {
    if (e.indexOf("-") === -1)
      return typeof n12.is == "string";
    switch (e) {
      case "annotation-xml":
      case "color-profile":
      case "font-face":
      case "font-face-src":
      case "font-face-uri":
      case "font-face-format":
      case "font-face-name":
      case "missing-glyph":
        return false;
      default:
        return true;
    }
  }
  var Ul2 = null;
  function Ci2(e) {
    return e = e.target || e.srcElement || window, e.correspondingUseElement && (e = e.correspondingUseElement), e.nodeType === 3 ? e.parentNode : e;
  }
  var jl2 = null, Vn2 = null, An2 = null;
  function gu(e) {
    if (e = Vt2(e)) {
      if (typeof jl2 != "function")
        throw Error(v3(280));
      var n12 = e.stateNode;
      n12 && (n12 = Qr2(n12), jl2(e.stateNode, e.type, n12));
    }
  }
  function Mo2(e) {
    Vn2 ? An2 ? An2.push(e) : An2 = [e] : Vn2 = e;
  }
  function Do2() {
    if (Vn2) {
      var e = Vn2, n12 = An2;
      if (An2 = Vn2 = null, gu(e), n12)
        for (e = 0; e < n12.length; e++)
          gu(n12[e]);
    }
  }
  function Oo2(e, n12) {
    return e(n12);
  }
  function Ro2() {
  }
  var ul2 = false;
  function Fo2(e, n12, t) {
    if (ul2)
      return e(n12, t);
    ul2 = true;
    try {
      return Oo2(e, n12, t);
    } finally {
      ul2 = false, (Vn2 !== null || An2 !== null) && (Ro2(), Do2());
    }
  }
  function Et2(e, n12) {
    var t = e.stateNode;
    if (t === null)
      return null;
    var r2 = Qr2(t);
    if (r2 === null)
      return null;
    t = r2[n12];
    e:
      switch (n12) {
        case "onClick":
        case "onClickCapture":
        case "onDoubleClick":
        case "onDoubleClickCapture":
        case "onMouseDown":
        case "onMouseDownCapture":
        case "onMouseMove":
        case "onMouseMoveCapture":
        case "onMouseUp":
        case "onMouseUpCapture":
        case "onMouseEnter":
          (r2 = !r2.disabled) || (e = e.type, r2 = !(e === "button" || e === "input" || e === "select" || e === "textarea")), e = !r2;
          break e;
        default:
          e = false;
      }
    if (e)
      return null;
    if (t && typeof t != "function")
      throw Error(v3(231, n12, typeof t));
    return t;
  }
  var Vl2 = false;
  if (Fe3)
    try {
      xn2 = {}, Object.defineProperty(xn2, "passive", { get: function() {
        Vl2 = true;
      } }), window.addEventListener("test", xn2, xn2), window.removeEventListener("test", xn2, xn2);
    } catch {
      Vl2 = false;
    }
  var xn2;
  function ja2(e, n12, t, r2, l2, i, u2, o, s2) {
    var d3 = Array.prototype.slice.call(arguments, 3);
    try {
      n12.apply(t, d3);
    } catch (m3) {
      this.onError(m3);
    }
  }
  var dt2 = false, yr2 = null, gr2 = false, Al2 = null, Va2 = { onError: function(e) {
    dt2 = true, yr2 = e;
  } };
  function Aa2(e, n12, t, r2, l2, i, u2, o, s2) {
    dt2 = false, yr2 = null, ja2.apply(Va2, arguments);
  }
  function Ba2(e, n12, t, r2, l2, i, u2, o, s2) {
    if (Aa2.apply(this, arguments), dt2) {
      if (dt2) {
        var d3 = yr2;
        dt2 = false, yr2 = null;
      } else
        throw Error(v3(198));
      gr2 || (gr2 = true, Al2 = d3);
    }
  }
  function Cn2(e) {
    var n12 = e, t = e;
    if (e.alternate)
      for (; n12.return; )
        n12 = n12.return;
    else {
      e = n12;
      do
        n12 = e, n12.flags & 4098 && (t = n12.return), e = n12.return;
      while (e);
    }
    return n12.tag === 3 ? t : null;
  }
  function Io2(e) {
    if (e.tag === 13) {
      var n12 = e.memoizedState;
      if (n12 === null && (e = e.alternate, e !== null && (n12 = e.memoizedState)), n12 !== null)
        return n12.dehydrated;
    }
    return null;
  }
  function wu(e) {
    if (Cn2(e) !== e)
      throw Error(v3(188));
  }
  function Ha2(e) {
    var n12 = e.alternate;
    if (!n12) {
      if (n12 = Cn2(e), n12 === null)
        throw Error(v3(188));
      return n12 !== e ? null : e;
    }
    for (var t = e, r2 = n12; ; ) {
      var l2 = t.return;
      if (l2 === null)
        break;
      var i = l2.alternate;
      if (i === null) {
        if (r2 = l2.return, r2 !== null) {
          t = r2;
          continue;
        }
        break;
      }
      if (l2.child === i.child) {
        for (i = l2.child; i; ) {
          if (i === t)
            return wu(l2), e;
          if (i === r2)
            return wu(l2), n12;
          i = i.sibling;
        }
        throw Error(v3(188));
      }
      if (t.return !== r2.return)
        t = l2, r2 = i;
      else {
        for (var u2 = false, o = l2.child; o; ) {
          if (o === t) {
            u2 = true, t = l2, r2 = i;
            break;
          }
          if (o === r2) {
            u2 = true, r2 = l2, t = i;
            break;
          }
          o = o.sibling;
        }
        if (!u2) {
          for (o = i.child; o; ) {
            if (o === t) {
              u2 = true, t = i, r2 = l2;
              break;
            }
            if (o === r2) {
              u2 = true, r2 = i, t = l2;
              break;
            }
            o = o.sibling;
          }
          if (!u2)
            throw Error(v3(189));
        }
      }
      if (t.alternate !== r2)
        throw Error(v3(190));
    }
    if (t.tag !== 3)
      throw Error(v3(188));
    return t.stateNode.current === t ? e : n12;
  }
  function Uo2(e) {
    return e = Ha2(e), e !== null ? jo2(e) : null;
  }
  function jo2(e) {
    if (e.tag === 5 || e.tag === 6)
      return e;
    for (e = e.child; e !== null; ) {
      var n12 = jo2(e);
      if (n12 !== null)
        return n12;
      e = e.sibling;
    }
    return null;
  }
  var Vo2 = ae4.unstable_scheduleCallback, Su = ae4.unstable_cancelCallback, Wa2 = ae4.unstable_shouldYield, Qa2 = ae4.unstable_requestPaint, j = ae4.unstable_now, $a2 = ae4.unstable_getCurrentPriorityLevel, xi2 = ae4.unstable_ImmediatePriority, Ao2 = ae4.unstable_UserBlockingPriority, wr2 = ae4.unstable_NormalPriority, Ka2 = ae4.unstable_LowPriority, Bo2 = ae4.unstable_IdlePriority, Ar2 = null, Pe4 = null;
  function Ya2(e) {
    if (Pe4 && typeof Pe4.onCommitFiberRoot == "function")
      try {
        Pe4.onCommitFiberRoot(Ar2, e, void 0, (e.current.flags & 128) === 128);
      } catch {
      }
  }
  var Ee4 = Math.clz32 ? Math.clz32 : Za2, Xa2 = Math.log, Ga2 = Math.LN2;
  function Za2(e) {
    return e >>>= 0, e === 0 ? 32 : 31 - (Xa2(e) / Ga2 | 0) | 0;
  }
  var Qt2 = 64, $t2 = 4194304;
  function at2(e) {
    switch (e & -e) {
      case 1:
        return 1;
      case 2:
        return 2;
      case 4:
        return 4;
      case 8:
        return 8;
      case 16:
        return 16;
      case 32:
        return 32;
      case 64:
      case 128:
      case 256:
      case 512:
      case 1024:
      case 2048:
      case 4096:
      case 8192:
      case 16384:
      case 32768:
      case 65536:
      case 131072:
      case 262144:
      case 524288:
      case 1048576:
      case 2097152:
        return e & 4194240;
      case 4194304:
      case 8388608:
      case 16777216:
      case 33554432:
      case 67108864:
        return e & 130023424;
      case 134217728:
        return 134217728;
      case 268435456:
        return 268435456;
      case 536870912:
        return 536870912;
      case 1073741824:
        return 1073741824;
      default:
        return e;
    }
  }
  function Sr2(e, n12) {
    var t = e.pendingLanes;
    if (t === 0)
      return 0;
    var r2 = 0, l2 = e.suspendedLanes, i = e.pingedLanes, u2 = t & 268435455;
    if (u2 !== 0) {
      var o = u2 & ~l2;
      o !== 0 ? r2 = at2(o) : (i &= u2, i !== 0 && (r2 = at2(i)));
    } else
      u2 = t & ~l2, u2 !== 0 ? r2 = at2(u2) : i !== 0 && (r2 = at2(i));
    if (r2 === 0)
      return 0;
    if (n12 !== 0 && n12 !== r2 && !(n12 & l2) && (l2 = r2 & -r2, i = n12 & -n12, l2 >= i || l2 === 16 && (i & 4194240) !== 0))
      return n12;
    if (r2 & 4 && (r2 |= t & 16), n12 = e.entangledLanes, n12 !== 0)
      for (e = e.entanglements, n12 &= r2; 0 < n12; )
        t = 31 - Ee4(n12), l2 = 1 << t, r2 |= e[t], n12 &= ~l2;
    return r2;
  }
  function Ja2(e, n12) {
    switch (e) {
      case 1:
      case 2:
      case 4:
        return n12 + 250;
      case 8:
      case 16:
      case 32:
      case 64:
      case 128:
      case 256:
      case 512:
      case 1024:
      case 2048:
      case 4096:
      case 8192:
      case 16384:
      case 32768:
      case 65536:
      case 131072:
      case 262144:
      case 524288:
      case 1048576:
      case 2097152:
        return n12 + 5e3;
      case 4194304:
      case 8388608:
      case 16777216:
      case 33554432:
      case 67108864:
        return -1;
      case 134217728:
      case 268435456:
      case 536870912:
      case 1073741824:
        return -1;
      default:
        return -1;
    }
  }
  function qa2(e, n12) {
    for (var t = e.suspendedLanes, r2 = e.pingedLanes, l2 = e.expirationTimes, i = e.pendingLanes; 0 < i; ) {
      var u2 = 31 - Ee4(i), o = 1 << u2, s2 = l2[u2];
      s2 === -1 ? (!(o & t) || o & r2) && (l2[u2] = Ja2(o, n12)) : s2 <= n12 && (e.expiredLanes |= o), i &= ~o;
    }
  }
  function Bl2(e) {
    return e = e.pendingLanes & -1073741825, e !== 0 ? e : e & 1073741824 ? 1073741824 : 0;
  }
  function Ho2() {
    var e = Qt2;
    return Qt2 <<= 1, !(Qt2 & 4194240) && (Qt2 = 64), e;
  }
  function ol2(e) {
    for (var n12 = [], t = 0; 31 > t; t++)
      n12.push(e);
    return n12;
  }
  function Ut2(e, n12, t) {
    e.pendingLanes |= n12, n12 !== 536870912 && (e.suspendedLanes = 0, e.pingedLanes = 0), e = e.eventTimes, n12 = 31 - Ee4(n12), e[n12] = t;
  }
  function ba2(e, n12) {
    var t = e.pendingLanes & ~n12;
    e.pendingLanes = n12, e.suspendedLanes = 0, e.pingedLanes = 0, e.expiredLanes &= n12, e.mutableReadLanes &= n12, e.entangledLanes &= n12, n12 = e.entanglements;
    var r2 = e.eventTimes;
    for (e = e.expirationTimes; 0 < t; ) {
      var l2 = 31 - Ee4(t), i = 1 << l2;
      n12[l2] = 0, r2[l2] = -1, e[l2] = -1, t &= ~i;
    }
  }
  function Ni2(e, n12) {
    var t = e.entangledLanes |= n12;
    for (e = e.entanglements; t; ) {
      var r2 = 31 - Ee4(t), l2 = 1 << r2;
      l2 & n12 | e[r2] & n12 && (e[r2] |= n12), t &= ~l2;
    }
  }
  var P2 = 0;
  function Wo2(e) {
    return e &= -e, 1 < e ? 4 < e ? e & 268435455 ? 16 : 536870912 : 4 : 1;
  }
  var Qo2, _i2, $o2, Ko2, Yo2, Hl2 = false, Kt2 = [], Xe2 = null, Ge2 = null, Ze2 = null, Ct2 = /* @__PURE__ */ new Map(), xt2 = /* @__PURE__ */ new Map(), Qe = [], ec2 = "mousedown mouseup touchcancel touchend touchstart auxclick dblclick pointercancel pointerdown pointerup dragend dragstart drop compositionend compositionstart keydown keypress keyup input textInput copy cut paste click change contextmenu reset submit".split(" ");
  function ku(e, n12) {
    switch (e) {
      case "focusin":
      case "focusout":
        Xe2 = null;
        break;
      case "dragenter":
      case "dragleave":
        Ge2 = null;
        break;
      case "mouseover":
      case "mouseout":
        Ze2 = null;
        break;
      case "pointerover":
      case "pointerout":
        Ct2.delete(n12.pointerId);
        break;
      case "gotpointercapture":
      case "lostpointercapture":
        xt2.delete(n12.pointerId);
    }
  }
  function et2(e, n12, t, r2, l2, i) {
    return e === null || e.nativeEvent !== i ? (e = { blockedOn: n12, domEventName: t, eventSystemFlags: r2, nativeEvent: i, targetContainers: [l2] }, n12 !== null && (n12 = Vt2(n12), n12 !== null && _i2(n12)), e) : (e.eventSystemFlags |= r2, n12 = e.targetContainers, l2 !== null && n12.indexOf(l2) === -1 && n12.push(l2), e);
  }
  function nc2(e, n12, t, r2, l2) {
    switch (n12) {
      case "focusin":
        return Xe2 = et2(Xe2, e, n12, t, r2, l2), true;
      case "dragenter":
        return Ge2 = et2(Ge2, e, n12, t, r2, l2), true;
      case "mouseover":
        return Ze2 = et2(Ze2, e, n12, t, r2, l2), true;
      case "pointerover":
        var i = l2.pointerId;
        return Ct2.set(i, et2(Ct2.get(i) || null, e, n12, t, r2, l2)), true;
      case "gotpointercapture":
        return i = l2.pointerId, xt2.set(i, et2(xt2.get(i) || null, e, n12, t, r2, l2)), true;
    }
    return false;
  }
  function Xo2(e) {
    var n12 = dn2(e.target);
    if (n12 !== null) {
      var t = Cn2(n12);
      if (t !== null) {
        if (n12 = t.tag, n12 === 13) {
          if (n12 = Io2(t), n12 !== null) {
            e.blockedOn = n12, Yo2(e.priority, function() {
              $o2(t);
            });
            return;
          }
        } else if (n12 === 3 && t.stateNode.current.memoizedState.isDehydrated) {
          e.blockedOn = t.tag === 3 ? t.stateNode.containerInfo : null;
          return;
        }
      }
    }
    e.blockedOn = null;
  }
  function ur2(e) {
    if (e.blockedOn !== null)
      return false;
    for (var n12 = e.targetContainers; 0 < n12.length; ) {
      var t = Wl2(e.domEventName, e.eventSystemFlags, n12[0], e.nativeEvent);
      if (t === null) {
        t = e.nativeEvent;
        var r2 = new t.constructor(t.type, t);
        Ul2 = r2, t.target.dispatchEvent(r2), Ul2 = null;
      } else
        return n12 = Vt2(t), n12 !== null && _i2(n12), e.blockedOn = t, false;
      n12.shift();
    }
    return true;
  }
  function Eu(e, n12, t) {
    ur2(e) && t.delete(n12);
  }
  function tc2() {
    Hl2 = false, Xe2 !== null && ur2(Xe2) && (Xe2 = null), Ge2 !== null && ur2(Ge2) && (Ge2 = null), Ze2 !== null && ur2(Ze2) && (Ze2 = null), Ct2.forEach(Eu), xt2.forEach(Eu);
  }
  function nt2(e, n12) {
    e.blockedOn === n12 && (e.blockedOn = null, Hl2 || (Hl2 = true, ae4.unstable_scheduleCallback(ae4.unstable_NormalPriority, tc2)));
  }
  function Nt2(e) {
    function n12(l2) {
      return nt2(l2, e);
    }
    if (0 < Kt2.length) {
      nt2(Kt2[0], e);
      for (var t = 1; t < Kt2.length; t++) {
        var r2 = Kt2[t];
        r2.blockedOn === e && (r2.blockedOn = null);
      }
    }
    for (Xe2 !== null && nt2(Xe2, e), Ge2 !== null && nt2(Ge2, e), Ze2 !== null && nt2(Ze2, e), Ct2.forEach(n12), xt2.forEach(n12), t = 0; t < Qe.length; t++)
      r2 = Qe[t], r2.blockedOn === e && (r2.blockedOn = null);
    for (; 0 < Qe.length && (t = Qe[0], t.blockedOn === null); )
      Xo2(t), t.blockedOn === null && Qe.shift();
  }
  var Bn2 = Ve3.ReactCurrentBatchConfig, kr2 = true;
  function rc2(e, n12, t, r2) {
    var l2 = P2, i = Bn2.transition;
    Bn2.transition = null;
    try {
      P2 = 1, zi2(e, n12, t, r2);
    } finally {
      P2 = l2, Bn2.transition = i;
    }
  }
  function lc2(e, n12, t, r2) {
    var l2 = P2, i = Bn2.transition;
    Bn2.transition = null;
    try {
      P2 = 4, zi2(e, n12, t, r2);
    } finally {
      P2 = l2, Bn2.transition = i;
    }
  }
  function zi2(e, n12, t, r2) {
    if (kr2) {
      var l2 = Wl2(e, n12, t, r2);
      if (l2 === null)
        ml2(e, n12, r2, Er2, t), ku(e, r2);
      else if (nc2(l2, e, n12, t, r2))
        r2.stopPropagation();
      else if (ku(e, r2), n12 & 4 && -1 < ec2.indexOf(e)) {
        for (; l2 !== null; ) {
          var i = Vt2(l2);
          if (i !== null && Qo2(i), i = Wl2(e, n12, t, r2), i === null && ml2(e, n12, r2, Er2, t), i === l2)
            break;
          l2 = i;
        }
        l2 !== null && r2.stopPropagation();
      } else
        ml2(e, n12, r2, null, t);
    }
  }
  var Er2 = null;
  function Wl2(e, n12, t, r2) {
    if (Er2 = null, e = Ci2(r2), e = dn2(e), e !== null)
      if (n12 = Cn2(e), n12 === null)
        e = null;
      else if (t = n12.tag, t === 13) {
        if (e = Io2(n12), e !== null)
          return e;
        e = null;
      } else if (t === 3) {
        if (n12.stateNode.current.memoizedState.isDehydrated)
          return n12.tag === 3 ? n12.stateNode.containerInfo : null;
        e = null;
      } else
        n12 !== e && (e = null);
    return Er2 = e, null;
  }
  function Go2(e) {
    switch (e) {
      case "cancel":
      case "click":
      case "close":
      case "contextmenu":
      case "copy":
      case "cut":
      case "auxclick":
      case "dblclick":
      case "dragend":
      case "dragstart":
      case "drop":
      case "focusin":
      case "focusout":
      case "input":
      case "invalid":
      case "keydown":
      case "keypress":
      case "keyup":
      case "mousedown":
      case "mouseup":
      case "paste":
      case "pause":
      case "play":
      case "pointercancel":
      case "pointerdown":
      case "pointerup":
      case "ratechange":
      case "reset":
      case "resize":
      case "seeked":
      case "submit":
      case "touchcancel":
      case "touchend":
      case "touchstart":
      case "volumechange":
      case "change":
      case "selectionchange":
      case "textInput":
      case "compositionstart":
      case "compositionend":
      case "compositionupdate":
      case "beforeblur":
      case "afterblur":
      case "beforeinput":
      case "blur":
      case "fullscreenchange":
      case "focus":
      case "hashchange":
      case "popstate":
      case "select":
      case "selectstart":
        return 1;
      case "drag":
      case "dragenter":
      case "dragexit":
      case "dragleave":
      case "dragover":
      case "mousemove":
      case "mouseout":
      case "mouseover":
      case "pointermove":
      case "pointerout":
      case "pointerover":
      case "scroll":
      case "toggle":
      case "touchmove":
      case "wheel":
      case "mouseenter":
      case "mouseleave":
      case "pointerenter":
      case "pointerleave":
        return 4;
      case "message":
        switch ($a2()) {
          case xi2:
            return 1;
          case Ao2:
            return 4;
          case wr2:
          case Ka2:
            return 16;
          case Bo2:
            return 536870912;
          default:
            return 16;
        }
      default:
        return 16;
    }
  }
  var Ke2 = null, Pi2 = null, or2 = null;
  function Zo2() {
    if (or2)
      return or2;
    var e, n12 = Pi2, t = n12.length, r2, l2 = "value" in Ke2 ? Ke2.value : Ke2.textContent, i = l2.length;
    for (e = 0; e < t && n12[e] === l2[e]; e++)
      ;
    var u2 = t - e;
    for (r2 = 1; r2 <= u2 && n12[t - r2] === l2[i - r2]; r2++)
      ;
    return or2 = l2.slice(e, 1 < r2 ? 1 - r2 : void 0);
  }
  function sr2(e) {
    var n12 = e.keyCode;
    return "charCode" in e ? (e = e.charCode, e === 0 && n12 === 13 && (e = 13)) : e = n12, e === 10 && (e = 13), 32 <= e || e === 13 ? e : 0;
  }
  function Yt2() {
    return true;
  }
  function Cu() {
    return false;
  }
  function ce2(e) {
    function n12(t, r2, l2, i, u2) {
      this._reactName = t, this._targetInst = l2, this.type = r2, this.nativeEvent = i, this.target = u2, this.currentTarget = null;
      for (var o in e)
        e.hasOwnProperty(o) && (t = e[o], this[o] = t ? t(i) : i[o]);
      return this.isDefaultPrevented = (i.defaultPrevented != null ? i.defaultPrevented : i.returnValue === false) ? Yt2 : Cu, this.isPropagationStopped = Cu, this;
    }
    return F2(n12.prototype, { preventDefault: function() {
      this.defaultPrevented = true;
      var t = this.nativeEvent;
      t && (t.preventDefault ? t.preventDefault() : typeof t.returnValue != "unknown" && (t.returnValue = false), this.isDefaultPrevented = Yt2);
    }, stopPropagation: function() {
      var t = this.nativeEvent;
      t && (t.stopPropagation ? t.stopPropagation() : typeof t.cancelBubble != "unknown" && (t.cancelBubble = true), this.isPropagationStopped = Yt2);
    }, persist: function() {
    }, isPersistent: Yt2 }), n12;
  }
  var Jn2 = { eventPhase: 0, bubbles: 0, cancelable: 0, timeStamp: function(e) {
    return e.timeStamp || Date.now();
  }, defaultPrevented: 0, isTrusted: 0 }, Li2 = ce2(Jn2), jt2 = F2({}, Jn2, { view: 0, detail: 0 }), ic2 = ce2(jt2), sl2, al2, tt2, Br2 = F2({}, jt2, { screenX: 0, screenY: 0, clientX: 0, clientY: 0, pageX: 0, pageY: 0, ctrlKey: 0, shiftKey: 0, altKey: 0, metaKey: 0, getModifierState: Ti2, button: 0, buttons: 0, relatedTarget: function(e) {
    return e.relatedTarget === void 0 ? e.fromElement === e.srcElement ? e.toElement : e.fromElement : e.relatedTarget;
  }, movementX: function(e) {
    return "movementX" in e ? e.movementX : (e !== tt2 && (tt2 && e.type === "mousemove" ? (sl2 = e.screenX - tt2.screenX, al2 = e.screenY - tt2.screenY) : al2 = sl2 = 0, tt2 = e), sl2);
  }, movementY: function(e) {
    return "movementY" in e ? e.movementY : al2;
  } }), xu = ce2(Br2), uc2 = F2({}, Br2, { dataTransfer: 0 }), oc2 = ce2(uc2), sc2 = F2({}, jt2, { relatedTarget: 0 }), cl2 = ce2(sc2), ac2 = F2({}, Jn2, { animationName: 0, elapsedTime: 0, pseudoElement: 0 }), cc2 = ce2(ac2), fc2 = F2({}, Jn2, { clipboardData: function(e) {
    return "clipboardData" in e ? e.clipboardData : window.clipboardData;
  } }), dc2 = ce2(fc2), pc = F2({}, Jn2, { data: 0 }), Nu = ce2(pc), mc = { Esc: "Escape", Spacebar: " ", Left: "ArrowLeft", Up: "ArrowUp", Right: "ArrowRight", Down: "ArrowDown", Del: "Delete", Win: "OS", Menu: "ContextMenu", Apps: "ContextMenu", Scroll: "ScrollLock", MozPrintableKey: "Unidentified" }, hc2 = { 8: "Backspace", 9: "Tab", 12: "Clear", 13: "Enter", 16: "Shift", 17: "Control", 18: "Alt", 19: "Pause", 20: "CapsLock", 27: "Escape", 32: " ", 33: "PageUp", 34: "PageDown", 35: "End", 36: "Home", 37: "ArrowLeft", 38: "ArrowUp", 39: "ArrowRight", 40: "ArrowDown", 45: "Insert", 46: "Delete", 112: "F1", 113: "F2", 114: "F3", 115: "F4", 116: "F5", 117: "F6", 118: "F7", 119: "F8", 120: "F9", 121: "F10", 122: "F11", 123: "F12", 144: "NumLock", 145: "ScrollLock", 224: "Meta" }, vc = { Alt: "altKey", Control: "ctrlKey", Meta: "metaKey", Shift: "shiftKey" };
  function yc(e) {
    var n12 = this.nativeEvent;
    return n12.getModifierState ? n12.getModifierState(e) : (e = vc[e]) ? !!n12[e] : false;
  }
  function Ti2() {
    return yc;
  }
  var gc = F2({}, jt2, { key: function(e) {
    if (e.key) {
      var n12 = mc[e.key] || e.key;
      if (n12 !== "Unidentified")
        return n12;
    }
    return e.type === "keypress" ? (e = sr2(e), e === 13 ? "Enter" : String.fromCharCode(e)) : e.type === "keydown" || e.type === "keyup" ? hc2[e.keyCode] || "Unidentified" : "";
  }, code: 0, location: 0, ctrlKey: 0, shiftKey: 0, altKey: 0, metaKey: 0, repeat: 0, locale: 0, getModifierState: Ti2, charCode: function(e) {
    return e.type === "keypress" ? sr2(e) : 0;
  }, keyCode: function(e) {
    return e.type === "keydown" || e.type === "keyup" ? e.keyCode : 0;
  }, which: function(e) {
    return e.type === "keypress" ? sr2(e) : e.type === "keydown" || e.type === "keyup" ? e.keyCode : 0;
  } }), wc = ce2(gc), Sc = F2({}, Br2, { pointerId: 0, width: 0, height: 0, pressure: 0, tangentialPressure: 0, tiltX: 0, tiltY: 0, twist: 0, pointerType: 0, isPrimary: 0 }), _u = ce2(Sc), kc = F2({}, jt2, { touches: 0, targetTouches: 0, changedTouches: 0, altKey: 0, metaKey: 0, ctrlKey: 0, shiftKey: 0, getModifierState: Ti2 }), Ec = ce2(kc), Cc = F2({}, Jn2, { propertyName: 0, elapsedTime: 0, pseudoElement: 0 }), xc = ce2(Cc), Nc = F2({}, Br2, { deltaX: function(e) {
    return "deltaX" in e ? e.deltaX : "wheelDeltaX" in e ? -e.wheelDeltaX : 0;
  }, deltaY: function(e) {
    return "deltaY" in e ? e.deltaY : "wheelDeltaY" in e ? -e.wheelDeltaY : "wheelDelta" in e ? -e.wheelDelta : 0;
  }, deltaZ: 0, deltaMode: 0 }), _c = ce2(Nc), zc = [9, 13, 27, 32], Mi2 = Fe3 && "CompositionEvent" in window, pt2 = null;
  Fe3 && "documentMode" in document && (pt2 = document.documentMode);
  var Pc = Fe3 && "TextEvent" in window && !pt2, Jo2 = Fe3 && (!Mi2 || pt2 && 8 < pt2 && 11 >= pt2), zu = " ", Pu = false;
  function qo2(e, n12) {
    switch (e) {
      case "keyup":
        return zc.indexOf(n12.keyCode) !== -1;
      case "keydown":
        return n12.keyCode !== 229;
      case "keypress":
      case "mousedown":
      case "focusout":
        return true;
      default:
        return false;
    }
  }
  function bo2(e) {
    return e = e.detail, typeof e == "object" && "data" in e ? e.data : null;
  }
  var Pn2 = false;
  function Lc(e, n12) {
    switch (e) {
      case "compositionend":
        return bo2(n12);
      case "keypress":
        return n12.which !== 32 ? null : (Pu = true, zu);
      case "textInput":
        return e = n12.data, e === zu && Pu ? null : e;
      default:
        return null;
    }
  }
  function Tc(e, n12) {
    if (Pn2)
      return e === "compositionend" || !Mi2 && qo2(e, n12) ? (e = Zo2(), or2 = Pi2 = Ke2 = null, Pn2 = false, e) : null;
    switch (e) {
      case "paste":
        return null;
      case "keypress":
        if (!(n12.ctrlKey || n12.altKey || n12.metaKey) || n12.ctrlKey && n12.altKey) {
          if (n12.char && 1 < n12.char.length)
            return n12.char;
          if (n12.which)
            return String.fromCharCode(n12.which);
        }
        return null;
      case "compositionend":
        return Jo2 && n12.locale !== "ko" ? null : n12.data;
      default:
        return null;
    }
  }
  var Mc = { color: true, date: true, datetime: true, "datetime-local": true, email: true, month: true, number: true, password: true, range: true, search: true, tel: true, text: true, time: true, url: true, week: true };
  function Lu(e) {
    var n12 = e && e.nodeName && e.nodeName.toLowerCase();
    return n12 === "input" ? !!Mc[e.type] : n12 === "textarea";
  }
  function es(e, n12, t, r2) {
    Mo2(r2), n12 = Cr2(n12, "onChange"), 0 < n12.length && (t = new Li2("onChange", "change", null, t, r2), e.push({ event: t, listeners: n12 }));
  }
  var mt2 = null, _t2 = null;
  function Dc(e) {
    fs2(e, 0);
  }
  function Hr2(e) {
    var n12 = Mn2(e);
    if (xo2(n12))
      return e;
  }
  function Oc(e, n12) {
    if (e === "change")
      return n12;
  }
  var ns2 = false;
  Fe3 && (Fe3 ? (Gt2 = "oninput" in document, Gt2 || (fl2 = document.createElement("div"), fl2.setAttribute("oninput", "return;"), Gt2 = typeof fl2.oninput == "function"), Xt2 = Gt2) : Xt2 = false, ns2 = Xt2 && (!document.documentMode || 9 < document.documentMode));
  var Xt2, Gt2, fl2;
  function Tu() {
    mt2 && (mt2.detachEvent("onpropertychange", ts2), _t2 = mt2 = null);
  }
  function ts2(e) {
    if (e.propertyName === "value" && Hr2(_t2)) {
      var n12 = [];
      es(n12, _t2, e, Ci2(e)), Fo2(Dc, n12);
    }
  }
  function Rc(e, n12, t) {
    e === "focusin" ? (Tu(), mt2 = n12, _t2 = t, mt2.attachEvent("onpropertychange", ts2)) : e === "focusout" && Tu();
  }
  function Fc(e) {
    if (e === "selectionchange" || e === "keyup" || e === "keydown")
      return Hr2(_t2);
  }
  function Ic(e, n12) {
    if (e === "click")
      return Hr2(n12);
  }
  function Uc(e, n12) {
    if (e === "input" || e === "change")
      return Hr2(n12);
  }
  function jc(e, n12) {
    return e === n12 && (e !== 0 || 1 / e === 1 / n12) || e !== e && n12 !== n12;
  }
  var xe4 = typeof Object.is == "function" ? Object.is : jc;
  function zt2(e, n12) {
    if (xe4(e, n12))
      return true;
    if (typeof e != "object" || e === null || typeof n12 != "object" || n12 === null)
      return false;
    var t = Object.keys(e), r2 = Object.keys(n12);
    if (t.length !== r2.length)
      return false;
    for (r2 = 0; r2 < t.length; r2++) {
      var l2 = t[r2];
      if (!Nl2.call(n12, l2) || !xe4(e[l2], n12[l2]))
        return false;
    }
    return true;
  }
  function Mu(e) {
    for (; e && e.firstChild; )
      e = e.firstChild;
    return e;
  }
  function Du(e, n12) {
    var t = Mu(e);
    e = 0;
    for (var r2; t; ) {
      if (t.nodeType === 3) {
        if (r2 = e + t.textContent.length, e <= n12 && r2 >= n12)
          return { node: t, offset: n12 - e };
        e = r2;
      }
      e: {
        for (; t; ) {
          if (t.nextSibling) {
            t = t.nextSibling;
            break e;
          }
          t = t.parentNode;
        }
        t = void 0;
      }
      t = Mu(t);
    }
  }
  function rs2(e, n12) {
    return e && n12 ? e === n12 ? true : e && e.nodeType === 3 ? false : n12 && n12.nodeType === 3 ? rs2(e, n12.parentNode) : "contains" in e ? e.contains(n12) : e.compareDocumentPosition ? !!(e.compareDocumentPosition(n12) & 16) : false : false;
  }
  function ls2() {
    for (var e = window, n12 = vr2(); n12 instanceof e.HTMLIFrameElement; ) {
      try {
        var t = typeof n12.contentWindow.location.href == "string";
      } catch {
        t = false;
      }
      if (t)
        e = n12.contentWindow;
      else
        break;
      n12 = vr2(e.document);
    }
    return n12;
  }
  function Di2(e) {
    var n12 = e && e.nodeName && e.nodeName.toLowerCase();
    return n12 && (n12 === "input" && (e.type === "text" || e.type === "search" || e.type === "tel" || e.type === "url" || e.type === "password") || n12 === "textarea" || e.contentEditable === "true");
  }
  function Vc(e) {
    var n12 = ls2(), t = e.focusedElem, r2 = e.selectionRange;
    if (n12 !== t && t && t.ownerDocument && rs2(t.ownerDocument.documentElement, t)) {
      if (r2 !== null && Di2(t)) {
        if (n12 = r2.start, e = r2.end, e === void 0 && (e = n12), "selectionStart" in t)
          t.selectionStart = n12, t.selectionEnd = Math.min(e, t.value.length);
        else if (e = (n12 = t.ownerDocument || document) && n12.defaultView || window, e.getSelection) {
          e = e.getSelection();
          var l2 = t.textContent.length, i = Math.min(r2.start, l2);
          r2 = r2.end === void 0 ? i : Math.min(r2.end, l2), !e.extend && i > r2 && (l2 = r2, r2 = i, i = l2), l2 = Du(t, i);
          var u2 = Du(t, r2);
          l2 && u2 && (e.rangeCount !== 1 || e.anchorNode !== l2.node || e.anchorOffset !== l2.offset || e.focusNode !== u2.node || e.focusOffset !== u2.offset) && (n12 = n12.createRange(), n12.setStart(l2.node, l2.offset), e.removeAllRanges(), i > r2 ? (e.addRange(n12), e.extend(u2.node, u2.offset)) : (n12.setEnd(u2.node, u2.offset), e.addRange(n12)));
        }
      }
      for (n12 = [], e = t; e = e.parentNode; )
        e.nodeType === 1 && n12.push({ element: e, left: e.scrollLeft, top: e.scrollTop });
      for (typeof t.focus == "function" && t.focus(), t = 0; t < n12.length; t++)
        e = n12[t], e.element.scrollLeft = e.left, e.element.scrollTop = e.top;
    }
  }
  var Ac = Fe3 && "documentMode" in document && 11 >= document.documentMode, Ln2 = null, Ql2 = null, ht2 = null, $l2 = false;
  function Ou(e, n12, t) {
    var r2 = t.window === t ? t.document : t.nodeType === 9 ? t : t.ownerDocument;
    $l2 || Ln2 == null || Ln2 !== vr2(r2) || (r2 = Ln2, "selectionStart" in r2 && Di2(r2) ? r2 = { start: r2.selectionStart, end: r2.selectionEnd } : (r2 = (r2.ownerDocument && r2.ownerDocument.defaultView || window).getSelection(), r2 = { anchorNode: r2.anchorNode, anchorOffset: r2.anchorOffset, focusNode: r2.focusNode, focusOffset: r2.focusOffset }), ht2 && zt2(ht2, r2) || (ht2 = r2, r2 = Cr2(Ql2, "onSelect"), 0 < r2.length && (n12 = new Li2("onSelect", "select", null, n12, t), e.push({ event: n12, listeners: r2 }), n12.target = Ln2)));
  }
  function Zt2(e, n12) {
    var t = {};
    return t[e.toLowerCase()] = n12.toLowerCase(), t["Webkit" + e] = "webkit" + n12, t["Moz" + e] = "moz" + n12, t;
  }
  var Tn2 = { animationend: Zt2("Animation", "AnimationEnd"), animationiteration: Zt2("Animation", "AnimationIteration"), animationstart: Zt2("Animation", "AnimationStart"), transitionend: Zt2("Transition", "TransitionEnd") }, dl2 = {}, is2 = {};
  Fe3 && (is2 = document.createElement("div").style, "AnimationEvent" in window || (delete Tn2.animationend.animation, delete Tn2.animationiteration.animation, delete Tn2.animationstart.animation), "TransitionEvent" in window || delete Tn2.transitionend.transition);
  function Wr2(e) {
    if (dl2[e])
      return dl2[e];
    if (!Tn2[e])
      return e;
    var n12 = Tn2[e], t;
    for (t in n12)
      if (n12.hasOwnProperty(t) && t in is2)
        return dl2[e] = n12[t];
    return e;
  }
  var us2 = Wr2("animationend"), os2 = Wr2("animationiteration"), ss2 = Wr2("animationstart"), as2 = Wr2("transitionend"), cs2 = /* @__PURE__ */ new Map(), Ru = "abort auxClick cancel canPlay canPlayThrough click close contextMenu copy cut drag dragEnd dragEnter dragExit dragLeave dragOver dragStart drop durationChange emptied encrypted ended error gotPointerCapture input invalid keyDown keyPress keyUp load loadedData loadedMetadata loadStart lostPointerCapture mouseDown mouseMove mouseOut mouseOver mouseUp paste pause play playing pointerCancel pointerDown pointerMove pointerOut pointerOver pointerUp progress rateChange reset resize seeked seeking stalled submit suspend timeUpdate touchCancel touchEnd touchStart volumeChange scroll toggle touchMove waiting wheel".split(" ");
  function ln2(e, n12) {
    cs2.set(e, n12), En2(n12, [e]);
  }
  for (Jt2 = 0; Jt2 < Ru.length; Jt2++)
    qt2 = Ru[Jt2], Fu = qt2.toLowerCase(), Iu = qt2[0].toUpperCase() + qt2.slice(1), ln2(Fu, "on" + Iu);
  var qt2, Fu, Iu, Jt2;
  ln2(us2, "onAnimationEnd");
  ln2(os2, "onAnimationIteration");
  ln2(ss2, "onAnimationStart");
  ln2("dblclick", "onDoubleClick");
  ln2("focusin", "onFocus");
  ln2("focusout", "onBlur");
  ln2(as2, "onTransitionEnd");
  Qn2("onMouseEnter", ["mouseout", "mouseover"]);
  Qn2("onMouseLeave", ["mouseout", "mouseover"]);
  Qn2("onPointerEnter", ["pointerout", "pointerover"]);
  Qn2("onPointerLeave", ["pointerout", "pointerover"]);
  En2("onChange", "change click focusin focusout input keydown keyup selectionchange".split(" "));
  En2("onSelect", "focusout contextmenu dragend focusin keydown keyup mousedown mouseup selectionchange".split(" "));
  En2("onBeforeInput", ["compositionend", "keypress", "textInput", "paste"]);
  En2("onCompositionEnd", "compositionend focusout keydown keypress keyup mousedown".split(" "));
  En2("onCompositionStart", "compositionstart focusout keydown keypress keyup mousedown".split(" "));
  En2("onCompositionUpdate", "compositionupdate focusout keydown keypress keyup mousedown".split(" "));
  var ct2 = "abort canplay canplaythrough durationchange emptied encrypted ended error loadeddata loadedmetadata loadstart pause play playing progress ratechange resize seeked seeking stalled suspend timeupdate volumechange waiting".split(" "), Bc = new Set("cancel close invalid load scroll toggle".split(" ").concat(ct2));
  function Uu(e, n12, t) {
    var r2 = e.type || "unknown-event";
    e.currentTarget = t, Ba2(r2, n12, void 0, e), e.currentTarget = null;
  }
  function fs2(e, n12) {
    n12 = (n12 & 4) !== 0;
    for (var t = 0; t < e.length; t++) {
      var r2 = e[t], l2 = r2.event;
      r2 = r2.listeners;
      e: {
        var i = void 0;
        if (n12)
          for (var u2 = r2.length - 1; 0 <= u2; u2--) {
            var o = r2[u2], s2 = o.instance, d3 = o.currentTarget;
            if (o = o.listener, s2 !== i && l2.isPropagationStopped())
              break e;
            Uu(l2, o, d3), i = s2;
          }
        else
          for (u2 = 0; u2 < r2.length; u2++) {
            if (o = r2[u2], s2 = o.instance, d3 = o.currentTarget, o = o.listener, s2 !== i && l2.isPropagationStopped())
              break e;
            Uu(l2, o, d3), i = s2;
          }
      }
    }
    if (gr2)
      throw e = Al2, gr2 = false, Al2 = null, e;
  }
  function T2(e, n12) {
    var t = n12[Zl2];
    t === void 0 && (t = n12[Zl2] = /* @__PURE__ */ new Set());
    var r2 = e + "__bubble";
    t.has(r2) || (ds2(n12, e, 2, false), t.add(r2));
  }
  function pl2(e, n12, t) {
    var r2 = 0;
    n12 && (r2 |= 4), ds2(t, e, r2, n12);
  }
  var bt2 = "_reactListening" + Math.random().toString(36).slice(2);
  function Pt2(e) {
    if (!e[bt2]) {
      e[bt2] = true, wo2.forEach(function(t) {
        t !== "selectionchange" && (Bc.has(t) || pl2(t, false, e), pl2(t, true, e));
      });
      var n12 = e.nodeType === 9 ? e : e.ownerDocument;
      n12 === null || n12[bt2] || (n12[bt2] = true, pl2("selectionchange", false, n12));
    }
  }
  function ds2(e, n12, t, r2) {
    switch (Go2(n12)) {
      case 1:
        var l2 = rc2;
        break;
      case 4:
        l2 = lc2;
        break;
      default:
        l2 = zi2;
    }
    t = l2.bind(null, n12, t, e), l2 = void 0, !Vl2 || n12 !== "touchstart" && n12 !== "touchmove" && n12 !== "wheel" || (l2 = true), r2 ? l2 !== void 0 ? e.addEventListener(n12, t, { capture: true, passive: l2 }) : e.addEventListener(n12, t, true) : l2 !== void 0 ? e.addEventListener(n12, t, { passive: l2 }) : e.addEventListener(n12, t, false);
  }
  function ml2(e, n12, t, r2, l2) {
    var i = r2;
    if (!(n12 & 1) && !(n12 & 2) && r2 !== null)
      e:
        for (; ; ) {
          if (r2 === null)
            return;
          var u2 = r2.tag;
          if (u2 === 3 || u2 === 4) {
            var o = r2.stateNode.containerInfo;
            if (o === l2 || o.nodeType === 8 && o.parentNode === l2)
              break;
            if (u2 === 4)
              for (u2 = r2.return; u2 !== null; ) {
                var s2 = u2.tag;
                if ((s2 === 3 || s2 === 4) && (s2 = u2.stateNode.containerInfo, s2 === l2 || s2.nodeType === 8 && s2.parentNode === l2))
                  return;
                u2 = u2.return;
              }
            for (; o !== null; ) {
              if (u2 = dn2(o), u2 === null)
                return;
              if (s2 = u2.tag, s2 === 5 || s2 === 6) {
                r2 = i = u2;
                continue e;
              }
              o = o.parentNode;
            }
          }
          r2 = r2.return;
        }
    Fo2(function() {
      var d3 = i, m3 = Ci2(t), h2 = [];
      e: {
        var p3 = cs2.get(e);
        if (p3 !== void 0) {
          var g2 = Li2, S2 = e;
          switch (e) {
            case "keypress":
              if (sr2(t) === 0)
                break e;
            case "keydown":
            case "keyup":
              g2 = wc;
              break;
            case "focusin":
              S2 = "focus", g2 = cl2;
              break;
            case "focusout":
              S2 = "blur", g2 = cl2;
              break;
            case "beforeblur":
            case "afterblur":
              g2 = cl2;
              break;
            case "click":
              if (t.button === 2)
                break e;
            case "auxclick":
            case "dblclick":
            case "mousedown":
            case "mousemove":
            case "mouseup":
            case "mouseout":
            case "mouseover":
            case "contextmenu":
              g2 = xu;
              break;
            case "drag":
            case "dragend":
            case "dragenter":
            case "dragexit":
            case "dragleave":
            case "dragover":
            case "dragstart":
            case "drop":
              g2 = oc2;
              break;
            case "touchcancel":
            case "touchend":
            case "touchmove":
            case "touchstart":
              g2 = Ec;
              break;
            case us2:
            case os2:
            case ss2:
              g2 = cc2;
              break;
            case as2:
              g2 = xc;
              break;
            case "scroll":
              g2 = ic2;
              break;
            case "wheel":
              g2 = _c;
              break;
            case "copy":
            case "cut":
            case "paste":
              g2 = dc2;
              break;
            case "gotpointercapture":
            case "lostpointercapture":
            case "pointercancel":
            case "pointerdown":
            case "pointermove":
            case "pointerout":
            case "pointerover":
            case "pointerup":
              g2 = _u;
          }
          var k = (n12 & 4) !== 0, U3 = !k && e === "scroll", c = k ? p3 !== null ? p3 + "Capture" : null : p3;
          k = [];
          for (var a2 = d3, f3; a2 !== null; ) {
            f3 = a2;
            var y3 = f3.stateNode;
            if (f3.tag === 5 && y3 !== null && (f3 = y3, c !== null && (y3 = Et2(a2, c), y3 != null && k.push(Lt2(a2, y3, f3)))), U3)
              break;
            a2 = a2.return;
          }
          0 < k.length && (p3 = new g2(p3, S2, null, t, m3), h2.push({ event: p3, listeners: k }));
        }
      }
      if (!(n12 & 7)) {
        e: {
          if (p3 = e === "mouseover" || e === "pointerover", g2 = e === "mouseout" || e === "pointerout", p3 && t !== Ul2 && (S2 = t.relatedTarget || t.fromElement) && (dn2(S2) || S2[Ie4]))
            break e;
          if ((g2 || p3) && (p3 = m3.window === m3 ? m3 : (p3 = m3.ownerDocument) ? p3.defaultView || p3.parentWindow : window, g2 ? (S2 = t.relatedTarget || t.toElement, g2 = d3, S2 = S2 ? dn2(S2) : null, S2 !== null && (U3 = Cn2(S2), S2 !== U3 || S2.tag !== 5 && S2.tag !== 6) && (S2 = null)) : (g2 = null, S2 = d3), g2 !== S2)) {
            if (k = xu, y3 = "onMouseLeave", c = "onMouseEnter", a2 = "mouse", (e === "pointerout" || e === "pointerover") && (k = _u, y3 = "onPointerLeave", c = "onPointerEnter", a2 = "pointer"), U3 = g2 == null ? p3 : Mn2(g2), f3 = S2 == null ? p3 : Mn2(S2), p3 = new k(y3, a2 + "leave", g2, t, m3), p3.target = U3, p3.relatedTarget = f3, y3 = null, dn2(m3) === d3 && (k = new k(c, a2 + "enter", S2, t, m3), k.target = f3, k.relatedTarget = U3, y3 = k), U3 = y3, g2 && S2)
              n: {
                for (k = g2, c = S2, a2 = 0, f3 = k; f3; f3 = Nn2(f3))
                  a2++;
                for (f3 = 0, y3 = c; y3; y3 = Nn2(y3))
                  f3++;
                for (; 0 < a2 - f3; )
                  k = Nn2(k), a2--;
                for (; 0 < f3 - a2; )
                  c = Nn2(c), f3--;
                for (; a2--; ) {
                  if (k === c || c !== null && k === c.alternate)
                    break n;
                  k = Nn2(k), c = Nn2(c);
                }
                k = null;
              }
            else
              k = null;
            g2 !== null && ju(h2, p3, g2, k, false), S2 !== null && U3 !== null && ju(h2, U3, S2, k, true);
          }
        }
        e: {
          if (p3 = d3 ? Mn2(d3) : window, g2 = p3.nodeName && p3.nodeName.toLowerCase(), g2 === "select" || g2 === "input" && p3.type === "file")
            var E4 = Oc;
          else if (Lu(p3))
            if (ns2)
              E4 = Uc;
            else {
              E4 = Fc;
              var C2 = Rc;
            }
          else
            (g2 = p3.nodeName) && g2.toLowerCase() === "input" && (p3.type === "checkbox" || p3.type === "radio") && (E4 = Ic);
          if (E4 && (E4 = E4(e, d3))) {
            es(h2, E4, t, m3);
            break e;
          }
          C2 && C2(e, p3, d3), e === "focusout" && (C2 = p3._wrapperState) && C2.controlled && p3.type === "number" && Dl2(p3, "number", p3.value);
        }
        switch (C2 = d3 ? Mn2(d3) : window, e) {
          case "focusin":
            (Lu(C2) || C2.contentEditable === "true") && (Ln2 = C2, Ql2 = d3, ht2 = null);
            break;
          case "focusout":
            ht2 = Ql2 = Ln2 = null;
            break;
          case "mousedown":
            $l2 = true;
            break;
          case "contextmenu":
          case "mouseup":
          case "dragend":
            $l2 = false, Ou(h2, t, m3);
            break;
          case "selectionchange":
            if (Ac)
              break;
          case "keydown":
          case "keyup":
            Ou(h2, t, m3);
        }
        var x3;
        if (Mi2)
          e: {
            switch (e) {
              case "compositionstart":
                var N3 = "onCompositionStart";
                break e;
              case "compositionend":
                N3 = "onCompositionEnd";
                break e;
              case "compositionupdate":
                N3 = "onCompositionUpdate";
                break e;
            }
            N3 = void 0;
          }
        else
          Pn2 ? qo2(e, t) && (N3 = "onCompositionEnd") : e === "keydown" && t.keyCode === 229 && (N3 = "onCompositionStart");
        N3 && (Jo2 && t.locale !== "ko" && (Pn2 || N3 !== "onCompositionStart" ? N3 === "onCompositionEnd" && Pn2 && (x3 = Zo2()) : (Ke2 = m3, Pi2 = "value" in Ke2 ? Ke2.value : Ke2.textContent, Pn2 = true)), C2 = Cr2(d3, N3), 0 < C2.length && (N3 = new Nu(N3, e, null, t, m3), h2.push({ event: N3, listeners: C2 }), x3 ? N3.data = x3 : (x3 = bo2(t), x3 !== null && (N3.data = x3)))), (x3 = Pc ? Lc(e, t) : Tc(e, t)) && (d3 = Cr2(d3, "onBeforeInput"), 0 < d3.length && (m3 = new Nu("onBeforeInput", "beforeinput", null, t, m3), h2.push({ event: m3, listeners: d3 }), m3.data = x3));
      }
      fs2(h2, n12);
    });
  }
  function Lt2(e, n12, t) {
    return { instance: e, listener: n12, currentTarget: t };
  }
  function Cr2(e, n12) {
    for (var t = n12 + "Capture", r2 = []; e !== null; ) {
      var l2 = e, i = l2.stateNode;
      l2.tag === 5 && i !== null && (l2 = i, i = Et2(e, t), i != null && r2.unshift(Lt2(e, i, l2)), i = Et2(e, n12), i != null && r2.push(Lt2(e, i, l2))), e = e.return;
    }
    return r2;
  }
  function Nn2(e) {
    if (e === null)
      return null;
    do
      e = e.return;
    while (e && e.tag !== 5);
    return e || null;
  }
  function ju(e, n12, t, r2, l2) {
    for (var i = n12._reactName, u2 = []; t !== null && t !== r2; ) {
      var o = t, s2 = o.alternate, d3 = o.stateNode;
      if (s2 !== null && s2 === r2)
        break;
      o.tag === 5 && d3 !== null && (o = d3, l2 ? (s2 = Et2(t, i), s2 != null && u2.unshift(Lt2(t, s2, o))) : l2 || (s2 = Et2(t, i), s2 != null && u2.push(Lt2(t, s2, o)))), t = t.return;
    }
    u2.length !== 0 && e.push({ event: n12, listeners: u2 });
  }
  var Hc = /\r\n?/g, Wc = /\u0000|\uFFFD/g;
  function Vu(e) {
    return (typeof e == "string" ? e : "" + e).replace(Hc, `
`).replace(Wc, "");
  }
  function er2(e, n12, t) {
    if (n12 = Vu(n12), Vu(e) !== n12 && t)
      throw Error(v3(425));
  }
  function xr2() {
  }
  var Kl2 = null, Yl2 = null;
  function Xl2(e, n12) {
    return e === "textarea" || e === "noscript" || typeof n12.children == "string" || typeof n12.children == "number" || typeof n12.dangerouslySetInnerHTML == "object" && n12.dangerouslySetInnerHTML !== null && n12.dangerouslySetInnerHTML.__html != null;
  }
  var Gl2 = typeof setTimeout == "function" ? setTimeout : void 0, Qc = typeof clearTimeout == "function" ? clearTimeout : void 0, Au = typeof Promise == "function" ? Promise : void 0, $c = typeof queueMicrotask == "function" ? queueMicrotask : typeof Au < "u" ? function(e) {
    return Au.resolve(null).then(e).catch(Kc);
  } : Gl2;
  function Kc(e) {
    setTimeout(function() {
      throw e;
    });
  }
  function hl2(e, n12) {
    var t = n12, r2 = 0;
    do {
      var l2 = t.nextSibling;
      if (e.removeChild(t), l2 && l2.nodeType === 8)
        if (t = l2.data, t === "/$") {
          if (r2 === 0) {
            e.removeChild(l2), Nt2(n12);
            return;
          }
          r2--;
        } else
          t !== "$" && t !== "$?" && t !== "$!" || r2++;
      t = l2;
    } while (t);
    Nt2(n12);
  }
  function Je2(e) {
    for (; e != null; e = e.nextSibling) {
      var n12 = e.nodeType;
      if (n12 === 1 || n12 === 3)
        break;
      if (n12 === 8) {
        if (n12 = e.data, n12 === "$" || n12 === "$!" || n12 === "$?")
          break;
        if (n12 === "/$")
          return null;
      }
    }
    return e;
  }
  function Bu(e) {
    e = e.previousSibling;
    for (var n12 = 0; e; ) {
      if (e.nodeType === 8) {
        var t = e.data;
        if (t === "$" || t === "$!" || t === "$?") {
          if (n12 === 0)
            return e;
          n12--;
        } else
          t === "/$" && n12++;
      }
      e = e.previousSibling;
    }
    return null;
  }
  var qn2 = Math.random().toString(36).slice(2), ze3 = "__reactFiber$" + qn2, Tt2 = "__reactProps$" + qn2, Ie4 = "__reactContainer$" + qn2, Zl2 = "__reactEvents$" + qn2, Yc = "__reactListeners$" + qn2, Xc = "__reactHandles$" + qn2;
  function dn2(e) {
    var n12 = e[ze3];
    if (n12)
      return n12;
    for (var t = e.parentNode; t; ) {
      if (n12 = t[Ie4] || t[ze3]) {
        if (t = n12.alternate, n12.child !== null || t !== null && t.child !== null)
          for (e = Bu(e); e !== null; ) {
            if (t = e[ze3])
              return t;
            e = Bu(e);
          }
        return n12;
      }
      e = t, t = e.parentNode;
    }
    return null;
  }
  function Vt2(e) {
    return e = e[ze3] || e[Ie4], !e || e.tag !== 5 && e.tag !== 6 && e.tag !== 13 && e.tag !== 3 ? null : e;
  }
  function Mn2(e) {
    if (e.tag === 5 || e.tag === 6)
      return e.stateNode;
    throw Error(v3(33));
  }
  function Qr2(e) {
    return e[Tt2] || null;
  }
  var Jl2 = [], Dn2 = -1;
  function un2(e) {
    return { current: e };
  }
  function M2(e) {
    0 > Dn2 || (e.current = Jl2[Dn2], Jl2[Dn2] = null, Dn2--);
  }
  function L(e, n12) {
    Dn2++, Jl2[Dn2] = e.current, e.current = n12;
  }
  var rn2 = {}, J2 = un2(rn2), re2 = un2(false), yn2 = rn2;
  function $n2(e, n12) {
    var t = e.type.contextTypes;
    if (!t)
      return rn2;
    var r2 = e.stateNode;
    if (r2 && r2.__reactInternalMemoizedUnmaskedChildContext === n12)
      return r2.__reactInternalMemoizedMaskedChildContext;
    var l2 = {}, i;
    for (i in t)
      l2[i] = n12[i];
    return r2 && (e = e.stateNode, e.__reactInternalMemoizedUnmaskedChildContext = n12, e.__reactInternalMemoizedMaskedChildContext = l2), l2;
  }
  function le4(e) {
    return e = e.childContextTypes, e != null;
  }
  function Nr2() {
    M2(re2), M2(J2);
  }
  function Hu(e, n12, t) {
    if (J2.current !== rn2)
      throw Error(v3(168));
    L(J2, n12), L(re2, t);
  }
  function ps2(e, n12, t) {
    var r2 = e.stateNode;
    if (n12 = n12.childContextTypes, typeof r2.getChildContext != "function")
      return t;
    r2 = r2.getChildContext();
    for (var l2 in r2)
      if (!(l2 in n12))
        throw Error(v3(108, Ra2(e) || "Unknown", l2));
    return F2({}, t, r2);
  }
  function _r2(e) {
    return e = (e = e.stateNode) && e.__reactInternalMemoizedMergedChildContext || rn2, yn2 = J2.current, L(J2, e), L(re2, re2.current), true;
  }
  function Wu(e, n12, t) {
    var r2 = e.stateNode;
    if (!r2)
      throw Error(v3(169));
    t ? (e = ps2(e, n12, yn2), r2.__reactInternalMemoizedMergedChildContext = e, M2(re2), M2(J2), L(J2, e)) : M2(re2), L(re2, t);
  }
  var Me3 = null, $r2 = false, vl2 = false;
  function ms2(e) {
    Me3 === null ? Me3 = [e] : Me3.push(e);
  }
  function Gc(e) {
    $r2 = true, ms2(e);
  }
  function on2() {
    if (!vl2 && Me3 !== null) {
      vl2 = true;
      var e = 0, n12 = P2;
      try {
        var t = Me3;
        for (P2 = 1; e < t.length; e++) {
          var r2 = t[e];
          do
            r2 = r2(true);
          while (r2 !== null);
        }
        Me3 = null, $r2 = false;
      } catch (l2) {
        throw Me3 !== null && (Me3 = Me3.slice(e + 1)), Vo2(xi2, on2), l2;
      } finally {
        P2 = n12, vl2 = false;
      }
    }
    return null;
  }
  var On2 = [], Rn2 = 0, zr2 = null, Pr2 = 0, de4 = [], pe4 = 0, gn2 = null, De3 = 1, Oe3 = "";
  function cn2(e, n12) {
    On2[Rn2++] = Pr2, On2[Rn2++] = zr2, zr2 = e, Pr2 = n12;
  }
  function hs2(e, n12, t) {
    de4[pe4++] = De3, de4[pe4++] = Oe3, de4[pe4++] = gn2, gn2 = e;
    var r2 = De3;
    e = Oe3;
    var l2 = 32 - Ee4(r2) - 1;
    r2 &= ~(1 << l2), t += 1;
    var i = 32 - Ee4(n12) + l2;
    if (30 < i) {
      var u2 = l2 - l2 % 5;
      i = (r2 & (1 << u2) - 1).toString(32), r2 >>= u2, l2 -= u2, De3 = 1 << 32 - Ee4(n12) + l2 | t << l2 | r2, Oe3 = i + e;
    } else
      De3 = 1 << i | t << l2 | r2, Oe3 = e;
  }
  function Oi2(e) {
    e.return !== null && (cn2(e, 1), hs2(e, 1, 0));
  }
  function Ri2(e) {
    for (; e === zr2; )
      zr2 = On2[--Rn2], On2[Rn2] = null, Pr2 = On2[--Rn2], On2[Rn2] = null;
    for (; e === gn2; )
      gn2 = de4[--pe4], de4[pe4] = null, Oe3 = de4[--pe4], de4[pe4] = null, De3 = de4[--pe4], de4[pe4] = null;
  }
  var se3 = null, oe3 = null, D2 = false, ke4 = null;
  function vs2(e, n12) {
    var t = me4(5, null, null, 0);
    t.elementType = "DELETED", t.stateNode = n12, t.return = e, n12 = e.deletions, n12 === null ? (e.deletions = [t], e.flags |= 16) : n12.push(t);
  }
  function Qu(e, n12) {
    switch (e.tag) {
      case 5:
        var t = e.type;
        return n12 = n12.nodeType !== 1 || t.toLowerCase() !== n12.nodeName.toLowerCase() ? null : n12, n12 !== null ? (e.stateNode = n12, se3 = e, oe3 = Je2(n12.firstChild), true) : false;
      case 6:
        return n12 = e.pendingProps === "" || n12.nodeType !== 3 ? null : n12, n12 !== null ? (e.stateNode = n12, se3 = e, oe3 = null, true) : false;
      case 13:
        return n12 = n12.nodeType !== 8 ? null : n12, n12 !== null ? (t = gn2 !== null ? { id: De3, overflow: Oe3 } : null, e.memoizedState = { dehydrated: n12, treeContext: t, retryLane: 1073741824 }, t = me4(18, null, null, 0), t.stateNode = n12, t.return = e, e.child = t, se3 = e, oe3 = null, true) : false;
      default:
        return false;
    }
  }
  function ql2(e) {
    return (e.mode & 1) !== 0 && (e.flags & 128) === 0;
  }
  function bl2(e) {
    if (D2) {
      var n12 = oe3;
      if (n12) {
        var t = n12;
        if (!Qu(e, n12)) {
          if (ql2(e))
            throw Error(v3(418));
          n12 = Je2(t.nextSibling);
          var r2 = se3;
          n12 && Qu(e, n12) ? vs2(r2, t) : (e.flags = e.flags & -4097 | 2, D2 = false, se3 = e);
        }
      } else {
        if (ql2(e))
          throw Error(v3(418));
        e.flags = e.flags & -4097 | 2, D2 = false, se3 = e;
      }
    }
  }
  function $u(e) {
    for (e = e.return; e !== null && e.tag !== 5 && e.tag !== 3 && e.tag !== 13; )
      e = e.return;
    se3 = e;
  }
  function nr2(e) {
    if (e !== se3)
      return false;
    if (!D2)
      return $u(e), D2 = true, false;
    var n12;
    if ((n12 = e.tag !== 3) && !(n12 = e.tag !== 5) && (n12 = e.type, n12 = n12 !== "head" && n12 !== "body" && !Xl2(e.type, e.memoizedProps)), n12 && (n12 = oe3)) {
      if (ql2(e))
        throw ys2(), Error(v3(418));
      for (; n12; )
        vs2(e, n12), n12 = Je2(n12.nextSibling);
    }
    if ($u(e), e.tag === 13) {
      if (e = e.memoizedState, e = e !== null ? e.dehydrated : null, !e)
        throw Error(v3(317));
      e: {
        for (e = e.nextSibling, n12 = 0; e; ) {
          if (e.nodeType === 8) {
            var t = e.data;
            if (t === "/$") {
              if (n12 === 0) {
                oe3 = Je2(e.nextSibling);
                break e;
              }
              n12--;
            } else
              t !== "$" && t !== "$!" && t !== "$?" || n12++;
          }
          e = e.nextSibling;
        }
        oe3 = null;
      }
    } else
      oe3 = se3 ? Je2(e.stateNode.nextSibling) : null;
    return true;
  }
  function ys2() {
    for (var e = oe3; e; )
      e = Je2(e.nextSibling);
  }
  function Kn2() {
    oe3 = se3 = null, D2 = false;
  }
  function Fi2(e) {
    ke4 === null ? ke4 = [e] : ke4.push(e);
  }
  var Zc = Ve3.ReactCurrentBatchConfig;
  function we4(e, n12) {
    if (e && e.defaultProps) {
      n12 = F2({}, n12), e = e.defaultProps;
      for (var t in e)
        n12[t] === void 0 && (n12[t] = e[t]);
      return n12;
    }
    return n12;
  }
  var Lr2 = un2(null), Tr2 = null, Fn2 = null, Ii2 = null;
  function Ui2() {
    Ii2 = Fn2 = Tr2 = null;
  }
  function ji2(e) {
    var n12 = Lr2.current;
    M2(Lr2), e._currentValue = n12;
  }
  function ei2(e, n12, t) {
    for (; e !== null; ) {
      var r2 = e.alternate;
      if ((e.childLanes & n12) !== n12 ? (e.childLanes |= n12, r2 !== null && (r2.childLanes |= n12)) : r2 !== null && (r2.childLanes & n12) !== n12 && (r2.childLanes |= n12), e === t)
        break;
      e = e.return;
    }
  }
  function Hn2(e, n12) {
    Tr2 = e, Ii2 = Fn2 = null, e = e.dependencies, e !== null && e.firstContext !== null && (e.lanes & n12 && (te3 = true), e.firstContext = null);
  }
  function ve4(e) {
    var n12 = e._currentValue;
    if (Ii2 !== e)
      if (e = { context: e, memoizedValue: n12, next: null }, Fn2 === null) {
        if (Tr2 === null)
          throw Error(v3(308));
        Fn2 = e, Tr2.dependencies = { lanes: 0, firstContext: e };
      } else
        Fn2 = Fn2.next = e;
    return n12;
  }
  var pn2 = null;
  function Vi2(e) {
    pn2 === null ? pn2 = [e] : pn2.push(e);
  }
  function gs2(e, n12, t, r2) {
    var l2 = n12.interleaved;
    return l2 === null ? (t.next = t, Vi2(n12)) : (t.next = l2.next, l2.next = t), n12.interleaved = t, Ue3(e, r2);
  }
  function Ue3(e, n12) {
    e.lanes |= n12;
    var t = e.alternate;
    for (t !== null && (t.lanes |= n12), t = e, e = e.return; e !== null; )
      e.childLanes |= n12, t = e.alternate, t !== null && (t.childLanes |= n12), t = e, e = e.return;
    return t.tag === 3 ? t.stateNode : null;
  }
  var We3 = false;
  function Ai2(e) {
    e.updateQueue = { baseState: e.memoizedState, firstBaseUpdate: null, lastBaseUpdate: null, shared: { pending: null, interleaved: null, lanes: 0 }, effects: null };
  }
  function ws2(e, n12) {
    e = e.updateQueue, n12.updateQueue === e && (n12.updateQueue = { baseState: e.baseState, firstBaseUpdate: e.firstBaseUpdate, lastBaseUpdate: e.lastBaseUpdate, shared: e.shared, effects: e.effects });
  }
  function Re3(e, n12) {
    return { eventTime: e, lane: n12, tag: 0, payload: null, callback: null, next: null };
  }
  function qe3(e, n12, t) {
    var r2 = e.updateQueue;
    if (r2 === null)
      return null;
    if (r2 = r2.shared, _ & 2) {
      var l2 = r2.pending;
      return l2 === null ? n12.next = n12 : (n12.next = l2.next, l2.next = n12), r2.pending = n12, Ue3(e, t);
    }
    return l2 = r2.interleaved, l2 === null ? (n12.next = n12, Vi2(r2)) : (n12.next = l2.next, l2.next = n12), r2.interleaved = n12, Ue3(e, t);
  }
  function ar2(e, n12, t) {
    if (n12 = n12.updateQueue, n12 !== null && (n12 = n12.shared, (t & 4194240) !== 0)) {
      var r2 = n12.lanes;
      r2 &= e.pendingLanes, t |= r2, n12.lanes = t, Ni2(e, t);
    }
  }
  function Ku(e, n12) {
    var t = e.updateQueue, r2 = e.alternate;
    if (r2 !== null && (r2 = r2.updateQueue, t === r2)) {
      var l2 = null, i = null;
      if (t = t.firstBaseUpdate, t !== null) {
        do {
          var u2 = { eventTime: t.eventTime, lane: t.lane, tag: t.tag, payload: t.payload, callback: t.callback, next: null };
          i === null ? l2 = i = u2 : i = i.next = u2, t = t.next;
        } while (t !== null);
        i === null ? l2 = i = n12 : i = i.next = n12;
      } else
        l2 = i = n12;
      t = { baseState: r2.baseState, firstBaseUpdate: l2, lastBaseUpdate: i, shared: r2.shared, effects: r2.effects }, e.updateQueue = t;
      return;
    }
    e = t.lastBaseUpdate, e === null ? t.firstBaseUpdate = n12 : e.next = n12, t.lastBaseUpdate = n12;
  }
  function Mr2(e, n12, t, r2) {
    var l2 = e.updateQueue;
    We3 = false;
    var i = l2.firstBaseUpdate, u2 = l2.lastBaseUpdate, o = l2.shared.pending;
    if (o !== null) {
      l2.shared.pending = null;
      var s2 = o, d3 = s2.next;
      s2.next = null, u2 === null ? i = d3 : u2.next = d3, u2 = s2;
      var m3 = e.alternate;
      m3 !== null && (m3 = m3.updateQueue, o = m3.lastBaseUpdate, o !== u2 && (o === null ? m3.firstBaseUpdate = d3 : o.next = d3, m3.lastBaseUpdate = s2));
    }
    if (i !== null) {
      var h2 = l2.baseState;
      u2 = 0, m3 = d3 = s2 = null, o = i;
      do {
        var p3 = o.lane, g2 = o.eventTime;
        if ((r2 & p3) === p3) {
          m3 !== null && (m3 = m3.next = { eventTime: g2, lane: 0, tag: o.tag, payload: o.payload, callback: o.callback, next: null });
          e: {
            var S2 = e, k = o;
            switch (p3 = n12, g2 = t, k.tag) {
              case 1:
                if (S2 = k.payload, typeof S2 == "function") {
                  h2 = S2.call(g2, h2, p3);
                  break e;
                }
                h2 = S2;
                break e;
              case 3:
                S2.flags = S2.flags & -65537 | 128;
              case 0:
                if (S2 = k.payload, p3 = typeof S2 == "function" ? S2.call(g2, h2, p3) : S2, p3 == null)
                  break e;
                h2 = F2({}, h2, p3);
                break e;
              case 2:
                We3 = true;
            }
          }
          o.callback !== null && o.lane !== 0 && (e.flags |= 64, p3 = l2.effects, p3 === null ? l2.effects = [o] : p3.push(o));
        } else
          g2 = { eventTime: g2, lane: p3, tag: o.tag, payload: o.payload, callback: o.callback, next: null }, m3 === null ? (d3 = m3 = g2, s2 = h2) : m3 = m3.next = g2, u2 |= p3;
        if (o = o.next, o === null) {
          if (o = l2.shared.pending, o === null)
            break;
          p3 = o, o = p3.next, p3.next = null, l2.lastBaseUpdate = p3, l2.shared.pending = null;
        }
      } while (true);
      if (m3 === null && (s2 = h2), l2.baseState = s2, l2.firstBaseUpdate = d3, l2.lastBaseUpdate = m3, n12 = l2.shared.interleaved, n12 !== null) {
        l2 = n12;
        do
          u2 |= l2.lane, l2 = l2.next;
        while (l2 !== n12);
      } else
        i === null && (l2.shared.lanes = 0);
      Sn2 |= u2, e.lanes = u2, e.memoizedState = h2;
    }
  }
  function Yu(e, n12, t) {
    if (e = n12.effects, n12.effects = null, e !== null)
      for (n12 = 0; n12 < e.length; n12++) {
        var r2 = e[n12], l2 = r2.callback;
        if (l2 !== null) {
          if (r2.callback = null, r2 = t, typeof l2 != "function")
            throw Error(v3(191, l2));
          l2.call(r2);
        }
      }
  }
  var Ss2 = new go2.Component().refs;
  function ni2(e, n12, t, r2) {
    n12 = e.memoizedState, t = t(r2, n12), t = t == null ? n12 : F2({}, n12, t), e.memoizedState = t, e.lanes === 0 && (e.updateQueue.baseState = t);
  }
  var Kr2 = { isMounted: function(e) {
    return (e = e._reactInternals) ? Cn2(e) === e : false;
  }, enqueueSetState: function(e, n12, t) {
    e = e._reactInternals;
    var r2 = b(), l2 = en2(e), i = Re3(r2, l2);
    i.payload = n12, t != null && (i.callback = t), n12 = qe3(e, i, l2), n12 !== null && (Ce4(n12, e, l2, r2), ar2(n12, e, l2));
  }, enqueueReplaceState: function(e, n12, t) {
    e = e._reactInternals;
    var r2 = b(), l2 = en2(e), i = Re3(r2, l2);
    i.tag = 1, i.payload = n12, t != null && (i.callback = t), n12 = qe3(e, i, l2), n12 !== null && (Ce4(n12, e, l2, r2), ar2(n12, e, l2));
  }, enqueueForceUpdate: function(e, n12) {
    e = e._reactInternals;
    var t = b(), r2 = en2(e), l2 = Re3(t, r2);
    l2.tag = 2, n12 != null && (l2.callback = n12), n12 = qe3(e, l2, r2), n12 !== null && (Ce4(n12, e, r2, t), ar2(n12, e, r2));
  } };
  function Xu(e, n12, t, r2, l2, i, u2) {
    return e = e.stateNode, typeof e.shouldComponentUpdate == "function" ? e.shouldComponentUpdate(r2, i, u2) : n12.prototype && n12.prototype.isPureReactComponent ? !zt2(t, r2) || !zt2(l2, i) : true;
  }
  function ks2(e, n12, t) {
    var r2 = false, l2 = rn2, i = n12.contextType;
    return typeof i == "object" && i !== null ? i = ve4(i) : (l2 = le4(n12) ? yn2 : J2.current, r2 = n12.contextTypes, i = (r2 = r2 != null) ? $n2(e, l2) : rn2), n12 = new n12(t, i), e.memoizedState = n12.state !== null && n12.state !== void 0 ? n12.state : null, n12.updater = Kr2, e.stateNode = n12, n12._reactInternals = e, r2 && (e = e.stateNode, e.__reactInternalMemoizedUnmaskedChildContext = l2, e.__reactInternalMemoizedMaskedChildContext = i), n12;
  }
  function Gu(e, n12, t, r2) {
    e = n12.state, typeof n12.componentWillReceiveProps == "function" && n12.componentWillReceiveProps(t, r2), typeof n12.UNSAFE_componentWillReceiveProps == "function" && n12.UNSAFE_componentWillReceiveProps(t, r2), n12.state !== e && Kr2.enqueueReplaceState(n12, n12.state, null);
  }
  function ti2(e, n12, t, r2) {
    var l2 = e.stateNode;
    l2.props = t, l2.state = e.memoizedState, l2.refs = Ss2, Ai2(e);
    var i = n12.contextType;
    typeof i == "object" && i !== null ? l2.context = ve4(i) : (i = le4(n12) ? yn2 : J2.current, l2.context = $n2(e, i)), l2.state = e.memoizedState, i = n12.getDerivedStateFromProps, typeof i == "function" && (ni2(e, n12, i, t), l2.state = e.memoizedState), typeof n12.getDerivedStateFromProps == "function" || typeof l2.getSnapshotBeforeUpdate == "function" || typeof l2.UNSAFE_componentWillMount != "function" && typeof l2.componentWillMount != "function" || (n12 = l2.state, typeof l2.componentWillMount == "function" && l2.componentWillMount(), typeof l2.UNSAFE_componentWillMount == "function" && l2.UNSAFE_componentWillMount(), n12 !== l2.state && Kr2.enqueueReplaceState(l2, l2.state, null), Mr2(e, t, l2, r2), l2.state = e.memoizedState), typeof l2.componentDidMount == "function" && (e.flags |= 4194308);
  }
  function rt2(e, n12, t) {
    if (e = t.ref, e !== null && typeof e != "function" && typeof e != "object") {
      if (t._owner) {
        if (t = t._owner, t) {
          if (t.tag !== 1)
            throw Error(v3(309));
          var r2 = t.stateNode;
        }
        if (!r2)
          throw Error(v3(147, e));
        var l2 = r2, i = "" + e;
        return n12 !== null && n12.ref !== null && typeof n12.ref == "function" && n12.ref._stringRef === i ? n12.ref : (n12 = function(u2) {
          var o = l2.refs;
          o === Ss2 && (o = l2.refs = {}), u2 === null ? delete o[i] : o[i] = u2;
        }, n12._stringRef = i, n12);
      }
      if (typeof e != "string")
        throw Error(v3(284));
      if (!t._owner)
        throw Error(v3(290, e));
    }
    return e;
  }
  function tr2(e, n12) {
    throw e = Object.prototype.toString.call(n12), Error(v3(31, e === "[object Object]" ? "object with keys {" + Object.keys(n12).join(", ") + "}" : e));
  }
  function Zu(e) {
    var n12 = e._init;
    return n12(e._payload);
  }
  function Es2(e) {
    function n12(c, a2) {
      if (e) {
        var f3 = c.deletions;
        f3 === null ? (c.deletions = [a2], c.flags |= 16) : f3.push(a2);
      }
    }
    function t(c, a2) {
      if (!e)
        return null;
      for (; a2 !== null; )
        n12(c, a2), a2 = a2.sibling;
      return null;
    }
    function r2(c, a2) {
      for (c = /* @__PURE__ */ new Map(); a2 !== null; )
        a2.key !== null ? c.set(a2.key, a2) : c.set(a2.index, a2), a2 = a2.sibling;
      return c;
    }
    function l2(c, a2) {
      return c = nn2(c, a2), c.index = 0, c.sibling = null, c;
    }
    function i(c, a2, f3) {
      return c.index = f3, e ? (f3 = c.alternate, f3 !== null ? (f3 = f3.index, f3 < a2 ? (c.flags |= 2, a2) : f3) : (c.flags |= 2, a2)) : (c.flags |= 1048576, a2);
    }
    function u2(c) {
      return e && c.alternate === null && (c.flags |= 2), c;
    }
    function o(c, a2, f3, y3) {
      return a2 === null || a2.tag !== 6 ? (a2 = Cl2(f3, c.mode, y3), a2.return = c, a2) : (a2 = l2(a2, f3), a2.return = c, a2);
    }
    function s2(c, a2, f3, y3) {
      var E4 = f3.type;
      return E4 === zn2 ? m3(c, a2, f3.props.children, y3, f3.key) : a2 !== null && (a2.elementType === E4 || typeof E4 == "object" && E4 !== null && E4.$$typeof === He3 && Zu(E4) === a2.type) ? (y3 = l2(a2, f3.props), y3.ref = rt2(c, a2, f3), y3.return = c, y3) : (y3 = hr2(f3.type, f3.key, f3.props, null, c.mode, y3), y3.ref = rt2(c, a2, f3), y3.return = c, y3);
    }
    function d3(c, a2, f3, y3) {
      return a2 === null || a2.tag !== 4 || a2.stateNode.containerInfo !== f3.containerInfo || a2.stateNode.implementation !== f3.implementation ? (a2 = xl2(f3, c.mode, y3), a2.return = c, a2) : (a2 = l2(a2, f3.children || []), a2.return = c, a2);
    }
    function m3(c, a2, f3, y3, E4) {
      return a2 === null || a2.tag !== 7 ? (a2 = vn2(f3, c.mode, y3, E4), a2.return = c, a2) : (a2 = l2(a2, f3), a2.return = c, a2);
    }
    function h2(c, a2, f3) {
      if (typeof a2 == "string" && a2 !== "" || typeof a2 == "number")
        return a2 = Cl2("" + a2, c.mode, f3), a2.return = c, a2;
      if (typeof a2 == "object" && a2 !== null) {
        switch (a2.$$typeof) {
          case Bt2:
            return f3 = hr2(a2.type, a2.key, a2.props, null, c.mode, f3), f3.ref = rt2(c, null, a2), f3.return = c, f3;
          case _n2:
            return a2 = xl2(a2, c.mode, f3), a2.return = c, a2;
          case He3:
            var y3 = a2._init;
            return h2(c, y3(a2._payload), f3);
        }
        if (st2(a2) || bn2(a2))
          return a2 = vn2(a2, c.mode, f3, null), a2.return = c, a2;
        tr2(c, a2);
      }
      return null;
    }
    function p3(c, a2, f3, y3) {
      var E4 = a2 !== null ? a2.key : null;
      if (typeof f3 == "string" && f3 !== "" || typeof f3 == "number")
        return E4 !== null ? null : o(c, a2, "" + f3, y3);
      if (typeof f3 == "object" && f3 !== null) {
        switch (f3.$$typeof) {
          case Bt2:
            return f3.key === E4 ? s2(c, a2, f3, y3) : null;
          case _n2:
            return f3.key === E4 ? d3(c, a2, f3, y3) : null;
          case He3:
            return E4 = f3._init, p3(c, a2, E4(f3._payload), y3);
        }
        if (st2(f3) || bn2(f3))
          return E4 !== null ? null : m3(c, a2, f3, y3, null);
        tr2(c, f3);
      }
      return null;
    }
    function g2(c, a2, f3, y3, E4) {
      if (typeof y3 == "string" && y3 !== "" || typeof y3 == "number")
        return c = c.get(f3) || null, o(a2, c, "" + y3, E4);
      if (typeof y3 == "object" && y3 !== null) {
        switch (y3.$$typeof) {
          case Bt2:
            return c = c.get(y3.key === null ? f3 : y3.key) || null, s2(a2, c, y3, E4);
          case _n2:
            return c = c.get(y3.key === null ? f3 : y3.key) || null, d3(a2, c, y3, E4);
          case He3:
            var C2 = y3._init;
            return g2(c, a2, f3, C2(y3._payload), E4);
        }
        if (st2(y3) || bn2(y3))
          return c = c.get(f3) || null, m3(a2, c, y3, E4, null);
        tr2(a2, y3);
      }
      return null;
    }
    function S2(c, a2, f3, y3) {
      for (var E4 = null, C2 = null, x3 = a2, N3 = a2 = 0, H3 = null; x3 !== null && N3 < f3.length; N3++) {
        x3.index > N3 ? (H3 = x3, x3 = null) : H3 = x3.sibling;
        var z3 = p3(c, x3, f3[N3], y3);
        if (z3 === null) {
          x3 === null && (x3 = H3);
          break;
        }
        e && x3 && z3.alternate === null && n12(c, x3), a2 = i(z3, a2, N3), C2 === null ? E4 = z3 : C2.sibling = z3, C2 = z3, x3 = H3;
      }
      if (N3 === f3.length)
        return t(c, x3), D2 && cn2(c, N3), E4;
      if (x3 === null) {
        for (; N3 < f3.length; N3++)
          x3 = h2(c, f3[N3], y3), x3 !== null && (a2 = i(x3, a2, N3), C2 === null ? E4 = x3 : C2.sibling = x3, C2 = x3);
        return D2 && cn2(c, N3), E4;
      }
      for (x3 = r2(c, x3); N3 < f3.length; N3++)
        H3 = g2(x3, c, N3, f3[N3], y3), H3 !== null && (e && H3.alternate !== null && x3.delete(H3.key === null ? N3 : H3.key), a2 = i(H3, a2, N3), C2 === null ? E4 = H3 : C2.sibling = H3, C2 = H3);
      return e && x3.forEach(function(Ae3) {
        return n12(c, Ae3);
      }), D2 && cn2(c, N3), E4;
    }
    function k(c, a2, f3, y3) {
      var E4 = bn2(f3);
      if (typeof E4 != "function")
        throw Error(v3(150));
      if (f3 = E4.call(f3), f3 == null)
        throw Error(v3(151));
      for (var C2 = E4 = null, x3 = a2, N3 = a2 = 0, H3 = null, z3 = f3.next(); x3 !== null && !z3.done; N3++, z3 = f3.next()) {
        x3.index > N3 ? (H3 = x3, x3 = null) : H3 = x3.sibling;
        var Ae3 = p3(c, x3, z3.value, y3);
        if (Ae3 === null) {
          x3 === null && (x3 = H3);
          break;
        }
        e && x3 && Ae3.alternate === null && n12(c, x3), a2 = i(Ae3, a2, N3), C2 === null ? E4 = Ae3 : C2.sibling = Ae3, C2 = Ae3, x3 = H3;
      }
      if (z3.done)
        return t(c, x3), D2 && cn2(c, N3), E4;
      if (x3 === null) {
        for (; !z3.done; N3++, z3 = f3.next())
          z3 = h2(c, z3.value, y3), z3 !== null && (a2 = i(z3, a2, N3), C2 === null ? E4 = z3 : C2.sibling = z3, C2 = z3);
        return D2 && cn2(c, N3), E4;
      }
      for (x3 = r2(c, x3); !z3.done; N3++, z3 = f3.next())
        z3 = g2(x3, c, N3, z3.value, y3), z3 !== null && (e && z3.alternate !== null && x3.delete(z3.key === null ? N3 : z3.key), a2 = i(z3, a2, N3), C2 === null ? E4 = z3 : C2.sibling = z3, C2 = z3);
      return e && x3.forEach(function(Ea2) {
        return n12(c, Ea2);
      }), D2 && cn2(c, N3), E4;
    }
    function U3(c, a2, f3, y3) {
      if (typeof f3 == "object" && f3 !== null && f3.type === zn2 && f3.key === null && (f3 = f3.props.children), typeof f3 == "object" && f3 !== null) {
        switch (f3.$$typeof) {
          case Bt2:
            e: {
              for (var E4 = f3.key, C2 = a2; C2 !== null; ) {
                if (C2.key === E4) {
                  if (E4 = f3.type, E4 === zn2) {
                    if (C2.tag === 7) {
                      t(c, C2.sibling), a2 = l2(C2, f3.props.children), a2.return = c, c = a2;
                      break e;
                    }
                  } else if (C2.elementType === E4 || typeof E4 == "object" && E4 !== null && E4.$$typeof === He3 && Zu(E4) === C2.type) {
                    t(c, C2.sibling), a2 = l2(C2, f3.props), a2.ref = rt2(c, C2, f3), a2.return = c, c = a2;
                    break e;
                  }
                  t(c, C2);
                  break;
                } else
                  n12(c, C2);
                C2 = C2.sibling;
              }
              f3.type === zn2 ? (a2 = vn2(f3.props.children, c.mode, y3, f3.key), a2.return = c, c = a2) : (y3 = hr2(f3.type, f3.key, f3.props, null, c.mode, y3), y3.ref = rt2(c, a2, f3), y3.return = c, c = y3);
            }
            return u2(c);
          case _n2:
            e: {
              for (C2 = f3.key; a2 !== null; ) {
                if (a2.key === C2)
                  if (a2.tag === 4 && a2.stateNode.containerInfo === f3.containerInfo && a2.stateNode.implementation === f3.implementation) {
                    t(c, a2.sibling), a2 = l2(a2, f3.children || []), a2.return = c, c = a2;
                    break e;
                  } else {
                    t(c, a2);
                    break;
                  }
                else
                  n12(c, a2);
                a2 = a2.sibling;
              }
              a2 = xl2(f3, c.mode, y3), a2.return = c, c = a2;
            }
            return u2(c);
          case He3:
            return C2 = f3._init, U3(c, a2, C2(f3._payload), y3);
        }
        if (st2(f3))
          return S2(c, a2, f3, y3);
        if (bn2(f3))
          return k(c, a2, f3, y3);
        tr2(c, f3);
      }
      return typeof f3 == "string" && f3 !== "" || typeof f3 == "number" ? (f3 = "" + f3, a2 !== null && a2.tag === 6 ? (t(c, a2.sibling), a2 = l2(a2, f3), a2.return = c, c = a2) : (t(c, a2), a2 = Cl2(f3, c.mode, y3), a2.return = c, c = a2), u2(c)) : t(c, a2);
    }
    return U3;
  }
  var Yn2 = Es2(true), Cs2 = Es2(false), At2 = {}, Le3 = un2(At2), Mt2 = un2(At2), Dt2 = un2(At2);
  function mn2(e) {
    if (e === At2)
      throw Error(v3(174));
    return e;
  }
  function Bi2(e, n12) {
    switch (L(Dt2, n12), L(Mt2, e), L(Le3, At2), e = n12.nodeType, e) {
      case 9:
      case 11:
        n12 = (n12 = n12.documentElement) ? n12.namespaceURI : Rl2(null, "");
        break;
      default:
        e = e === 8 ? n12.parentNode : n12, n12 = e.namespaceURI || null, e = e.tagName, n12 = Rl2(n12, e);
    }
    M2(Le3), L(Le3, n12);
  }
  function Xn2() {
    M2(Le3), M2(Mt2), M2(Dt2);
  }
  function xs2(e) {
    mn2(Dt2.current);
    var n12 = mn2(Le3.current), t = Rl2(n12, e.type);
    n12 !== t && (L(Mt2, e), L(Le3, t));
  }
  function Hi2(e) {
    Mt2.current === e && (M2(Le3), M2(Mt2));
  }
  var O5 = un2(0);
  function Dr2(e) {
    for (var n12 = e; n12 !== null; ) {
      if (n12.tag === 13) {
        var t = n12.memoizedState;
        if (t !== null && (t = t.dehydrated, t === null || t.data === "$?" || t.data === "$!"))
          return n12;
      } else if (n12.tag === 19 && n12.memoizedProps.revealOrder !== void 0) {
        if (n12.flags & 128)
          return n12;
      } else if (n12.child !== null) {
        n12.child.return = n12, n12 = n12.child;
        continue;
      }
      if (n12 === e)
        break;
      for (; n12.sibling === null; ) {
        if (n12.return === null || n12.return === e)
          return null;
        n12 = n12.return;
      }
      n12.sibling.return = n12.return, n12 = n12.sibling;
    }
    return null;
  }
  var yl2 = [];
  function Wi2() {
    for (var e = 0; e < yl2.length; e++)
      yl2[e]._workInProgressVersionPrimary = null;
    yl2.length = 0;
  }
  var cr2 = Ve3.ReactCurrentDispatcher, gl2 = Ve3.ReactCurrentBatchConfig, wn2 = 0, R = null, A3 = null, W4 = null, Or2 = false, vt2 = false, Ot2 = 0, Jc = 0;
  function X() {
    throw Error(v3(321));
  }
  function Qi2(e, n12) {
    if (n12 === null)
      return false;
    for (var t = 0; t < n12.length && t < e.length; t++)
      if (!xe4(e[t], n12[t]))
        return false;
    return true;
  }
  function $i2(e, n12, t, r2, l2, i) {
    if (wn2 = i, R = n12, n12.memoizedState = null, n12.updateQueue = null, n12.lanes = 0, cr2.current = e === null || e.memoizedState === null ? nf : tf, e = t(r2, l2), vt2) {
      i = 0;
      do {
        if (vt2 = false, Ot2 = 0, 25 <= i)
          throw Error(v3(301));
        i += 1, W4 = A3 = null, n12.updateQueue = null, cr2.current = rf, e = t(r2, l2);
      } while (vt2);
    }
    if (cr2.current = Rr2, n12 = A3 !== null && A3.next !== null, wn2 = 0, W4 = A3 = R = null, Or2 = false, n12)
      throw Error(v3(300));
    return e;
  }
  function Ki2() {
    var e = Ot2 !== 0;
    return Ot2 = 0, e;
  }
  function _e4() {
    var e = { memoizedState: null, baseState: null, baseQueue: null, queue: null, next: null };
    return W4 === null ? R.memoizedState = W4 = e : W4 = W4.next = e, W4;
  }
  function ye4() {
    if (A3 === null) {
      var e = R.alternate;
      e = e !== null ? e.memoizedState : null;
    } else
      e = A3.next;
    var n12 = W4 === null ? R.memoizedState : W4.next;
    if (n12 !== null)
      W4 = n12, A3 = e;
    else {
      if (e === null)
        throw Error(v3(310));
      A3 = e, e = { memoizedState: A3.memoizedState, baseState: A3.baseState, baseQueue: A3.baseQueue, queue: A3.queue, next: null }, W4 === null ? R.memoizedState = W4 = e : W4 = W4.next = e;
    }
    return W4;
  }
  function Rt2(e, n12) {
    return typeof n12 == "function" ? n12(e) : n12;
  }
  function wl2(e) {
    var n12 = ye4(), t = n12.queue;
    if (t === null)
      throw Error(v3(311));
    t.lastRenderedReducer = e;
    var r2 = A3, l2 = r2.baseQueue, i = t.pending;
    if (i !== null) {
      if (l2 !== null) {
        var u2 = l2.next;
        l2.next = i.next, i.next = u2;
      }
      r2.baseQueue = l2 = i, t.pending = null;
    }
    if (l2 !== null) {
      i = l2.next, r2 = r2.baseState;
      var o = u2 = null, s2 = null, d3 = i;
      do {
        var m3 = d3.lane;
        if ((wn2 & m3) === m3)
          s2 !== null && (s2 = s2.next = { lane: 0, action: d3.action, hasEagerState: d3.hasEagerState, eagerState: d3.eagerState, next: null }), r2 = d3.hasEagerState ? d3.eagerState : e(r2, d3.action);
        else {
          var h2 = { lane: m3, action: d3.action, hasEagerState: d3.hasEagerState, eagerState: d3.eagerState, next: null };
          s2 === null ? (o = s2 = h2, u2 = r2) : s2 = s2.next = h2, R.lanes |= m3, Sn2 |= m3;
        }
        d3 = d3.next;
      } while (d3 !== null && d3 !== i);
      s2 === null ? u2 = r2 : s2.next = o, xe4(r2, n12.memoizedState) || (te3 = true), n12.memoizedState = r2, n12.baseState = u2, n12.baseQueue = s2, t.lastRenderedState = r2;
    }
    if (e = t.interleaved, e !== null) {
      l2 = e;
      do
        i = l2.lane, R.lanes |= i, Sn2 |= i, l2 = l2.next;
      while (l2 !== e);
    } else
      l2 === null && (t.lanes = 0);
    return [n12.memoizedState, t.dispatch];
  }
  function Sl2(e) {
    var n12 = ye4(), t = n12.queue;
    if (t === null)
      throw Error(v3(311));
    t.lastRenderedReducer = e;
    var r2 = t.dispatch, l2 = t.pending, i = n12.memoizedState;
    if (l2 !== null) {
      t.pending = null;
      var u2 = l2 = l2.next;
      do
        i = e(i, u2.action), u2 = u2.next;
      while (u2 !== l2);
      xe4(i, n12.memoizedState) || (te3 = true), n12.memoizedState = i, n12.baseQueue === null && (n12.baseState = i), t.lastRenderedState = i;
    }
    return [i, r2];
  }
  function Ns2() {
  }
  function _s2(e, n12) {
    var t = R, r2 = ye4(), l2 = n12(), i = !xe4(r2.memoizedState, l2);
    if (i && (r2.memoizedState = l2, te3 = true), r2 = r2.queue, Yi2(Ls2.bind(null, t, r2, e), [e]), r2.getSnapshot !== n12 || i || W4 !== null && W4.memoizedState.tag & 1) {
      if (t.flags |= 2048, Ft2(9, Ps2.bind(null, t, r2, l2, n12), void 0, null), Q2 === null)
        throw Error(v3(349));
      wn2 & 30 || zs2(t, n12, l2);
    }
    return l2;
  }
  function zs2(e, n12, t) {
    e.flags |= 16384, e = { getSnapshot: n12, value: t }, n12 = R.updateQueue, n12 === null ? (n12 = { lastEffect: null, stores: null }, R.updateQueue = n12, n12.stores = [e]) : (t = n12.stores, t === null ? n12.stores = [e] : t.push(e));
  }
  function Ps2(e, n12, t, r2) {
    n12.value = t, n12.getSnapshot = r2, Ts2(n12) && Ms2(e);
  }
  function Ls2(e, n12, t) {
    return t(function() {
      Ts2(n12) && Ms2(e);
    });
  }
  function Ts2(e) {
    var n12 = e.getSnapshot;
    e = e.value;
    try {
      var t = n12();
      return !xe4(e, t);
    } catch {
      return true;
    }
  }
  function Ms2(e) {
    var n12 = Ue3(e, 1);
    n12 !== null && Ce4(n12, e, 1, -1);
  }
  function Ju(e) {
    var n12 = _e4();
    return typeof e == "function" && (e = e()), n12.memoizedState = n12.baseState = e, e = { pending: null, interleaved: null, lanes: 0, dispatch: null, lastRenderedReducer: Rt2, lastRenderedState: e }, n12.queue = e, e = e.dispatch = ef.bind(null, R, e), [n12.memoizedState, e];
  }
  function Ft2(e, n12, t, r2) {
    return e = { tag: e, create: n12, destroy: t, deps: r2, next: null }, n12 = R.updateQueue, n12 === null ? (n12 = { lastEffect: null, stores: null }, R.updateQueue = n12, n12.lastEffect = e.next = e) : (t = n12.lastEffect, t === null ? n12.lastEffect = e.next = e : (r2 = t.next, t.next = e, e.next = r2, n12.lastEffect = e)), e;
  }
  function Ds2() {
    return ye4().memoizedState;
  }
  function fr2(e, n12, t, r2) {
    var l2 = _e4();
    R.flags |= e, l2.memoizedState = Ft2(1 | n12, t, void 0, r2 === void 0 ? null : r2);
  }
  function Yr2(e, n12, t, r2) {
    var l2 = ye4();
    r2 = r2 === void 0 ? null : r2;
    var i = void 0;
    if (A3 !== null) {
      var u2 = A3.memoizedState;
      if (i = u2.destroy, r2 !== null && Qi2(r2, u2.deps)) {
        l2.memoizedState = Ft2(n12, t, i, r2);
        return;
      }
    }
    R.flags |= e, l2.memoizedState = Ft2(1 | n12, t, i, r2);
  }
  function qu(e, n12) {
    return fr2(8390656, 8, e, n12);
  }
  function Yi2(e, n12) {
    return Yr2(2048, 8, e, n12);
  }
  function Os2(e, n12) {
    return Yr2(4, 2, e, n12);
  }
  function Rs2(e, n12) {
    return Yr2(4, 4, e, n12);
  }
  function Fs2(e, n12) {
    if (typeof n12 == "function")
      return e = e(), n12(e), function() {
        n12(null);
      };
    if (n12 != null)
      return e = e(), n12.current = e, function() {
        n12.current = null;
      };
  }
  function Is2(e, n12, t) {
    return t = t != null ? t.concat([e]) : null, Yr2(4, 4, Fs2.bind(null, n12, e), t);
  }
  function Xi2() {
  }
  function Us2(e, n12) {
    var t = ye4();
    n12 = n12 === void 0 ? null : n12;
    var r2 = t.memoizedState;
    return r2 !== null && n12 !== null && Qi2(n12, r2[1]) ? r2[0] : (t.memoizedState = [e, n12], e);
  }
  function js2(e, n12) {
    var t = ye4();
    n12 = n12 === void 0 ? null : n12;
    var r2 = t.memoizedState;
    return r2 !== null && n12 !== null && Qi2(n12, r2[1]) ? r2[0] : (e = e(), t.memoizedState = [e, n12], e);
  }
  function Vs2(e, n12, t) {
    return wn2 & 21 ? (xe4(t, n12) || (t = Ho2(), R.lanes |= t, Sn2 |= t, e.baseState = true), n12) : (e.baseState && (e.baseState = false, te3 = true), e.memoizedState = t);
  }
  function qc(e, n12) {
    var t = P2;
    P2 = t !== 0 && 4 > t ? t : 4, e(true);
    var r2 = gl2.transition;
    gl2.transition = {};
    try {
      e(false), n12();
    } finally {
      P2 = t, gl2.transition = r2;
    }
  }
  function As2() {
    return ye4().memoizedState;
  }
  function bc(e, n12, t) {
    var r2 = en2(e);
    if (t = { lane: r2, action: t, hasEagerState: false, eagerState: null, next: null }, Bs2(e))
      Hs2(n12, t);
    else if (t = gs2(e, n12, t, r2), t !== null) {
      var l2 = b();
      Ce4(t, e, r2, l2), Ws2(t, n12, r2);
    }
  }
  function ef(e, n12, t) {
    var r2 = en2(e), l2 = { lane: r2, action: t, hasEagerState: false, eagerState: null, next: null };
    if (Bs2(e))
      Hs2(n12, l2);
    else {
      var i = e.alternate;
      if (e.lanes === 0 && (i === null || i.lanes === 0) && (i = n12.lastRenderedReducer, i !== null))
        try {
          var u2 = n12.lastRenderedState, o = i(u2, t);
          if (l2.hasEagerState = true, l2.eagerState = o, xe4(o, u2)) {
            var s2 = n12.interleaved;
            s2 === null ? (l2.next = l2, Vi2(n12)) : (l2.next = s2.next, s2.next = l2), n12.interleaved = l2;
            return;
          }
        } catch {
        } finally {
        }
      t = gs2(e, n12, l2, r2), t !== null && (l2 = b(), Ce4(t, e, r2, l2), Ws2(t, n12, r2));
    }
  }
  function Bs2(e) {
    var n12 = e.alternate;
    return e === R || n12 !== null && n12 === R;
  }
  function Hs2(e, n12) {
    vt2 = Or2 = true;
    var t = e.pending;
    t === null ? n12.next = n12 : (n12.next = t.next, t.next = n12), e.pending = n12;
  }
  function Ws2(e, n12, t) {
    if (t & 4194240) {
      var r2 = n12.lanes;
      r2 &= e.pendingLanes, t |= r2, n12.lanes = t, Ni2(e, t);
    }
  }
  var Rr2 = { readContext: ve4, useCallback: X, useContext: X, useEffect: X, useImperativeHandle: X, useInsertionEffect: X, useLayoutEffect: X, useMemo: X, useReducer: X, useRef: X, useState: X, useDebugValue: X, useDeferredValue: X, useTransition: X, useMutableSource: X, useSyncExternalStore: X, useId: X, unstable_isNewReconciler: false }, nf = { readContext: ve4, useCallback: function(e, n12) {
    return _e4().memoizedState = [e, n12 === void 0 ? null : n12], e;
  }, useContext: ve4, useEffect: qu, useImperativeHandle: function(e, n12, t) {
    return t = t != null ? t.concat([e]) : null, fr2(4194308, 4, Fs2.bind(null, n12, e), t);
  }, useLayoutEffect: function(e, n12) {
    return fr2(4194308, 4, e, n12);
  }, useInsertionEffect: function(e, n12) {
    return fr2(4, 2, e, n12);
  }, useMemo: function(e, n12) {
    var t = _e4();
    return n12 = n12 === void 0 ? null : n12, e = e(), t.memoizedState = [e, n12], e;
  }, useReducer: function(e, n12, t) {
    var r2 = _e4();
    return n12 = t !== void 0 ? t(n12) : n12, r2.memoizedState = r2.baseState = n12, e = { pending: null, interleaved: null, lanes: 0, dispatch: null, lastRenderedReducer: e, lastRenderedState: n12 }, r2.queue = e, e = e.dispatch = bc.bind(null, R, e), [r2.memoizedState, e];
  }, useRef: function(e) {
    var n12 = _e4();
    return e = { current: e }, n12.memoizedState = e;
  }, useState: Ju, useDebugValue: Xi2, useDeferredValue: function(e) {
    return _e4().memoizedState = e;
  }, useTransition: function() {
    var e = Ju(false), n12 = e[0];
    return e = qc.bind(null, e[1]), _e4().memoizedState = e, [n12, e];
  }, useMutableSource: function() {
  }, useSyncExternalStore: function(e, n12, t) {
    var r2 = R, l2 = _e4();
    if (D2) {
      if (t === void 0)
        throw Error(v3(407));
      t = t();
    } else {
      if (t = n12(), Q2 === null)
        throw Error(v3(349));
      wn2 & 30 || zs2(r2, n12, t);
    }
    l2.memoizedState = t;
    var i = { value: t, getSnapshot: n12 };
    return l2.queue = i, qu(Ls2.bind(null, r2, i, e), [e]), r2.flags |= 2048, Ft2(9, Ps2.bind(null, r2, i, t, n12), void 0, null), t;
  }, useId: function() {
    var e = _e4(), n12 = Q2.identifierPrefix;
    if (D2) {
      var t = Oe3, r2 = De3;
      t = (r2 & ~(1 << 32 - Ee4(r2) - 1)).toString(32) + t, n12 = ":" + n12 + "R" + t, t = Ot2++, 0 < t && (n12 += "H" + t.toString(32)), n12 += ":";
    } else
      t = Jc++, n12 = ":" + n12 + "r" + t.toString(32) + ":";
    return e.memoizedState = n12;
  }, unstable_isNewReconciler: false }, tf = { readContext: ve4, useCallback: Us2, useContext: ve4, useEffect: Yi2, useImperativeHandle: Is2, useInsertionEffect: Os2, useLayoutEffect: Rs2, useMemo: js2, useReducer: wl2, useRef: Ds2, useState: function() {
    return wl2(Rt2);
  }, useDebugValue: Xi2, useDeferredValue: function(e) {
    var n12 = ye4();
    return Vs2(n12, A3.memoizedState, e);
  }, useTransition: function() {
    var e = wl2(Rt2)[0], n12 = ye4().memoizedState;
    return [e, n12];
  }, useMutableSource: Ns2, useSyncExternalStore: _s2, useId: As2, unstable_isNewReconciler: false }, rf = { readContext: ve4, useCallback: Us2, useContext: ve4, useEffect: Yi2, useImperativeHandle: Is2, useInsertionEffect: Os2, useLayoutEffect: Rs2, useMemo: js2, useReducer: Sl2, useRef: Ds2, useState: function() {
    return Sl2(Rt2);
  }, useDebugValue: Xi2, useDeferredValue: function(e) {
    var n12 = ye4();
    return A3 === null ? n12.memoizedState = e : Vs2(n12, A3.memoizedState, e);
  }, useTransition: function() {
    var e = Sl2(Rt2)[0], n12 = ye4().memoizedState;
    return [e, n12];
  }, useMutableSource: Ns2, useSyncExternalStore: _s2, useId: As2, unstable_isNewReconciler: false };
  function Gn2(e, n12) {
    try {
      var t = "", r2 = n12;
      do
        t += Oa2(r2), r2 = r2.return;
      while (r2);
      var l2 = t;
    } catch (i) {
      l2 = `
Error generating stack: ` + i.message + `
` + i.stack;
    }
    return { value: e, source: n12, stack: l2, digest: null };
  }
  function kl2(e, n12, t) {
    return { value: e, source: null, stack: t ?? null, digest: n12 ?? null };
  }
  function ri2(e, n12) {
    try {
      console.error(n12.value);
    } catch (t) {
      setTimeout(function() {
        throw t;
      });
    }
  }
  var lf = typeof WeakMap == "function" ? WeakMap : Map;
  function Qs2(e, n12, t) {
    t = Re3(-1, t), t.tag = 3, t.payload = { element: null };
    var r2 = n12.value;
    return t.callback = function() {
      Ir2 || (Ir2 = true, pi2 = r2), ri2(e, n12);
    }, t;
  }
  function $s2(e, n12, t) {
    t = Re3(-1, t), t.tag = 3;
    var r2 = e.type.getDerivedStateFromError;
    if (typeof r2 == "function") {
      var l2 = n12.value;
      t.payload = function() {
        return r2(l2);
      }, t.callback = function() {
        ri2(e, n12);
      };
    }
    var i = e.stateNode;
    return i !== null && typeof i.componentDidCatch == "function" && (t.callback = function() {
      ri2(e, n12), typeof r2 != "function" && (be4 === null ? be4 = /* @__PURE__ */ new Set([this]) : be4.add(this));
      var u2 = n12.stack;
      this.componentDidCatch(n12.value, { componentStack: u2 !== null ? u2 : "" });
    }), t;
  }
  function bu(e, n12, t) {
    var r2 = e.pingCache;
    if (r2 === null) {
      r2 = e.pingCache = new lf();
      var l2 = /* @__PURE__ */ new Set();
      r2.set(n12, l2);
    } else
      l2 = r2.get(n12), l2 === void 0 && (l2 = /* @__PURE__ */ new Set(), r2.set(n12, l2));
    l2.has(t) || (l2.add(t), e = wf.bind(null, e, n12, t), n12.then(e, e));
  }
  function eo2(e) {
    do {
      var n12;
      if ((n12 = e.tag === 13) && (n12 = e.memoizedState, n12 = n12 !== null ? n12.dehydrated !== null : true), n12)
        return e;
      e = e.return;
    } while (e !== null);
    return null;
  }
  function no2(e, n12, t, r2, l2) {
    return e.mode & 1 ? (e.flags |= 65536, e.lanes = l2, e) : (e === n12 ? e.flags |= 65536 : (e.flags |= 128, t.flags |= 131072, t.flags &= -52805, t.tag === 1 && (t.alternate === null ? t.tag = 17 : (n12 = Re3(-1, 1), n12.tag = 2, qe3(t, n12, 1))), t.lanes |= 1), e);
  }
  var uf = Ve3.ReactCurrentOwner, te3 = false;
  function q(e, n12, t, r2) {
    n12.child = e === null ? Cs2(n12, null, t, r2) : Yn2(n12, e.child, t, r2);
  }
  function to2(e, n12, t, r2, l2) {
    t = t.render;
    var i = n12.ref;
    return Hn2(n12, l2), r2 = $i2(e, n12, t, r2, i, l2), t = Ki2(), e !== null && !te3 ? (n12.updateQueue = e.updateQueue, n12.flags &= -2053, e.lanes &= ~l2, je3(e, n12, l2)) : (D2 && t && Oi2(n12), n12.flags |= 1, q(e, n12, r2, l2), n12.child);
  }
  function ro2(e, n12, t, r2, l2) {
    if (e === null) {
      var i = t.type;
      return typeof i == "function" && !tu(i) && i.defaultProps === void 0 && t.compare === null && t.defaultProps === void 0 ? (n12.tag = 15, n12.type = i, Ks2(e, n12, i, r2, l2)) : (e = hr2(t.type, null, r2, n12, n12.mode, l2), e.ref = n12.ref, e.return = n12, n12.child = e);
    }
    if (i = e.child, !(e.lanes & l2)) {
      var u2 = i.memoizedProps;
      if (t = t.compare, t = t !== null ? t : zt2, t(u2, r2) && e.ref === n12.ref)
        return je3(e, n12, l2);
    }
    return n12.flags |= 1, e = nn2(i, r2), e.ref = n12.ref, e.return = n12, n12.child = e;
  }
  function Ks2(e, n12, t, r2, l2) {
    if (e !== null) {
      var i = e.memoizedProps;
      if (zt2(i, r2) && e.ref === n12.ref)
        if (te3 = false, n12.pendingProps = r2 = i, (e.lanes & l2) !== 0)
          e.flags & 131072 && (te3 = true);
        else
          return n12.lanes = e.lanes, je3(e, n12, l2);
    }
    return li2(e, n12, t, r2, l2);
  }
  function Ys2(e, n12, t) {
    var r2 = n12.pendingProps, l2 = r2.children, i = e !== null ? e.memoizedState : null;
    if (r2.mode === "hidden")
      if (!(n12.mode & 1))
        n12.memoizedState = { baseLanes: 0, cachePool: null, transitions: null }, L(Un2, ue2), ue2 |= t;
      else {
        if (!(t & 1073741824))
          return e = i !== null ? i.baseLanes | t : t, n12.lanes = n12.childLanes = 1073741824, n12.memoizedState = { baseLanes: e, cachePool: null, transitions: null }, n12.updateQueue = null, L(Un2, ue2), ue2 |= e, null;
        n12.memoizedState = { baseLanes: 0, cachePool: null, transitions: null }, r2 = i !== null ? i.baseLanes : t, L(Un2, ue2), ue2 |= r2;
      }
    else
      i !== null ? (r2 = i.baseLanes | t, n12.memoizedState = null) : r2 = t, L(Un2, ue2), ue2 |= r2;
    return q(e, n12, l2, t), n12.child;
  }
  function Xs2(e, n12) {
    var t = n12.ref;
    (e === null && t !== null || e !== null && e.ref !== t) && (n12.flags |= 512, n12.flags |= 2097152);
  }
  function li2(e, n12, t, r2, l2) {
    var i = le4(t) ? yn2 : J2.current;
    return i = $n2(n12, i), Hn2(n12, l2), t = $i2(e, n12, t, r2, i, l2), r2 = Ki2(), e !== null && !te3 ? (n12.updateQueue = e.updateQueue, n12.flags &= -2053, e.lanes &= ~l2, je3(e, n12, l2)) : (D2 && r2 && Oi2(n12), n12.flags |= 1, q(e, n12, t, l2), n12.child);
  }
  function lo2(e, n12, t, r2, l2) {
    if (le4(t)) {
      var i = true;
      _r2(n12);
    } else
      i = false;
    if (Hn2(n12, l2), n12.stateNode === null)
      dr2(e, n12), ks2(n12, t, r2), ti2(n12, t, r2, l2), r2 = true;
    else if (e === null) {
      var u2 = n12.stateNode, o = n12.memoizedProps;
      u2.props = o;
      var s2 = u2.context, d3 = t.contextType;
      typeof d3 == "object" && d3 !== null ? d3 = ve4(d3) : (d3 = le4(t) ? yn2 : J2.current, d3 = $n2(n12, d3));
      var m3 = t.getDerivedStateFromProps, h2 = typeof m3 == "function" || typeof u2.getSnapshotBeforeUpdate == "function";
      h2 || typeof u2.UNSAFE_componentWillReceiveProps != "function" && typeof u2.componentWillReceiveProps != "function" || (o !== r2 || s2 !== d3) && Gu(n12, u2, r2, d3), We3 = false;
      var p3 = n12.memoizedState;
      u2.state = p3, Mr2(n12, r2, u2, l2), s2 = n12.memoizedState, o !== r2 || p3 !== s2 || re2.current || We3 ? (typeof m3 == "function" && (ni2(n12, t, m3, r2), s2 = n12.memoizedState), (o = We3 || Xu(n12, t, o, r2, p3, s2, d3)) ? (h2 || typeof u2.UNSAFE_componentWillMount != "function" && typeof u2.componentWillMount != "function" || (typeof u2.componentWillMount == "function" && u2.componentWillMount(), typeof u2.UNSAFE_componentWillMount == "function" && u2.UNSAFE_componentWillMount()), typeof u2.componentDidMount == "function" && (n12.flags |= 4194308)) : (typeof u2.componentDidMount == "function" && (n12.flags |= 4194308), n12.memoizedProps = r2, n12.memoizedState = s2), u2.props = r2, u2.state = s2, u2.context = d3, r2 = o) : (typeof u2.componentDidMount == "function" && (n12.flags |= 4194308), r2 = false);
    } else {
      u2 = n12.stateNode, ws2(e, n12), o = n12.memoizedProps, d3 = n12.type === n12.elementType ? o : we4(n12.type, o), u2.props = d3, h2 = n12.pendingProps, p3 = u2.context, s2 = t.contextType, typeof s2 == "object" && s2 !== null ? s2 = ve4(s2) : (s2 = le4(t) ? yn2 : J2.current, s2 = $n2(n12, s2));
      var g2 = t.getDerivedStateFromProps;
      (m3 = typeof g2 == "function" || typeof u2.getSnapshotBeforeUpdate == "function") || typeof u2.UNSAFE_componentWillReceiveProps != "function" && typeof u2.componentWillReceiveProps != "function" || (o !== h2 || p3 !== s2) && Gu(n12, u2, r2, s2), We3 = false, p3 = n12.memoizedState, u2.state = p3, Mr2(n12, r2, u2, l2);
      var S2 = n12.memoizedState;
      o !== h2 || p3 !== S2 || re2.current || We3 ? (typeof g2 == "function" && (ni2(n12, t, g2, r2), S2 = n12.memoizedState), (d3 = We3 || Xu(n12, t, d3, r2, p3, S2, s2) || false) ? (m3 || typeof u2.UNSAFE_componentWillUpdate != "function" && typeof u2.componentWillUpdate != "function" || (typeof u2.componentWillUpdate == "function" && u2.componentWillUpdate(r2, S2, s2), typeof u2.UNSAFE_componentWillUpdate == "function" && u2.UNSAFE_componentWillUpdate(r2, S2, s2)), typeof u2.componentDidUpdate == "function" && (n12.flags |= 4), typeof u2.getSnapshotBeforeUpdate == "function" && (n12.flags |= 1024)) : (typeof u2.componentDidUpdate != "function" || o === e.memoizedProps && p3 === e.memoizedState || (n12.flags |= 4), typeof u2.getSnapshotBeforeUpdate != "function" || o === e.memoizedProps && p3 === e.memoizedState || (n12.flags |= 1024), n12.memoizedProps = r2, n12.memoizedState = S2), u2.props = r2, u2.state = S2, u2.context = s2, r2 = d3) : (typeof u2.componentDidUpdate != "function" || o === e.memoizedProps && p3 === e.memoizedState || (n12.flags |= 4), typeof u2.getSnapshotBeforeUpdate != "function" || o === e.memoizedProps && p3 === e.memoizedState || (n12.flags |= 1024), r2 = false);
    }
    return ii2(e, n12, t, r2, i, l2);
  }
  function ii2(e, n12, t, r2, l2, i) {
    Xs2(e, n12);
    var u2 = (n12.flags & 128) !== 0;
    if (!r2 && !u2)
      return l2 && Wu(n12, t, false), je3(e, n12, i);
    r2 = n12.stateNode, uf.current = n12;
    var o = u2 && typeof t.getDerivedStateFromError != "function" ? null : r2.render();
    return n12.flags |= 1, e !== null && u2 ? (n12.child = Yn2(n12, e.child, null, i), n12.child = Yn2(n12, null, o, i)) : q(e, n12, o, i), n12.memoizedState = r2.state, l2 && Wu(n12, t, true), n12.child;
  }
  function Gs2(e) {
    var n12 = e.stateNode;
    n12.pendingContext ? Hu(e, n12.pendingContext, n12.pendingContext !== n12.context) : n12.context && Hu(e, n12.context, false), Bi2(e, n12.containerInfo);
  }
  function io2(e, n12, t, r2, l2) {
    return Kn2(), Fi2(l2), n12.flags |= 256, q(e, n12, t, r2), n12.child;
  }
  var ui2 = { dehydrated: null, treeContext: null, retryLane: 0 };
  function oi2(e) {
    return { baseLanes: e, cachePool: null, transitions: null };
  }
  function Zs2(e, n12, t) {
    var r2 = n12.pendingProps, l2 = O5.current, i = false, u2 = (n12.flags & 128) !== 0, o;
    if ((o = u2) || (o = e !== null && e.memoizedState === null ? false : (l2 & 2) !== 0), o ? (i = true, n12.flags &= -129) : (e === null || e.memoizedState !== null) && (l2 |= 1), L(O5, l2 & 1), e === null)
      return bl2(n12), e = n12.memoizedState, e !== null && (e = e.dehydrated, e !== null) ? (n12.mode & 1 ? e.data === "$!" ? n12.lanes = 8 : n12.lanes = 1073741824 : n12.lanes = 1, null) : (u2 = r2.children, e = r2.fallback, i ? (r2 = n12.mode, i = n12.child, u2 = { mode: "hidden", children: u2 }, !(r2 & 1) && i !== null ? (i.childLanes = 0, i.pendingProps = u2) : i = Zr2(u2, r2, 0, null), e = vn2(e, r2, t, null), i.return = n12, e.return = n12, i.sibling = e, n12.child = i, n12.child.memoizedState = oi2(t), n12.memoizedState = ui2, e) : Gi2(n12, u2));
    if (l2 = e.memoizedState, l2 !== null && (o = l2.dehydrated, o !== null))
      return of(e, n12, u2, r2, o, l2, t);
    if (i) {
      i = r2.fallback, u2 = n12.mode, l2 = e.child, o = l2.sibling;
      var s2 = { mode: "hidden", children: r2.children };
      return !(u2 & 1) && n12.child !== l2 ? (r2 = n12.child, r2.childLanes = 0, r2.pendingProps = s2, n12.deletions = null) : (r2 = nn2(l2, s2), r2.subtreeFlags = l2.subtreeFlags & 14680064), o !== null ? i = nn2(o, i) : (i = vn2(i, u2, t, null), i.flags |= 2), i.return = n12, r2.return = n12, r2.sibling = i, n12.child = r2, r2 = i, i = n12.child, u2 = e.child.memoizedState, u2 = u2 === null ? oi2(t) : { baseLanes: u2.baseLanes | t, cachePool: null, transitions: u2.transitions }, i.memoizedState = u2, i.childLanes = e.childLanes & ~t, n12.memoizedState = ui2, r2;
    }
    return i = e.child, e = i.sibling, r2 = nn2(i, { mode: "visible", children: r2.children }), !(n12.mode & 1) && (r2.lanes = t), r2.return = n12, r2.sibling = null, e !== null && (t = n12.deletions, t === null ? (n12.deletions = [e], n12.flags |= 16) : t.push(e)), n12.child = r2, n12.memoizedState = null, r2;
  }
  function Gi2(e, n12) {
    return n12 = Zr2({ mode: "visible", children: n12 }, e.mode, 0, null), n12.return = e, e.child = n12;
  }
  function rr2(e, n12, t, r2) {
    return r2 !== null && Fi2(r2), Yn2(n12, e.child, null, t), e = Gi2(n12, n12.pendingProps.children), e.flags |= 2, n12.memoizedState = null, e;
  }
  function of(e, n12, t, r2, l2, i, u2) {
    if (t)
      return n12.flags & 256 ? (n12.flags &= -257, r2 = kl2(Error(v3(422))), rr2(e, n12, u2, r2)) : n12.memoizedState !== null ? (n12.child = e.child, n12.flags |= 128, null) : (i = r2.fallback, l2 = n12.mode, r2 = Zr2({ mode: "visible", children: r2.children }, l2, 0, null), i = vn2(i, l2, u2, null), i.flags |= 2, r2.return = n12, i.return = n12, r2.sibling = i, n12.child = r2, n12.mode & 1 && Yn2(n12, e.child, null, u2), n12.child.memoizedState = oi2(u2), n12.memoizedState = ui2, i);
    if (!(n12.mode & 1))
      return rr2(e, n12, u2, null);
    if (l2.data === "$!") {
      if (r2 = l2.nextSibling && l2.nextSibling.dataset, r2)
        var o = r2.dgst;
      return r2 = o, i = Error(v3(419)), r2 = kl2(i, r2, void 0), rr2(e, n12, u2, r2);
    }
    if (o = (u2 & e.childLanes) !== 0, te3 || o) {
      if (r2 = Q2, r2 !== null) {
        switch (u2 & -u2) {
          case 4:
            l2 = 2;
            break;
          case 16:
            l2 = 8;
            break;
          case 64:
          case 128:
          case 256:
          case 512:
          case 1024:
          case 2048:
          case 4096:
          case 8192:
          case 16384:
          case 32768:
          case 65536:
          case 131072:
          case 262144:
          case 524288:
          case 1048576:
          case 2097152:
          case 4194304:
          case 8388608:
          case 16777216:
          case 33554432:
          case 67108864:
            l2 = 32;
            break;
          case 536870912:
            l2 = 268435456;
            break;
          default:
            l2 = 0;
        }
        l2 = l2 & (r2.suspendedLanes | u2) ? 0 : l2, l2 !== 0 && l2 !== i.retryLane && (i.retryLane = l2, Ue3(e, l2), Ce4(r2, e, l2, -1));
      }
      return nu(), r2 = kl2(Error(v3(421))), rr2(e, n12, u2, r2);
    }
    return l2.data === "$?" ? (n12.flags |= 128, n12.child = e.child, n12 = Sf.bind(null, e), l2._reactRetry = n12, null) : (e = i.treeContext, oe3 = Je2(l2.nextSibling), se3 = n12, D2 = true, ke4 = null, e !== null && (de4[pe4++] = De3, de4[pe4++] = Oe3, de4[pe4++] = gn2, De3 = e.id, Oe3 = e.overflow, gn2 = n12), n12 = Gi2(n12, r2.children), n12.flags |= 4096, n12);
  }
  function uo2(e, n12, t) {
    e.lanes |= n12;
    var r2 = e.alternate;
    r2 !== null && (r2.lanes |= n12), ei2(e.return, n12, t);
  }
  function El2(e, n12, t, r2, l2) {
    var i = e.memoizedState;
    i === null ? e.memoizedState = { isBackwards: n12, rendering: null, renderingStartTime: 0, last: r2, tail: t, tailMode: l2 } : (i.isBackwards = n12, i.rendering = null, i.renderingStartTime = 0, i.last = r2, i.tail = t, i.tailMode = l2);
  }
  function Js2(e, n12, t) {
    var r2 = n12.pendingProps, l2 = r2.revealOrder, i = r2.tail;
    if (q(e, n12, r2.children, t), r2 = O5.current, r2 & 2)
      r2 = r2 & 1 | 2, n12.flags |= 128;
    else {
      if (e !== null && e.flags & 128)
        e:
          for (e = n12.child; e !== null; ) {
            if (e.tag === 13)
              e.memoizedState !== null && uo2(e, t, n12);
            else if (e.tag === 19)
              uo2(e, t, n12);
            else if (e.child !== null) {
              e.child.return = e, e = e.child;
              continue;
            }
            if (e === n12)
              break e;
            for (; e.sibling === null; ) {
              if (e.return === null || e.return === n12)
                break e;
              e = e.return;
            }
            e.sibling.return = e.return, e = e.sibling;
          }
      r2 &= 1;
    }
    if (L(O5, r2), !(n12.mode & 1))
      n12.memoizedState = null;
    else
      switch (l2) {
        case "forwards":
          for (t = n12.child, l2 = null; t !== null; )
            e = t.alternate, e !== null && Dr2(e) === null && (l2 = t), t = t.sibling;
          t = l2, t === null ? (l2 = n12.child, n12.child = null) : (l2 = t.sibling, t.sibling = null), El2(n12, false, l2, t, i);
          break;
        case "backwards":
          for (t = null, l2 = n12.child, n12.child = null; l2 !== null; ) {
            if (e = l2.alternate, e !== null && Dr2(e) === null) {
              n12.child = l2;
              break;
            }
            e = l2.sibling, l2.sibling = t, t = l2, l2 = e;
          }
          El2(n12, true, t, null, i);
          break;
        case "together":
          El2(n12, false, null, null, void 0);
          break;
        default:
          n12.memoizedState = null;
      }
    return n12.child;
  }
  function dr2(e, n12) {
    !(n12.mode & 1) && e !== null && (e.alternate = null, n12.alternate = null, n12.flags |= 2);
  }
  function je3(e, n12, t) {
    if (e !== null && (n12.dependencies = e.dependencies), Sn2 |= n12.lanes, !(t & n12.childLanes))
      return null;
    if (e !== null && n12.child !== e.child)
      throw Error(v3(153));
    if (n12.child !== null) {
      for (e = n12.child, t = nn2(e, e.pendingProps), n12.child = t, t.return = n12; e.sibling !== null; )
        e = e.sibling, t = t.sibling = nn2(e, e.pendingProps), t.return = n12;
      t.sibling = null;
    }
    return n12.child;
  }
  function sf(e, n12, t) {
    switch (n12.tag) {
      case 3:
        Gs2(n12), Kn2();
        break;
      case 5:
        xs2(n12);
        break;
      case 1:
        le4(n12.type) && _r2(n12);
        break;
      case 4:
        Bi2(n12, n12.stateNode.containerInfo);
        break;
      case 10:
        var r2 = n12.type._context, l2 = n12.memoizedProps.value;
        L(Lr2, r2._currentValue), r2._currentValue = l2;
        break;
      case 13:
        if (r2 = n12.memoizedState, r2 !== null)
          return r2.dehydrated !== null ? (L(O5, O5.current & 1), n12.flags |= 128, null) : t & n12.child.childLanes ? Zs2(e, n12, t) : (L(O5, O5.current & 1), e = je3(e, n12, t), e !== null ? e.sibling : null);
        L(O5, O5.current & 1);
        break;
      case 19:
        if (r2 = (t & n12.childLanes) !== 0, e.flags & 128) {
          if (r2)
            return Js2(e, n12, t);
          n12.flags |= 128;
        }
        if (l2 = n12.memoizedState, l2 !== null && (l2.rendering = null, l2.tail = null, l2.lastEffect = null), L(O5, O5.current), r2)
          break;
        return null;
      case 22:
      case 23:
        return n12.lanes = 0, Ys2(e, n12, t);
    }
    return je3(e, n12, t);
  }
  var qs2, si2, bs2, ea2;
  qs2 = function(e, n12) {
    for (var t = n12.child; t !== null; ) {
      if (t.tag === 5 || t.tag === 6)
        e.appendChild(t.stateNode);
      else if (t.tag !== 4 && t.child !== null) {
        t.child.return = t, t = t.child;
        continue;
      }
      if (t === n12)
        break;
      for (; t.sibling === null; ) {
        if (t.return === null || t.return === n12)
          return;
        t = t.return;
      }
      t.sibling.return = t.return, t = t.sibling;
    }
  };
  si2 = function() {
  };
  bs2 = function(e, n12, t, r2) {
    var l2 = e.memoizedProps;
    if (l2 !== r2) {
      e = n12.stateNode, mn2(Le3.current);
      var i = null;
      switch (t) {
        case "input":
          l2 = Tl2(e, l2), r2 = Tl2(e, r2), i = [];
          break;
        case "select":
          l2 = F2({}, l2, { value: void 0 }), r2 = F2({}, r2, { value: void 0 }), i = [];
          break;
        case "textarea":
          l2 = Ol2(e, l2), r2 = Ol2(e, r2), i = [];
          break;
        default:
          typeof l2.onClick != "function" && typeof r2.onClick == "function" && (e.onclick = xr2);
      }
      Fl2(t, r2);
      var u2;
      t = null;
      for (d3 in l2)
        if (!r2.hasOwnProperty(d3) && l2.hasOwnProperty(d3) && l2[d3] != null)
          if (d3 === "style") {
            var o = l2[d3];
            for (u2 in o)
              o.hasOwnProperty(u2) && (t || (t = {}), t[u2] = "");
          } else
            d3 !== "dangerouslySetInnerHTML" && d3 !== "children" && d3 !== "suppressContentEditableWarning" && d3 !== "suppressHydrationWarning" && d3 !== "autoFocus" && (St2.hasOwnProperty(d3) ? i || (i = []) : (i = i || []).push(d3, null));
      for (d3 in r2) {
        var s2 = r2[d3];
        if (o = l2?.[d3], r2.hasOwnProperty(d3) && s2 !== o && (s2 != null || o != null))
          if (d3 === "style")
            if (o) {
              for (u2 in o)
                !o.hasOwnProperty(u2) || s2 && s2.hasOwnProperty(u2) || (t || (t = {}), t[u2] = "");
              for (u2 in s2)
                s2.hasOwnProperty(u2) && o[u2] !== s2[u2] && (t || (t = {}), t[u2] = s2[u2]);
            } else
              t || (i || (i = []), i.push(d3, t)), t = s2;
          else
            d3 === "dangerouslySetInnerHTML" ? (s2 = s2 ? s2.__html : void 0, o = o ? o.__html : void 0, s2 != null && o !== s2 && (i = i || []).push(d3, s2)) : d3 === "children" ? typeof s2 != "string" && typeof s2 != "number" || (i = i || []).push(d3, "" + s2) : d3 !== "suppressContentEditableWarning" && d3 !== "suppressHydrationWarning" && (St2.hasOwnProperty(d3) ? (s2 != null && d3 === "onScroll" && T2("scroll", e), i || o === s2 || (i = [])) : (i = i || []).push(d3, s2));
      }
      t && (i = i || []).push("style", t);
      var d3 = i;
      (n12.updateQueue = d3) && (n12.flags |= 4);
    }
  };
  ea2 = function(e, n12, t, r2) {
    t !== r2 && (n12.flags |= 4);
  };
  function lt2(e, n12) {
    if (!D2)
      switch (e.tailMode) {
        case "hidden":
          n12 = e.tail;
          for (var t = null; n12 !== null; )
            n12.alternate !== null && (t = n12), n12 = n12.sibling;
          t === null ? e.tail = null : t.sibling = null;
          break;
        case "collapsed":
          t = e.tail;
          for (var r2 = null; t !== null; )
            t.alternate !== null && (r2 = t), t = t.sibling;
          r2 === null ? n12 || e.tail === null ? e.tail = null : e.tail.sibling = null : r2.sibling = null;
      }
  }
  function G2(e) {
    var n12 = e.alternate !== null && e.alternate.child === e.child, t = 0, r2 = 0;
    if (n12)
      for (var l2 = e.child; l2 !== null; )
        t |= l2.lanes | l2.childLanes, r2 |= l2.subtreeFlags & 14680064, r2 |= l2.flags & 14680064, l2.return = e, l2 = l2.sibling;
    else
      for (l2 = e.child; l2 !== null; )
        t |= l2.lanes | l2.childLanes, r2 |= l2.subtreeFlags, r2 |= l2.flags, l2.return = e, l2 = l2.sibling;
    return e.subtreeFlags |= r2, e.childLanes = t, n12;
  }
  function af(e, n12, t) {
    var r2 = n12.pendingProps;
    switch (Ri2(n12), n12.tag) {
      case 2:
      case 16:
      case 15:
      case 0:
      case 11:
      case 7:
      case 8:
      case 12:
      case 9:
      case 14:
        return G2(n12), null;
      case 1:
        return le4(n12.type) && Nr2(), G2(n12), null;
      case 3:
        return r2 = n12.stateNode, Xn2(), M2(re2), M2(J2), Wi2(), r2.pendingContext && (r2.context = r2.pendingContext, r2.pendingContext = null), (e === null || e.child === null) && (nr2(n12) ? n12.flags |= 4 : e === null || e.memoizedState.isDehydrated && !(n12.flags & 256) || (n12.flags |= 1024, ke4 !== null && (vi2(ke4), ke4 = null))), si2(e, n12), G2(n12), null;
      case 5:
        Hi2(n12);
        var l2 = mn2(Dt2.current);
        if (t = n12.type, e !== null && n12.stateNode != null)
          bs2(e, n12, t, r2, l2), e.ref !== n12.ref && (n12.flags |= 512, n12.flags |= 2097152);
        else {
          if (!r2) {
            if (n12.stateNode === null)
              throw Error(v3(166));
            return G2(n12), null;
          }
          if (e = mn2(Le3.current), nr2(n12)) {
            r2 = n12.stateNode, t = n12.type;
            var i = n12.memoizedProps;
            switch (r2[ze3] = n12, r2[Tt2] = i, e = (n12.mode & 1) !== 0, t) {
              case "dialog":
                T2("cancel", r2), T2("close", r2);
                break;
              case "iframe":
              case "object":
              case "embed":
                T2("load", r2);
                break;
              case "video":
              case "audio":
                for (l2 = 0; l2 < ct2.length; l2++)
                  T2(ct2[l2], r2);
                break;
              case "source":
                T2("error", r2);
                break;
              case "img":
              case "image":
              case "link":
                T2("error", r2), T2("load", r2);
                break;
              case "details":
                T2("toggle", r2);
                break;
              case "input":
                mu(r2, i), T2("invalid", r2);
                break;
              case "select":
                r2._wrapperState = { wasMultiple: !!i.multiple }, T2("invalid", r2);
                break;
              case "textarea":
                vu(r2, i), T2("invalid", r2);
            }
            Fl2(t, i), l2 = null;
            for (var u2 in i)
              if (i.hasOwnProperty(u2)) {
                var o = i[u2];
                u2 === "children" ? typeof o == "string" ? r2.textContent !== o && (i.suppressHydrationWarning !== true && er2(r2.textContent, o, e), l2 = ["children", o]) : typeof o == "number" && r2.textContent !== "" + o && (i.suppressHydrationWarning !== true && er2(r2.textContent, o, e), l2 = ["children", "" + o]) : St2.hasOwnProperty(u2) && o != null && u2 === "onScroll" && T2("scroll", r2);
              }
            switch (t) {
              case "input":
                Ht2(r2), hu(r2, i, true);
                break;
              case "textarea":
                Ht2(r2), yu(r2);
                break;
              case "select":
              case "option":
                break;
              default:
                typeof i.onClick == "function" && (r2.onclick = xr2);
            }
            r2 = l2, n12.updateQueue = r2, r2 !== null && (n12.flags |= 4);
          } else {
            u2 = l2.nodeType === 9 ? l2 : l2.ownerDocument, e === "http://www.w3.org/1999/xhtml" && (e = zo2(t)), e === "http://www.w3.org/1999/xhtml" ? t === "script" ? (e = u2.createElement("div"), e.innerHTML = "<script><\/script>", e = e.removeChild(e.firstChild)) : typeof r2.is == "string" ? e = u2.createElement(t, { is: r2.is }) : (e = u2.createElement(t), t === "select" && (u2 = e, r2.multiple ? u2.multiple = true : r2.size && (u2.size = r2.size))) : e = u2.createElementNS(e, t), e[ze3] = n12, e[Tt2] = r2, qs2(e, n12, false, false), n12.stateNode = e;
            e: {
              switch (u2 = Il2(t, r2), t) {
                case "dialog":
                  T2("cancel", e), T2("close", e), l2 = r2;
                  break;
                case "iframe":
                case "object":
                case "embed":
                  T2("load", e), l2 = r2;
                  break;
                case "video":
                case "audio":
                  for (l2 = 0; l2 < ct2.length; l2++)
                    T2(ct2[l2], e);
                  l2 = r2;
                  break;
                case "source":
                  T2("error", e), l2 = r2;
                  break;
                case "img":
                case "image":
                case "link":
                  T2("error", e), T2("load", e), l2 = r2;
                  break;
                case "details":
                  T2("toggle", e), l2 = r2;
                  break;
                case "input":
                  mu(e, r2), l2 = Tl2(e, r2), T2("invalid", e);
                  break;
                case "option":
                  l2 = r2;
                  break;
                case "select":
                  e._wrapperState = { wasMultiple: !!r2.multiple }, l2 = F2({}, r2, { value: void 0 }), T2("invalid", e);
                  break;
                case "textarea":
                  vu(e, r2), l2 = Ol2(e, r2), T2("invalid", e);
                  break;
                default:
                  l2 = r2;
              }
              Fl2(t, l2), o = l2;
              for (i in o)
                if (o.hasOwnProperty(i)) {
                  var s2 = o[i];
                  i === "style" ? To2(e, s2) : i === "dangerouslySetInnerHTML" ? (s2 = s2 ? s2.__html : void 0, s2 != null && Po2(e, s2)) : i === "children" ? typeof s2 == "string" ? (t !== "textarea" || s2 !== "") && kt2(e, s2) : typeof s2 == "number" && kt2(e, "" + s2) : i !== "suppressContentEditableWarning" && i !== "suppressHydrationWarning" && i !== "autoFocus" && (St2.hasOwnProperty(i) ? s2 != null && i === "onScroll" && T2("scroll", e) : s2 != null && wi2(e, i, s2, u2));
                }
              switch (t) {
                case "input":
                  Ht2(e), hu(e, r2, false);
                  break;
                case "textarea":
                  Ht2(e), yu(e);
                  break;
                case "option":
                  r2.value != null && e.setAttribute("value", "" + tn2(r2.value));
                  break;
                case "select":
                  e.multiple = !!r2.multiple, i = r2.value, i != null ? jn2(e, !!r2.multiple, i, false) : r2.defaultValue != null && jn2(e, !!r2.multiple, r2.defaultValue, true);
                  break;
                default:
                  typeof l2.onClick == "function" && (e.onclick = xr2);
              }
              switch (t) {
                case "button":
                case "input":
                case "select":
                case "textarea":
                  r2 = !!r2.autoFocus;
                  break e;
                case "img":
                  r2 = true;
                  break e;
                default:
                  r2 = false;
              }
            }
            r2 && (n12.flags |= 4);
          }
          n12.ref !== null && (n12.flags |= 512, n12.flags |= 2097152);
        }
        return G2(n12), null;
      case 6:
        if (e && n12.stateNode != null)
          ea2(e, n12, e.memoizedProps, r2);
        else {
          if (typeof r2 != "string" && n12.stateNode === null)
            throw Error(v3(166));
          if (t = mn2(Dt2.current), mn2(Le3.current), nr2(n12)) {
            if (r2 = n12.stateNode, t = n12.memoizedProps, r2[ze3] = n12, (i = r2.nodeValue !== t) && (e = se3, e !== null))
              switch (e.tag) {
                case 3:
                  er2(r2.nodeValue, t, (e.mode & 1) !== 0);
                  break;
                case 5:
                  e.memoizedProps.suppressHydrationWarning !== true && er2(r2.nodeValue, t, (e.mode & 1) !== 0);
              }
            i && (n12.flags |= 4);
          } else
            r2 = (t.nodeType === 9 ? t : t.ownerDocument).createTextNode(r2), r2[ze3] = n12, n12.stateNode = r2;
        }
        return G2(n12), null;
      case 13:
        if (M2(O5), r2 = n12.memoizedState, e === null || e.memoizedState !== null && e.memoizedState.dehydrated !== null) {
          if (D2 && oe3 !== null && n12.mode & 1 && !(n12.flags & 128))
            ys2(), Kn2(), n12.flags |= 98560, i = false;
          else if (i = nr2(n12), r2 !== null && r2.dehydrated !== null) {
            if (e === null) {
              if (!i)
                throw Error(v3(318));
              if (i = n12.memoizedState, i = i !== null ? i.dehydrated : null, !i)
                throw Error(v3(317));
              i[ze3] = n12;
            } else
              Kn2(), !(n12.flags & 128) && (n12.memoizedState = null), n12.flags |= 4;
            G2(n12), i = false;
          } else
            ke4 !== null && (vi2(ke4), ke4 = null), i = true;
          if (!i)
            return n12.flags & 65536 ? n12 : null;
        }
        return n12.flags & 128 ? (n12.lanes = t, n12) : (r2 = r2 !== null, r2 !== (e !== null && e.memoizedState !== null) && r2 && (n12.child.flags |= 8192, n12.mode & 1 && (e === null || O5.current & 1 ? B3 === 0 && (B3 = 3) : nu())), n12.updateQueue !== null && (n12.flags |= 4), G2(n12), null);
      case 4:
        return Xn2(), si2(e, n12), e === null && Pt2(n12.stateNode.containerInfo), G2(n12), null;
      case 10:
        return ji2(n12.type._context), G2(n12), null;
      case 17:
        return le4(n12.type) && Nr2(), G2(n12), null;
      case 19:
        if (M2(O5), i = n12.memoizedState, i === null)
          return G2(n12), null;
        if (r2 = (n12.flags & 128) !== 0, u2 = i.rendering, u2 === null)
          if (r2)
            lt2(i, false);
          else {
            if (B3 !== 0 || e !== null && e.flags & 128)
              for (e = n12.child; e !== null; ) {
                if (u2 = Dr2(e), u2 !== null) {
                  for (n12.flags |= 128, lt2(i, false), r2 = u2.updateQueue, r2 !== null && (n12.updateQueue = r2, n12.flags |= 4), n12.subtreeFlags = 0, r2 = t, t = n12.child; t !== null; )
                    i = t, e = r2, i.flags &= 14680066, u2 = i.alternate, u2 === null ? (i.childLanes = 0, i.lanes = e, i.child = null, i.subtreeFlags = 0, i.memoizedProps = null, i.memoizedState = null, i.updateQueue = null, i.dependencies = null, i.stateNode = null) : (i.childLanes = u2.childLanes, i.lanes = u2.lanes, i.child = u2.child, i.subtreeFlags = 0, i.deletions = null, i.memoizedProps = u2.memoizedProps, i.memoizedState = u2.memoizedState, i.updateQueue = u2.updateQueue, i.type = u2.type, e = u2.dependencies, i.dependencies = e === null ? null : { lanes: e.lanes, firstContext: e.firstContext }), t = t.sibling;
                  return L(O5, O5.current & 1 | 2), n12.child;
                }
                e = e.sibling;
              }
            i.tail !== null && j() > Zn2 && (n12.flags |= 128, r2 = true, lt2(i, false), n12.lanes = 4194304);
          }
        else {
          if (!r2)
            if (e = Dr2(u2), e !== null) {
              if (n12.flags |= 128, r2 = true, t = e.updateQueue, t !== null && (n12.updateQueue = t, n12.flags |= 4), lt2(i, true), i.tail === null && i.tailMode === "hidden" && !u2.alternate && !D2)
                return G2(n12), null;
            } else
              2 * j() - i.renderingStartTime > Zn2 && t !== 1073741824 && (n12.flags |= 128, r2 = true, lt2(i, false), n12.lanes = 4194304);
          i.isBackwards ? (u2.sibling = n12.child, n12.child = u2) : (t = i.last, t !== null ? t.sibling = u2 : n12.child = u2, i.last = u2);
        }
        return i.tail !== null ? (n12 = i.tail, i.rendering = n12, i.tail = n12.sibling, i.renderingStartTime = j(), n12.sibling = null, t = O5.current, L(O5, r2 ? t & 1 | 2 : t & 1), n12) : (G2(n12), null);
      case 22:
      case 23:
        return eu(), r2 = n12.memoizedState !== null, e !== null && e.memoizedState !== null !== r2 && (n12.flags |= 8192), r2 && n12.mode & 1 ? ue2 & 1073741824 && (G2(n12), n12.subtreeFlags & 6 && (n12.flags |= 8192)) : G2(n12), null;
      case 24:
        return null;
      case 25:
        return null;
    }
    throw Error(v3(156, n12.tag));
  }
  function cf(e, n12) {
    switch (Ri2(n12), n12.tag) {
      case 1:
        return le4(n12.type) && Nr2(), e = n12.flags, e & 65536 ? (n12.flags = e & -65537 | 128, n12) : null;
      case 3:
        return Xn2(), M2(re2), M2(J2), Wi2(), e = n12.flags, e & 65536 && !(e & 128) ? (n12.flags = e & -65537 | 128, n12) : null;
      case 5:
        return Hi2(n12), null;
      case 13:
        if (M2(O5), e = n12.memoizedState, e !== null && e.dehydrated !== null) {
          if (n12.alternate === null)
            throw Error(v3(340));
          Kn2();
        }
        return e = n12.flags, e & 65536 ? (n12.flags = e & -65537 | 128, n12) : null;
      case 19:
        return M2(O5), null;
      case 4:
        return Xn2(), null;
      case 10:
        return ji2(n12.type._context), null;
      case 22:
      case 23:
        return eu(), null;
      case 24:
        return null;
      default:
        return null;
    }
  }
  var lr2 = false, Z3 = false, ff = typeof WeakSet == "function" ? WeakSet : Set, w = null;
  function In2(e, n12) {
    var t = e.ref;
    if (t !== null)
      if (typeof t == "function")
        try {
          t(null);
        } catch (r2) {
          I(e, n12, r2);
        }
      else
        t.current = null;
  }
  function ai2(e, n12, t) {
    try {
      t();
    } catch (r2) {
      I(e, n12, r2);
    }
  }
  var oo2 = false;
  function df(e, n12) {
    if (Kl2 = kr2, e = ls2(), Di2(e)) {
      if ("selectionStart" in e)
        var t = { start: e.selectionStart, end: e.selectionEnd };
      else
        e: {
          t = (t = e.ownerDocument) && t.defaultView || window;
          var r2 = t.getSelection && t.getSelection();
          if (r2 && r2.rangeCount !== 0) {
            t = r2.anchorNode;
            var l2 = r2.anchorOffset, i = r2.focusNode;
            r2 = r2.focusOffset;
            try {
              t.nodeType, i.nodeType;
            } catch {
              t = null;
              break e;
            }
            var u2 = 0, o = -1, s2 = -1, d3 = 0, m3 = 0, h2 = e, p3 = null;
            n:
              for (; ; ) {
                for (var g2; h2 !== t || l2 !== 0 && h2.nodeType !== 3 || (o = u2 + l2), h2 !== i || r2 !== 0 && h2.nodeType !== 3 || (s2 = u2 + r2), h2.nodeType === 3 && (u2 += h2.nodeValue.length), (g2 = h2.firstChild) !== null; )
                  p3 = h2, h2 = g2;
                for (; ; ) {
                  if (h2 === e)
                    break n;
                  if (p3 === t && ++d3 === l2 && (o = u2), p3 === i && ++m3 === r2 && (s2 = u2), (g2 = h2.nextSibling) !== null)
                    break;
                  h2 = p3, p3 = h2.parentNode;
                }
                h2 = g2;
              }
            t = o === -1 || s2 === -1 ? null : { start: o, end: s2 };
          } else
            t = null;
        }
      t = t || { start: 0, end: 0 };
    } else
      t = null;
    for (Yl2 = { focusedElem: e, selectionRange: t }, kr2 = false, w = n12; w !== null; )
      if (n12 = w, e = n12.child, (n12.subtreeFlags & 1028) !== 0 && e !== null)
        e.return = n12, w = e;
      else
        for (; w !== null; ) {
          n12 = w;
          try {
            var S2 = n12.alternate;
            if (n12.flags & 1024)
              switch (n12.tag) {
                case 0:
                case 11:
                case 15:
                  break;
                case 1:
                  if (S2 !== null) {
                    var k = S2.memoizedProps, U3 = S2.memoizedState, c = n12.stateNode, a2 = c.getSnapshotBeforeUpdate(n12.elementType === n12.type ? k : we4(n12.type, k), U3);
                    c.__reactInternalSnapshotBeforeUpdate = a2;
                  }
                  break;
                case 3:
                  var f3 = n12.stateNode.containerInfo;
                  f3.nodeType === 1 ? f3.textContent = "" : f3.nodeType === 9 && f3.documentElement && f3.removeChild(f3.documentElement);
                  break;
                case 5:
                case 6:
                case 4:
                case 17:
                  break;
                default:
                  throw Error(v3(163));
              }
          } catch (y3) {
            I(n12, n12.return, y3);
          }
          if (e = n12.sibling, e !== null) {
            e.return = n12.return, w = e;
            break;
          }
          w = n12.return;
        }
    return S2 = oo2, oo2 = false, S2;
  }
  function yt2(e, n12, t) {
    var r2 = n12.updateQueue;
    if (r2 = r2 !== null ? r2.lastEffect : null, r2 !== null) {
      var l2 = r2 = r2.next;
      do {
        if ((l2.tag & e) === e) {
          var i = l2.destroy;
          l2.destroy = void 0, i !== void 0 && ai2(n12, t, i);
        }
        l2 = l2.next;
      } while (l2 !== r2);
    }
  }
  function Xr2(e, n12) {
    if (n12 = n12.updateQueue, n12 = n12 !== null ? n12.lastEffect : null, n12 !== null) {
      var t = n12 = n12.next;
      do {
        if ((t.tag & e) === e) {
          var r2 = t.create;
          t.destroy = r2();
        }
        t = t.next;
      } while (t !== n12);
    }
  }
  function ci2(e) {
    var n12 = e.ref;
    if (n12 !== null) {
      var t = e.stateNode;
      switch (e.tag) {
        case 5:
          e = t;
          break;
        default:
          e = t;
      }
      typeof n12 == "function" ? n12(e) : n12.current = e;
    }
  }
  function na2(e) {
    var n12 = e.alternate;
    n12 !== null && (e.alternate = null, na2(n12)), e.child = null, e.deletions = null, e.sibling = null, e.tag === 5 && (n12 = e.stateNode, n12 !== null && (delete n12[ze3], delete n12[Tt2], delete n12[Zl2], delete n12[Yc], delete n12[Xc])), e.stateNode = null, e.return = null, e.dependencies = null, e.memoizedProps = null, e.memoizedState = null, e.pendingProps = null, e.stateNode = null, e.updateQueue = null;
  }
  function ta2(e) {
    return e.tag === 5 || e.tag === 3 || e.tag === 4;
  }
  function so2(e) {
    e:
      for (; ; ) {
        for (; e.sibling === null; ) {
          if (e.return === null || ta2(e.return))
            return null;
          e = e.return;
        }
        for (e.sibling.return = e.return, e = e.sibling; e.tag !== 5 && e.tag !== 6 && e.tag !== 18; ) {
          if (e.flags & 2 || e.child === null || e.tag === 4)
            continue e;
          e.child.return = e, e = e.child;
        }
        if (!(e.flags & 2))
          return e.stateNode;
      }
  }
  function fi2(e, n12, t) {
    var r2 = e.tag;
    if (r2 === 5 || r2 === 6)
      e = e.stateNode, n12 ? t.nodeType === 8 ? t.parentNode.insertBefore(e, n12) : t.insertBefore(e, n12) : (t.nodeType === 8 ? (n12 = t.parentNode, n12.insertBefore(e, t)) : (n12 = t, n12.appendChild(e)), t = t._reactRootContainer, t != null || n12.onclick !== null || (n12.onclick = xr2));
    else if (r2 !== 4 && (e = e.child, e !== null))
      for (fi2(e, n12, t), e = e.sibling; e !== null; )
        fi2(e, n12, t), e = e.sibling;
  }
  function di2(e, n12, t) {
    var r2 = e.tag;
    if (r2 === 5 || r2 === 6)
      e = e.stateNode, n12 ? t.insertBefore(e, n12) : t.appendChild(e);
    else if (r2 !== 4 && (e = e.child, e !== null))
      for (di2(e, n12, t), e = e.sibling; e !== null; )
        di2(e, n12, t), e = e.sibling;
  }
  var $3 = null, Se3 = false;
  function Be3(e, n12, t) {
    for (t = t.child; t !== null; )
      ra2(e, n12, t), t = t.sibling;
  }
  function ra2(e, n12, t) {
    if (Pe4 && typeof Pe4.onCommitFiberUnmount == "function")
      try {
        Pe4.onCommitFiberUnmount(Ar2, t);
      } catch {
      }
    switch (t.tag) {
      case 5:
        Z3 || In2(t, n12);
      case 6:
        var r2 = $3, l2 = Se3;
        $3 = null, Be3(e, n12, t), $3 = r2, Se3 = l2, $3 !== null && (Se3 ? (e = $3, t = t.stateNode, e.nodeType === 8 ? e.parentNode.removeChild(t) : e.removeChild(t)) : $3.removeChild(t.stateNode));
        break;
      case 18:
        $3 !== null && (Se3 ? (e = $3, t = t.stateNode, e.nodeType === 8 ? hl2(e.parentNode, t) : e.nodeType === 1 && hl2(e, t), Nt2(e)) : hl2($3, t.stateNode));
        break;
      case 4:
        r2 = $3, l2 = Se3, $3 = t.stateNode.containerInfo, Se3 = true, Be3(e, n12, t), $3 = r2, Se3 = l2;
        break;
      case 0:
      case 11:
      case 14:
      case 15:
        if (!Z3 && (r2 = t.updateQueue, r2 !== null && (r2 = r2.lastEffect, r2 !== null))) {
          l2 = r2 = r2.next;
          do {
            var i = l2, u2 = i.destroy;
            i = i.tag, u2 !== void 0 && (i & 2 || i & 4) && ai2(t, n12, u2), l2 = l2.next;
          } while (l2 !== r2);
        }
        Be3(e, n12, t);
        break;
      case 1:
        if (!Z3 && (In2(t, n12), r2 = t.stateNode, typeof r2.componentWillUnmount == "function"))
          try {
            r2.props = t.memoizedProps, r2.state = t.memoizedState, r2.componentWillUnmount();
          } catch (o) {
            I(t, n12, o);
          }
        Be3(e, n12, t);
        break;
      case 21:
        Be3(e, n12, t);
        break;
      case 22:
        t.mode & 1 ? (Z3 = (r2 = Z3) || t.memoizedState !== null, Be3(e, n12, t), Z3 = r2) : Be3(e, n12, t);
        break;
      default:
        Be3(e, n12, t);
    }
  }
  function ao2(e) {
    var n12 = e.updateQueue;
    if (n12 !== null) {
      e.updateQueue = null;
      var t = e.stateNode;
      t === null && (t = e.stateNode = new ff()), n12.forEach(function(r2) {
        var l2 = kf.bind(null, e, r2);
        t.has(r2) || (t.add(r2), r2.then(l2, l2));
      });
    }
  }
  function ge3(e, n12) {
    var t = n12.deletions;
    if (t !== null)
      for (var r2 = 0; r2 < t.length; r2++) {
        var l2 = t[r2];
        try {
          var i = e, u2 = n12, o = u2;
          e:
            for (; o !== null; ) {
              switch (o.tag) {
                case 5:
                  $3 = o.stateNode, Se3 = false;
                  break e;
                case 3:
                  $3 = o.stateNode.containerInfo, Se3 = true;
                  break e;
                case 4:
                  $3 = o.stateNode.containerInfo, Se3 = true;
                  break e;
              }
              o = o.return;
            }
          if ($3 === null)
            throw Error(v3(160));
          ra2(i, u2, l2), $3 = null, Se3 = false;
          var s2 = l2.alternate;
          s2 !== null && (s2.return = null), l2.return = null;
        } catch (d3) {
          I(l2, n12, d3);
        }
      }
    if (n12.subtreeFlags & 12854)
      for (n12 = n12.child; n12 !== null; )
        la2(n12, e), n12 = n12.sibling;
  }
  function la2(e, n12) {
    var t = e.alternate, r2 = e.flags;
    switch (e.tag) {
      case 0:
      case 11:
      case 14:
      case 15:
        if (ge3(n12, e), Ne3(e), r2 & 4) {
          try {
            yt2(3, e, e.return), Xr2(3, e);
          } catch (k) {
            I(e, e.return, k);
          }
          try {
            yt2(5, e, e.return);
          } catch (k) {
            I(e, e.return, k);
          }
        }
        break;
      case 1:
        ge3(n12, e), Ne3(e), r2 & 512 && t !== null && In2(t, t.return);
        break;
      case 5:
        if (ge3(n12, e), Ne3(e), r2 & 512 && t !== null && In2(t, t.return), e.flags & 32) {
          var l2 = e.stateNode;
          try {
            kt2(l2, "");
          } catch (k) {
            I(e, e.return, k);
          }
        }
        if (r2 & 4 && (l2 = e.stateNode, l2 != null)) {
          var i = e.memoizedProps, u2 = t !== null ? t.memoizedProps : i, o = e.type, s2 = e.updateQueue;
          if (e.updateQueue = null, s2 !== null)
            try {
              o === "input" && i.type === "radio" && i.name != null && No2(l2, i), Il2(o, u2);
              var d3 = Il2(o, i);
              for (u2 = 0; u2 < s2.length; u2 += 2) {
                var m3 = s2[u2], h2 = s2[u2 + 1];
                m3 === "style" ? To2(l2, h2) : m3 === "dangerouslySetInnerHTML" ? Po2(l2, h2) : m3 === "children" ? kt2(l2, h2) : wi2(l2, m3, h2, d3);
              }
              switch (o) {
                case "input":
                  Ml2(l2, i);
                  break;
                case "textarea":
                  _o2(l2, i);
                  break;
                case "select":
                  var p3 = l2._wrapperState.wasMultiple;
                  l2._wrapperState.wasMultiple = !!i.multiple;
                  var g2 = i.value;
                  g2 != null ? jn2(l2, !!i.multiple, g2, false) : p3 !== !!i.multiple && (i.defaultValue != null ? jn2(l2, !!i.multiple, i.defaultValue, true) : jn2(l2, !!i.multiple, i.multiple ? [] : "", false));
              }
              l2[Tt2] = i;
            } catch (k) {
              I(e, e.return, k);
            }
        }
        break;
      case 6:
        if (ge3(n12, e), Ne3(e), r2 & 4) {
          if (e.stateNode === null)
            throw Error(v3(162));
          l2 = e.stateNode, i = e.memoizedProps;
          try {
            l2.nodeValue = i;
          } catch (k) {
            I(e, e.return, k);
          }
        }
        break;
      case 3:
        if (ge3(n12, e), Ne3(e), r2 & 4 && t !== null && t.memoizedState.isDehydrated)
          try {
            Nt2(n12.containerInfo);
          } catch (k) {
            I(e, e.return, k);
          }
        break;
      case 4:
        ge3(n12, e), Ne3(e);
        break;
      case 13:
        ge3(n12, e), Ne3(e), l2 = e.child, l2.flags & 8192 && (i = l2.memoizedState !== null, l2.stateNode.isHidden = i, !i || l2.alternate !== null && l2.alternate.memoizedState !== null || (qi2 = j())), r2 & 4 && ao2(e);
        break;
      case 22:
        if (m3 = t !== null && t.memoizedState !== null, e.mode & 1 ? (Z3 = (d3 = Z3) || m3, ge3(n12, e), Z3 = d3) : ge3(n12, e), Ne3(e), r2 & 8192) {
          if (d3 = e.memoizedState !== null, (e.stateNode.isHidden = d3) && !m3 && e.mode & 1)
            for (w = e, m3 = e.child; m3 !== null; ) {
              for (h2 = w = m3; w !== null; ) {
                switch (p3 = w, g2 = p3.child, p3.tag) {
                  case 0:
                  case 11:
                  case 14:
                  case 15:
                    yt2(4, p3, p3.return);
                    break;
                  case 1:
                    In2(p3, p3.return);
                    var S2 = p3.stateNode;
                    if (typeof S2.componentWillUnmount == "function") {
                      r2 = p3, t = p3.return;
                      try {
                        n12 = r2, S2.props = n12.memoizedProps, S2.state = n12.memoizedState, S2.componentWillUnmount();
                      } catch (k) {
                        I(r2, t, k);
                      }
                    }
                    break;
                  case 5:
                    In2(p3, p3.return);
                    break;
                  case 22:
                    if (p3.memoizedState !== null) {
                      fo2(h2);
                      continue;
                    }
                }
                g2 !== null ? (g2.return = p3, w = g2) : fo2(h2);
              }
              m3 = m3.sibling;
            }
          e:
            for (m3 = null, h2 = e; ; ) {
              if (h2.tag === 5) {
                if (m3 === null) {
                  m3 = h2;
                  try {
                    l2 = h2.stateNode, d3 ? (i = l2.style, typeof i.setProperty == "function" ? i.setProperty("display", "none", "important") : i.display = "none") : (o = h2.stateNode, s2 = h2.memoizedProps.style, u2 = s2 != null && s2.hasOwnProperty("display") ? s2.display : null, o.style.display = Lo2("display", u2));
                  } catch (k) {
                    I(e, e.return, k);
                  }
                }
              } else if (h2.tag === 6) {
                if (m3 === null)
                  try {
                    h2.stateNode.nodeValue = d3 ? "" : h2.memoizedProps;
                  } catch (k) {
                    I(e, e.return, k);
                  }
              } else if ((h2.tag !== 22 && h2.tag !== 23 || h2.memoizedState === null || h2 === e) && h2.child !== null) {
                h2.child.return = h2, h2 = h2.child;
                continue;
              }
              if (h2 === e)
                break e;
              for (; h2.sibling === null; ) {
                if (h2.return === null || h2.return === e)
                  break e;
                m3 === h2 && (m3 = null), h2 = h2.return;
              }
              m3 === h2 && (m3 = null), h2.sibling.return = h2.return, h2 = h2.sibling;
            }
        }
        break;
      case 19:
        ge3(n12, e), Ne3(e), r2 & 4 && ao2(e);
        break;
      case 21:
        break;
      default:
        ge3(n12, e), Ne3(e);
    }
  }
  function Ne3(e) {
    var n12 = e.flags;
    if (n12 & 2) {
      try {
        e: {
          for (var t = e.return; t !== null; ) {
            if (ta2(t)) {
              var r2 = t;
              break e;
            }
            t = t.return;
          }
          throw Error(v3(160));
        }
        switch (r2.tag) {
          case 5:
            var l2 = r2.stateNode;
            r2.flags & 32 && (kt2(l2, ""), r2.flags &= -33);
            var i = so2(e);
            di2(e, i, l2);
            break;
          case 3:
          case 4:
            var u2 = r2.stateNode.containerInfo, o = so2(e);
            fi2(e, o, u2);
            break;
          default:
            throw Error(v3(161));
        }
      } catch (s2) {
        I(e, e.return, s2);
      }
      e.flags &= -3;
    }
    n12 & 4096 && (e.flags &= -4097);
  }
  function pf(e, n12, t) {
    w = e, ia2(e, n12, t);
  }
  function ia2(e, n12, t) {
    for (var r2 = (e.mode & 1) !== 0; w !== null; ) {
      var l2 = w, i = l2.child;
      if (l2.tag === 22 && r2) {
        var u2 = l2.memoizedState !== null || lr2;
        if (!u2) {
          var o = l2.alternate, s2 = o !== null && o.memoizedState !== null || Z3;
          o = lr2;
          var d3 = Z3;
          if (lr2 = u2, (Z3 = s2) && !d3)
            for (w = l2; w !== null; )
              u2 = w, s2 = u2.child, u2.tag === 22 && u2.memoizedState !== null ? po2(l2) : s2 !== null ? (s2.return = u2, w = s2) : po2(l2);
          for (; i !== null; )
            w = i, ia2(i, n12, t), i = i.sibling;
          w = l2, lr2 = o, Z3 = d3;
        }
        co2(e, n12, t);
      } else
        l2.subtreeFlags & 8772 && i !== null ? (i.return = l2, w = i) : co2(e, n12, t);
    }
  }
  function co2(e) {
    for (; w !== null; ) {
      var n12 = w;
      if (n12.flags & 8772) {
        var t = n12.alternate;
        try {
          if (n12.flags & 8772)
            switch (n12.tag) {
              case 0:
              case 11:
              case 15:
                Z3 || Xr2(5, n12);
                break;
              case 1:
                var r2 = n12.stateNode;
                if (n12.flags & 4 && !Z3)
                  if (t === null)
                    r2.componentDidMount();
                  else {
                    var l2 = n12.elementType === n12.type ? t.memoizedProps : we4(n12.type, t.memoizedProps);
                    r2.componentDidUpdate(l2, t.memoizedState, r2.__reactInternalSnapshotBeforeUpdate);
                  }
                var i = n12.updateQueue;
                i !== null && Yu(n12, i, r2);
                break;
              case 3:
                var u2 = n12.updateQueue;
                if (u2 !== null) {
                  if (t = null, n12.child !== null)
                    switch (n12.child.tag) {
                      case 5:
                        t = n12.child.stateNode;
                        break;
                      case 1:
                        t = n12.child.stateNode;
                    }
                  Yu(n12, u2, t);
                }
                break;
              case 5:
                var o = n12.stateNode;
                if (t === null && n12.flags & 4) {
                  t = o;
                  var s2 = n12.memoizedProps;
                  switch (n12.type) {
                    case "button":
                    case "input":
                    case "select":
                    case "textarea":
                      s2.autoFocus && t.focus();
                      break;
                    case "img":
                      s2.src && (t.src = s2.src);
                  }
                }
                break;
              case 6:
                break;
              case 4:
                break;
              case 12:
                break;
              case 13:
                if (n12.memoizedState === null) {
                  var d3 = n12.alternate;
                  if (d3 !== null) {
                    var m3 = d3.memoizedState;
                    if (m3 !== null) {
                      var h2 = m3.dehydrated;
                      h2 !== null && Nt2(h2);
                    }
                  }
                }
                break;
              case 19:
              case 17:
              case 21:
              case 22:
              case 23:
              case 25:
                break;
              default:
                throw Error(v3(163));
            }
          Z3 || n12.flags & 512 && ci2(n12);
        } catch (p3) {
          I(n12, n12.return, p3);
        }
      }
      if (n12 === e) {
        w = null;
        break;
      }
      if (t = n12.sibling, t !== null) {
        t.return = n12.return, w = t;
        break;
      }
      w = n12.return;
    }
  }
  function fo2(e) {
    for (; w !== null; ) {
      var n12 = w;
      if (n12 === e) {
        w = null;
        break;
      }
      var t = n12.sibling;
      if (t !== null) {
        t.return = n12.return, w = t;
        break;
      }
      w = n12.return;
    }
  }
  function po2(e) {
    for (; w !== null; ) {
      var n12 = w;
      try {
        switch (n12.tag) {
          case 0:
          case 11:
          case 15:
            var t = n12.return;
            try {
              Xr2(4, n12);
            } catch (s2) {
              I(n12, t, s2);
            }
            break;
          case 1:
            var r2 = n12.stateNode;
            if (typeof r2.componentDidMount == "function") {
              var l2 = n12.return;
              try {
                r2.componentDidMount();
              } catch (s2) {
                I(n12, l2, s2);
              }
            }
            var i = n12.return;
            try {
              ci2(n12);
            } catch (s2) {
              I(n12, i, s2);
            }
            break;
          case 5:
            var u2 = n12.return;
            try {
              ci2(n12);
            } catch (s2) {
              I(n12, u2, s2);
            }
        }
      } catch (s2) {
        I(n12, n12.return, s2);
      }
      if (n12 === e) {
        w = null;
        break;
      }
      var o = n12.sibling;
      if (o !== null) {
        o.return = n12.return, w = o;
        break;
      }
      w = n12.return;
    }
  }
  var mf = Math.ceil, Fr2 = Ve3.ReactCurrentDispatcher, Zi2 = Ve3.ReactCurrentOwner, he4 = Ve3.ReactCurrentBatchConfig, _ = 0, Q2 = null, V = null, K = 0, ue2 = 0, Un2 = un2(0), B3 = 0, It2 = null, Sn2 = 0, Gr2 = 0, Ji2 = 0, gt2 = null, ne3 = null, qi2 = 0, Zn2 = 1 / 0, Te3 = null, Ir2 = false, pi2 = null, be4 = null, ir2 = false, Ye2 = null, Ur2 = 0, wt2 = 0, mi2 = null, pr2 = -1, mr2 = 0;
  function b() {
    return _ & 6 ? j() : pr2 !== -1 ? pr2 : pr2 = j();
  }
  function en2(e) {
    return e.mode & 1 ? _ & 2 && K !== 0 ? K & -K : Zc.transition !== null ? (mr2 === 0 && (mr2 = Ho2()), mr2) : (e = P2, e !== 0 || (e = window.event, e = e === void 0 ? 16 : Go2(e.type)), e) : 1;
  }
  function Ce4(e, n12, t, r2) {
    if (50 < wt2)
      throw wt2 = 0, mi2 = null, Error(v3(185));
    Ut2(e, t, r2), (!(_ & 2) || e !== Q2) && (e === Q2 && (!(_ & 2) && (Gr2 |= t), B3 === 4 && $e3(e, K)), ie3(e, r2), t === 1 && _ === 0 && !(n12.mode & 1) && (Zn2 = j() + 500, $r2 && on2()));
  }
  function ie3(e, n12) {
    var t = e.callbackNode;
    qa2(e, n12);
    var r2 = Sr2(e, e === Q2 ? K : 0);
    if (r2 === 0)
      t !== null && Su(t), e.callbackNode = null, e.callbackPriority = 0;
    else if (n12 = r2 & -r2, e.callbackPriority !== n12) {
      if (t != null && Su(t), n12 === 1)
        e.tag === 0 ? Gc(mo2.bind(null, e)) : ms2(mo2.bind(null, e)), $c(function() {
          !(_ & 6) && on2();
        }), t = null;
      else {
        switch (Wo2(r2)) {
          case 1:
            t = xi2;
            break;
          case 4:
            t = Ao2;
            break;
          case 16:
            t = wr2;
            break;
          case 536870912:
            t = Bo2;
            break;
          default:
            t = wr2;
        }
        t = pa2(t, ua2.bind(null, e));
      }
      e.callbackPriority = n12, e.callbackNode = t;
    }
  }
  function ua2(e, n12) {
    if (pr2 = -1, mr2 = 0, _ & 6)
      throw Error(v3(327));
    var t = e.callbackNode;
    if (Wn2() && e.callbackNode !== t)
      return null;
    var r2 = Sr2(e, e === Q2 ? K : 0);
    if (r2 === 0)
      return null;
    if (r2 & 30 || r2 & e.expiredLanes || n12)
      n12 = jr2(e, r2);
    else {
      n12 = r2;
      var l2 = _;
      _ |= 2;
      var i = sa2();
      (Q2 !== e || K !== n12) && (Te3 = null, Zn2 = j() + 500, hn2(e, n12));
      do
        try {
          yf();
          break;
        } catch (o) {
          oa2(e, o);
        }
      while (true);
      Ui2(), Fr2.current = i, _ = l2, V !== null ? n12 = 0 : (Q2 = null, K = 0, n12 = B3);
    }
    if (n12 !== 0) {
      if (n12 === 2 && (l2 = Bl2(e), l2 !== 0 && (r2 = l2, n12 = hi2(e, l2))), n12 === 1)
        throw t = It2, hn2(e, 0), $e3(e, r2), ie3(e, j()), t;
      if (n12 === 6)
        $e3(e, r2);
      else {
        if (l2 = e.current.alternate, !(r2 & 30) && !hf(l2) && (n12 = jr2(e, r2), n12 === 2 && (i = Bl2(e), i !== 0 && (r2 = i, n12 = hi2(e, i))), n12 === 1))
          throw t = It2, hn2(e, 0), $e3(e, r2), ie3(e, j()), t;
        switch (e.finishedWork = l2, e.finishedLanes = r2, n12) {
          case 0:
          case 1:
            throw Error(v3(345));
          case 2:
            fn2(e, ne3, Te3);
            break;
          case 3:
            if ($e3(e, r2), (r2 & 130023424) === r2 && (n12 = qi2 + 500 - j(), 10 < n12)) {
              if (Sr2(e, 0) !== 0)
                break;
              if (l2 = e.suspendedLanes, (l2 & r2) !== r2) {
                b(), e.pingedLanes |= e.suspendedLanes & l2;
                break;
              }
              e.timeoutHandle = Gl2(fn2.bind(null, e, ne3, Te3), n12);
              break;
            }
            fn2(e, ne3, Te3);
            break;
          case 4:
            if ($e3(e, r2), (r2 & 4194240) === r2)
              break;
            for (n12 = e.eventTimes, l2 = -1; 0 < r2; ) {
              var u2 = 31 - Ee4(r2);
              i = 1 << u2, u2 = n12[u2], u2 > l2 && (l2 = u2), r2 &= ~i;
            }
            if (r2 = l2, r2 = j() - r2, r2 = (120 > r2 ? 120 : 480 > r2 ? 480 : 1080 > r2 ? 1080 : 1920 > r2 ? 1920 : 3e3 > r2 ? 3e3 : 4320 > r2 ? 4320 : 1960 * mf(r2 / 1960)) - r2, 10 < r2) {
              e.timeoutHandle = Gl2(fn2.bind(null, e, ne3, Te3), r2);
              break;
            }
            fn2(e, ne3, Te3);
            break;
          case 5:
            fn2(e, ne3, Te3);
            break;
          default:
            throw Error(v3(329));
        }
      }
    }
    return ie3(e, j()), e.callbackNode === t ? ua2.bind(null, e) : null;
  }
  function hi2(e, n12) {
    var t = gt2;
    return e.current.memoizedState.isDehydrated && (hn2(e, n12).flags |= 256), e = jr2(e, n12), e !== 2 && (n12 = ne3, ne3 = t, n12 !== null && vi2(n12)), e;
  }
  function vi2(e) {
    ne3 === null ? ne3 = e : ne3.push.apply(ne3, e);
  }
  function hf(e) {
    for (var n12 = e; ; ) {
      if (n12.flags & 16384) {
        var t = n12.updateQueue;
        if (t !== null && (t = t.stores, t !== null))
          for (var r2 = 0; r2 < t.length; r2++) {
            var l2 = t[r2], i = l2.getSnapshot;
            l2 = l2.value;
            try {
              if (!xe4(i(), l2))
                return false;
            } catch {
              return false;
            }
          }
      }
      if (t = n12.child, n12.subtreeFlags & 16384 && t !== null)
        t.return = n12, n12 = t;
      else {
        if (n12 === e)
          break;
        for (; n12.sibling === null; ) {
          if (n12.return === null || n12.return === e)
            return true;
          n12 = n12.return;
        }
        n12.sibling.return = n12.return, n12 = n12.sibling;
      }
    }
    return true;
  }
  function $e3(e, n12) {
    for (n12 &= ~Ji2, n12 &= ~Gr2, e.suspendedLanes |= n12, e.pingedLanes &= ~n12, e = e.expirationTimes; 0 < n12; ) {
      var t = 31 - Ee4(n12), r2 = 1 << t;
      e[t] = -1, n12 &= ~r2;
    }
  }
  function mo2(e) {
    if (_ & 6)
      throw Error(v3(327));
    Wn2();
    var n12 = Sr2(e, 0);
    if (!(n12 & 1))
      return ie3(e, j()), null;
    var t = jr2(e, n12);
    if (e.tag !== 0 && t === 2) {
      var r2 = Bl2(e);
      r2 !== 0 && (n12 = r2, t = hi2(e, r2));
    }
    if (t === 1)
      throw t = It2, hn2(e, 0), $e3(e, n12), ie3(e, j()), t;
    if (t === 6)
      throw Error(v3(345));
    return e.finishedWork = e.current.alternate, e.finishedLanes = n12, fn2(e, ne3, Te3), ie3(e, j()), null;
  }
  function bi2(e, n12) {
    var t = _;
    _ |= 1;
    try {
      return e(n12);
    } finally {
      _ = t, _ === 0 && (Zn2 = j() + 500, $r2 && on2());
    }
  }
  function kn2(e) {
    Ye2 !== null && Ye2.tag === 0 && !(_ & 6) && Wn2();
    var n12 = _;
    _ |= 1;
    var t = he4.transition, r2 = P2;
    try {
      if (he4.transition = null, P2 = 1, e)
        return e();
    } finally {
      P2 = r2, he4.transition = t, _ = n12, !(_ & 6) && on2();
    }
  }
  function eu() {
    ue2 = Un2.current, M2(Un2);
  }
  function hn2(e, n12) {
    e.finishedWork = null, e.finishedLanes = 0;
    var t = e.timeoutHandle;
    if (t !== -1 && (e.timeoutHandle = -1, Qc(t)), V !== null)
      for (t = V.return; t !== null; ) {
        var r2 = t;
        switch (Ri2(r2), r2.tag) {
          case 1:
            r2 = r2.type.childContextTypes, r2 != null && Nr2();
            break;
          case 3:
            Xn2(), M2(re2), M2(J2), Wi2();
            break;
          case 5:
            Hi2(r2);
            break;
          case 4:
            Xn2();
            break;
          case 13:
            M2(O5);
            break;
          case 19:
            M2(O5);
            break;
          case 10:
            ji2(r2.type._context);
            break;
          case 22:
          case 23:
            eu();
        }
        t = t.return;
      }
    if (Q2 = e, V = e = nn2(e.current, null), K = ue2 = n12, B3 = 0, It2 = null, Ji2 = Gr2 = Sn2 = 0, ne3 = gt2 = null, pn2 !== null) {
      for (n12 = 0; n12 < pn2.length; n12++)
        if (t = pn2[n12], r2 = t.interleaved, r2 !== null) {
          t.interleaved = null;
          var l2 = r2.next, i = t.pending;
          if (i !== null) {
            var u2 = i.next;
            i.next = l2, r2.next = u2;
          }
          t.pending = r2;
        }
      pn2 = null;
    }
    return e;
  }
  function oa2(e, n12) {
    do {
      var t = V;
      try {
        if (Ui2(), cr2.current = Rr2, Or2) {
          for (var r2 = R.memoizedState; r2 !== null; ) {
            var l2 = r2.queue;
            l2 !== null && (l2.pending = null), r2 = r2.next;
          }
          Or2 = false;
        }
        if (wn2 = 0, W4 = A3 = R = null, vt2 = false, Ot2 = 0, Zi2.current = null, t === null || t.return === null) {
          B3 = 1, It2 = n12, V = null;
          break;
        }
        e: {
          var i = e, u2 = t.return, o = t, s2 = n12;
          if (n12 = K, o.flags |= 32768, s2 !== null && typeof s2 == "object" && typeof s2.then == "function") {
            var d3 = s2, m3 = o, h2 = m3.tag;
            if (!(m3.mode & 1) && (h2 === 0 || h2 === 11 || h2 === 15)) {
              var p3 = m3.alternate;
              p3 ? (m3.updateQueue = p3.updateQueue, m3.memoizedState = p3.memoizedState, m3.lanes = p3.lanes) : (m3.updateQueue = null, m3.memoizedState = null);
            }
            var g2 = eo2(u2);
            if (g2 !== null) {
              g2.flags &= -257, no2(g2, u2, o, i, n12), g2.mode & 1 && bu(i, d3, n12), n12 = g2, s2 = d3;
              var S2 = n12.updateQueue;
              if (S2 === null) {
                var k = /* @__PURE__ */ new Set();
                k.add(s2), n12.updateQueue = k;
              } else
                S2.add(s2);
              break e;
            } else {
              if (!(n12 & 1)) {
                bu(i, d3, n12), nu();
                break e;
              }
              s2 = Error(v3(426));
            }
          } else if (D2 && o.mode & 1) {
            var U3 = eo2(u2);
            if (U3 !== null) {
              !(U3.flags & 65536) && (U3.flags |= 256), no2(U3, u2, o, i, n12), Fi2(Gn2(s2, o));
              break e;
            }
          }
          i = s2 = Gn2(s2, o), B3 !== 4 && (B3 = 2), gt2 === null ? gt2 = [i] : gt2.push(i), i = u2;
          do {
            switch (i.tag) {
              case 3:
                i.flags |= 65536, n12 &= -n12, i.lanes |= n12;
                var c = Qs2(i, s2, n12);
                Ku(i, c);
                break e;
              case 1:
                o = s2;
                var a2 = i.type, f3 = i.stateNode;
                if (!(i.flags & 128) && (typeof a2.getDerivedStateFromError == "function" || f3 !== null && typeof f3.componentDidCatch == "function" && (be4 === null || !be4.has(f3)))) {
                  i.flags |= 65536, n12 &= -n12, i.lanes |= n12;
                  var y3 = $s2(i, o, n12);
                  Ku(i, y3);
                  break e;
                }
            }
            i = i.return;
          } while (i !== null);
        }
        ca2(t);
      } catch (E4) {
        n12 = E4, V === t && t !== null && (V = t = t.return);
        continue;
      }
      break;
    } while (true);
  }
  function sa2() {
    var e = Fr2.current;
    return Fr2.current = Rr2, e === null ? Rr2 : e;
  }
  function nu() {
    (B3 === 0 || B3 === 3 || B3 === 2) && (B3 = 4), Q2 === null || !(Sn2 & 268435455) && !(Gr2 & 268435455) || $e3(Q2, K);
  }
  function jr2(e, n12) {
    var t = _;
    _ |= 2;
    var r2 = sa2();
    (Q2 !== e || K !== n12) && (Te3 = null, hn2(e, n12));
    do
      try {
        vf();
        break;
      } catch (l2) {
        oa2(e, l2);
      }
    while (true);
    if (Ui2(), _ = t, Fr2.current = r2, V !== null)
      throw Error(v3(261));
    return Q2 = null, K = 0, B3;
  }
  function vf() {
    for (; V !== null; )
      aa2(V);
  }
  function yf() {
    for (; V !== null && !Wa2(); )
      aa2(V);
  }
  function aa2(e) {
    var n12 = da2(e.alternate, e, ue2);
    e.memoizedProps = e.pendingProps, n12 === null ? ca2(e) : V = n12, Zi2.current = null;
  }
  function ca2(e) {
    var n12 = e;
    do {
      var t = n12.alternate;
      if (e = n12.return, n12.flags & 32768) {
        if (t = cf(t, n12), t !== null) {
          t.flags &= 32767, V = t;
          return;
        }
        if (e !== null)
          e.flags |= 32768, e.subtreeFlags = 0, e.deletions = null;
        else {
          B3 = 6, V = null;
          return;
        }
      } else if (t = af(t, n12, ue2), t !== null) {
        V = t;
        return;
      }
      if (n12 = n12.sibling, n12 !== null) {
        V = n12;
        return;
      }
      V = n12 = e;
    } while (n12 !== null);
    B3 === 0 && (B3 = 5);
  }
  function fn2(e, n12, t) {
    var r2 = P2, l2 = he4.transition;
    try {
      he4.transition = null, P2 = 1, gf(e, n12, t, r2);
    } finally {
      he4.transition = l2, P2 = r2;
    }
    return null;
  }
  function gf(e, n12, t, r2) {
    do
      Wn2();
    while (Ye2 !== null);
    if (_ & 6)
      throw Error(v3(327));
    t = e.finishedWork;
    var l2 = e.finishedLanes;
    if (t === null)
      return null;
    if (e.finishedWork = null, e.finishedLanes = 0, t === e.current)
      throw Error(v3(177));
    e.callbackNode = null, e.callbackPriority = 0;
    var i = t.lanes | t.childLanes;
    if (ba2(e, i), e === Q2 && (V = Q2 = null, K = 0), !(t.subtreeFlags & 2064) && !(t.flags & 2064) || ir2 || (ir2 = true, pa2(wr2, function() {
      return Wn2(), null;
    })), i = (t.flags & 15990) !== 0, t.subtreeFlags & 15990 || i) {
      i = he4.transition, he4.transition = null;
      var u2 = P2;
      P2 = 1;
      var o = _;
      _ |= 4, Zi2.current = null, df(e, t), la2(t, e), Vc(Yl2), kr2 = !!Kl2, Yl2 = Kl2 = null, e.current = t, pf(t, e, l2), Qa2(), _ = o, P2 = u2, he4.transition = i;
    } else
      e.current = t;
    if (ir2 && (ir2 = false, Ye2 = e, Ur2 = l2), i = e.pendingLanes, i === 0 && (be4 = null), Ya2(t.stateNode, r2), ie3(e, j()), n12 !== null)
      for (r2 = e.onRecoverableError, t = 0; t < n12.length; t++)
        l2 = n12[t], r2(l2.value, { componentStack: l2.stack, digest: l2.digest });
    if (Ir2)
      throw Ir2 = false, e = pi2, pi2 = null, e;
    return Ur2 & 1 && e.tag !== 0 && Wn2(), i = e.pendingLanes, i & 1 ? e === mi2 ? wt2++ : (wt2 = 0, mi2 = e) : wt2 = 0, on2(), null;
  }
  function Wn2() {
    if (Ye2 !== null) {
      var e = Wo2(Ur2), n12 = he4.transition, t = P2;
      try {
        if (he4.transition = null, P2 = 16 > e ? 16 : e, Ye2 === null)
          var r2 = false;
        else {
          if (e = Ye2, Ye2 = null, Ur2 = 0, _ & 6)
            throw Error(v3(331));
          var l2 = _;
          for (_ |= 4, w = e.current; w !== null; ) {
            var i = w, u2 = i.child;
            if (w.flags & 16) {
              var o = i.deletions;
              if (o !== null) {
                for (var s2 = 0; s2 < o.length; s2++) {
                  var d3 = o[s2];
                  for (w = d3; w !== null; ) {
                    var m3 = w;
                    switch (m3.tag) {
                      case 0:
                      case 11:
                      case 15:
                        yt2(8, m3, i);
                    }
                    var h2 = m3.child;
                    if (h2 !== null)
                      h2.return = m3, w = h2;
                    else
                      for (; w !== null; ) {
                        m3 = w;
                        var p3 = m3.sibling, g2 = m3.return;
                        if (na2(m3), m3 === d3) {
                          w = null;
                          break;
                        }
                        if (p3 !== null) {
                          p3.return = g2, w = p3;
                          break;
                        }
                        w = g2;
                      }
                  }
                }
                var S2 = i.alternate;
                if (S2 !== null) {
                  var k = S2.child;
                  if (k !== null) {
                    S2.child = null;
                    do {
                      var U3 = k.sibling;
                      k.sibling = null, k = U3;
                    } while (k !== null);
                  }
                }
                w = i;
              }
            }
            if (i.subtreeFlags & 2064 && u2 !== null)
              u2.return = i, w = u2;
            else
              e:
                for (; w !== null; ) {
                  if (i = w, i.flags & 2048)
                    switch (i.tag) {
                      case 0:
                      case 11:
                      case 15:
                        yt2(9, i, i.return);
                    }
                  var c = i.sibling;
                  if (c !== null) {
                    c.return = i.return, w = c;
                    break e;
                  }
                  w = i.return;
                }
          }
          var a2 = e.current;
          for (w = a2; w !== null; ) {
            u2 = w;
            var f3 = u2.child;
            if (u2.subtreeFlags & 2064 && f3 !== null)
              f3.return = u2, w = f3;
            else
              e:
                for (u2 = a2; w !== null; ) {
                  if (o = w, o.flags & 2048)
                    try {
                      switch (o.tag) {
                        case 0:
                        case 11:
                        case 15:
                          Xr2(9, o);
                      }
                    } catch (E4) {
                      I(o, o.return, E4);
                    }
                  if (o === u2) {
                    w = null;
                    break e;
                  }
                  var y3 = o.sibling;
                  if (y3 !== null) {
                    y3.return = o.return, w = y3;
                    break e;
                  }
                  w = o.return;
                }
          }
          if (_ = l2, on2(), Pe4 && typeof Pe4.onPostCommitFiberRoot == "function")
            try {
              Pe4.onPostCommitFiberRoot(Ar2, e);
            } catch {
            }
          r2 = true;
        }
        return r2;
      } finally {
        P2 = t, he4.transition = n12;
      }
    }
    return false;
  }
  function ho2(e, n12, t) {
    n12 = Gn2(t, n12), n12 = Qs2(e, n12, 1), e = qe3(e, n12, 1), n12 = b(), e !== null && (Ut2(e, 1, n12), ie3(e, n12));
  }
  function I(e, n12, t) {
    if (e.tag === 3)
      ho2(e, e, t);
    else
      for (; n12 !== null; ) {
        if (n12.tag === 3) {
          ho2(n12, e, t);
          break;
        } else if (n12.tag === 1) {
          var r2 = n12.stateNode;
          if (typeof n12.type.getDerivedStateFromError == "function" || typeof r2.componentDidCatch == "function" && (be4 === null || !be4.has(r2))) {
            e = Gn2(t, e), e = $s2(n12, e, 1), n12 = qe3(n12, e, 1), e = b(), n12 !== null && (Ut2(n12, 1, e), ie3(n12, e));
            break;
          }
        }
        n12 = n12.return;
      }
  }
  function wf(e, n12, t) {
    var r2 = e.pingCache;
    r2 !== null && r2.delete(n12), n12 = b(), e.pingedLanes |= e.suspendedLanes & t, Q2 === e && (K & t) === t && (B3 === 4 || B3 === 3 && (K & 130023424) === K && 500 > j() - qi2 ? hn2(e, 0) : Ji2 |= t), ie3(e, n12);
  }
  function fa2(e, n12) {
    n12 === 0 && (e.mode & 1 ? (n12 = $t2, $t2 <<= 1, !($t2 & 130023424) && ($t2 = 4194304)) : n12 = 1);
    var t = b();
    e = Ue3(e, n12), e !== null && (Ut2(e, n12, t), ie3(e, t));
  }
  function Sf(e) {
    var n12 = e.memoizedState, t = 0;
    n12 !== null && (t = n12.retryLane), fa2(e, t);
  }
  function kf(e, n12) {
    var t = 0;
    switch (e.tag) {
      case 13:
        var r2 = e.stateNode, l2 = e.memoizedState;
        l2 !== null && (t = l2.retryLane);
        break;
      case 19:
        r2 = e.stateNode;
        break;
      default:
        throw Error(v3(314));
    }
    r2 !== null && r2.delete(n12), fa2(e, t);
  }
  var da2;
  da2 = function(e, n12, t) {
    if (e !== null)
      if (e.memoizedProps !== n12.pendingProps || re2.current)
        te3 = true;
      else {
        if (!(e.lanes & t) && !(n12.flags & 128))
          return te3 = false, sf(e, n12, t);
        te3 = !!(e.flags & 131072);
      }
    else
      te3 = false, D2 && n12.flags & 1048576 && hs2(n12, Pr2, n12.index);
    switch (n12.lanes = 0, n12.tag) {
      case 2:
        var r2 = n12.type;
        dr2(e, n12), e = n12.pendingProps;
        var l2 = $n2(n12, J2.current);
        Hn2(n12, t), l2 = $i2(null, n12, r2, e, l2, t);
        var i = Ki2();
        return n12.flags |= 1, typeof l2 == "object" && l2 !== null && typeof l2.render == "function" && l2.$$typeof === void 0 ? (n12.tag = 1, n12.memoizedState = null, n12.updateQueue = null, le4(r2) ? (i = true, _r2(n12)) : i = false, n12.memoizedState = l2.state !== null && l2.state !== void 0 ? l2.state : null, Ai2(n12), l2.updater = Kr2, n12.stateNode = l2, l2._reactInternals = n12, ti2(n12, r2, e, t), n12 = ii2(null, n12, r2, true, i, t)) : (n12.tag = 0, D2 && i && Oi2(n12), q(null, n12, l2, t), n12 = n12.child), n12;
      case 16:
        r2 = n12.elementType;
        e: {
          switch (dr2(e, n12), e = n12.pendingProps, l2 = r2._init, r2 = l2(r2._payload), n12.type = r2, l2 = n12.tag = Cf(r2), e = we4(r2, e), l2) {
            case 0:
              n12 = li2(null, n12, r2, e, t);
              break e;
            case 1:
              n12 = lo2(null, n12, r2, e, t);
              break e;
            case 11:
              n12 = to2(null, n12, r2, e, t);
              break e;
            case 14:
              n12 = ro2(null, n12, r2, we4(r2.type, e), t);
              break e;
          }
          throw Error(v3(306, r2, ""));
        }
        return n12;
      case 0:
        return r2 = n12.type, l2 = n12.pendingProps, l2 = n12.elementType === r2 ? l2 : we4(r2, l2), li2(e, n12, r2, l2, t);
      case 1:
        return r2 = n12.type, l2 = n12.pendingProps, l2 = n12.elementType === r2 ? l2 : we4(r2, l2), lo2(e, n12, r2, l2, t);
      case 3:
        e: {
          if (Gs2(n12), e === null)
            throw Error(v3(387));
          r2 = n12.pendingProps, i = n12.memoizedState, l2 = i.element, ws2(e, n12), Mr2(n12, r2, null, t);
          var u2 = n12.memoizedState;
          if (r2 = u2.element, i.isDehydrated)
            if (i = { element: r2, isDehydrated: false, cache: u2.cache, pendingSuspenseBoundaries: u2.pendingSuspenseBoundaries, transitions: u2.transitions }, n12.updateQueue.baseState = i, n12.memoizedState = i, n12.flags & 256) {
              l2 = Gn2(Error(v3(423)), n12), n12 = io2(e, n12, r2, t, l2);
              break e;
            } else if (r2 !== l2) {
              l2 = Gn2(Error(v3(424)), n12), n12 = io2(e, n12, r2, t, l2);
              break e;
            } else
              for (oe3 = Je2(n12.stateNode.containerInfo.firstChild), se3 = n12, D2 = true, ke4 = null, t = Cs2(n12, null, r2, t), n12.child = t; t; )
                t.flags = t.flags & -3 | 4096, t = t.sibling;
          else {
            if (Kn2(), r2 === l2) {
              n12 = je3(e, n12, t);
              break e;
            }
            q(e, n12, r2, t);
          }
          n12 = n12.child;
        }
        return n12;
      case 5:
        return xs2(n12), e === null && bl2(n12), r2 = n12.type, l2 = n12.pendingProps, i = e !== null ? e.memoizedProps : null, u2 = l2.children, Xl2(r2, l2) ? u2 = null : i !== null && Xl2(r2, i) && (n12.flags |= 32), Xs2(e, n12), q(e, n12, u2, t), n12.child;
      case 6:
        return e === null && bl2(n12), null;
      case 13:
        return Zs2(e, n12, t);
      case 4:
        return Bi2(n12, n12.stateNode.containerInfo), r2 = n12.pendingProps, e === null ? n12.child = Yn2(n12, null, r2, t) : q(e, n12, r2, t), n12.child;
      case 11:
        return r2 = n12.type, l2 = n12.pendingProps, l2 = n12.elementType === r2 ? l2 : we4(r2, l2), to2(e, n12, r2, l2, t);
      case 7:
        return q(e, n12, n12.pendingProps, t), n12.child;
      case 8:
        return q(e, n12, n12.pendingProps.children, t), n12.child;
      case 12:
        return q(e, n12, n12.pendingProps.children, t), n12.child;
      case 10:
        e: {
          if (r2 = n12.type._context, l2 = n12.pendingProps, i = n12.memoizedProps, u2 = l2.value, L(Lr2, r2._currentValue), r2._currentValue = u2, i !== null)
            if (xe4(i.value, u2)) {
              if (i.children === l2.children && !re2.current) {
                n12 = je3(e, n12, t);
                break e;
              }
            } else
              for (i = n12.child, i !== null && (i.return = n12); i !== null; ) {
                var o = i.dependencies;
                if (o !== null) {
                  u2 = i.child;
                  for (var s2 = o.firstContext; s2 !== null; ) {
                    if (s2.context === r2) {
                      if (i.tag === 1) {
                        s2 = Re3(-1, t & -t), s2.tag = 2;
                        var d3 = i.updateQueue;
                        if (d3 !== null) {
                          d3 = d3.shared;
                          var m3 = d3.pending;
                          m3 === null ? s2.next = s2 : (s2.next = m3.next, m3.next = s2), d3.pending = s2;
                        }
                      }
                      i.lanes |= t, s2 = i.alternate, s2 !== null && (s2.lanes |= t), ei2(i.return, t, n12), o.lanes |= t;
                      break;
                    }
                    s2 = s2.next;
                  }
                } else if (i.tag === 10)
                  u2 = i.type === n12.type ? null : i.child;
                else if (i.tag === 18) {
                  if (u2 = i.return, u2 === null)
                    throw Error(v3(341));
                  u2.lanes |= t, o = u2.alternate, o !== null && (o.lanes |= t), ei2(u2, t, n12), u2 = i.sibling;
                } else
                  u2 = i.child;
                if (u2 !== null)
                  u2.return = i;
                else
                  for (u2 = i; u2 !== null; ) {
                    if (u2 === n12) {
                      u2 = null;
                      break;
                    }
                    if (i = u2.sibling, i !== null) {
                      i.return = u2.return, u2 = i;
                      break;
                    }
                    u2 = u2.return;
                  }
                i = u2;
              }
          q(e, n12, l2.children, t), n12 = n12.child;
        }
        return n12;
      case 9:
        return l2 = n12.type, r2 = n12.pendingProps.children, Hn2(n12, t), l2 = ve4(l2), r2 = r2(l2), n12.flags |= 1, q(e, n12, r2, t), n12.child;
      case 14:
        return r2 = n12.type, l2 = we4(r2, n12.pendingProps), l2 = we4(r2.type, l2), ro2(e, n12, r2, l2, t);
      case 15:
        return Ks2(e, n12, n12.type, n12.pendingProps, t);
      case 17:
        return r2 = n12.type, l2 = n12.pendingProps, l2 = n12.elementType === r2 ? l2 : we4(r2, l2), dr2(e, n12), n12.tag = 1, le4(r2) ? (e = true, _r2(n12)) : e = false, Hn2(n12, t), ks2(n12, r2, l2), ti2(n12, r2, l2, t), ii2(null, n12, r2, true, e, t);
      case 19:
        return Js2(e, n12, t);
      case 22:
        return Ys2(e, n12, t);
    }
    throw Error(v3(156, n12.tag));
  };
  function pa2(e, n12) {
    return Vo2(e, n12);
  }
  function Ef(e, n12, t, r2) {
    this.tag = e, this.key = t, this.sibling = this.child = this.return = this.stateNode = this.type = this.elementType = null, this.index = 0, this.ref = null, this.pendingProps = n12, this.dependencies = this.memoizedState = this.updateQueue = this.memoizedProps = null, this.mode = r2, this.subtreeFlags = this.flags = 0, this.deletions = null, this.childLanes = this.lanes = 0, this.alternate = null;
  }
  function me4(e, n12, t, r2) {
    return new Ef(e, n12, t, r2);
  }
  function tu(e) {
    return e = e.prototype, !(!e || !e.isReactComponent);
  }
  function Cf(e) {
    if (typeof e == "function")
      return tu(e) ? 1 : 0;
    if (e != null) {
      if (e = e.$$typeof, e === ki2)
        return 11;
      if (e === Ei2)
        return 14;
    }
    return 2;
  }
  function nn2(e, n12) {
    var t = e.alternate;
    return t === null ? (t = me4(e.tag, n12, e.key, e.mode), t.elementType = e.elementType, t.type = e.type, t.stateNode = e.stateNode, t.alternate = e, e.alternate = t) : (t.pendingProps = n12, t.type = e.type, t.flags = 0, t.subtreeFlags = 0, t.deletions = null), t.flags = e.flags & 14680064, t.childLanes = e.childLanes, t.lanes = e.lanes, t.child = e.child, t.memoizedProps = e.memoizedProps, t.memoizedState = e.memoizedState, t.updateQueue = e.updateQueue, n12 = e.dependencies, t.dependencies = n12 === null ? null : { lanes: n12.lanes, firstContext: n12.firstContext }, t.sibling = e.sibling, t.index = e.index, t.ref = e.ref, t;
  }
  function hr2(e, n12, t, r2, l2, i) {
    var u2 = 2;
    if (r2 = e, typeof e == "function")
      tu(e) && (u2 = 1);
    else if (typeof e == "string")
      u2 = 5;
    else
      e:
        switch (e) {
          case zn2:
            return vn2(t.children, l2, i, n12);
          case Si2:
            u2 = 8, l2 |= 8;
            break;
          case _l2:
            return e = me4(12, t, n12, l2 | 2), e.elementType = _l2, e.lanes = i, e;
          case zl2:
            return e = me4(13, t, n12, l2), e.elementType = zl2, e.lanes = i, e;
          case Pl2:
            return e = me4(19, t, n12, l2), e.elementType = Pl2, e.lanes = i, e;
          case Eo2:
            return Zr2(t, l2, i, n12);
          default:
            if (typeof e == "object" && e !== null)
              switch (e.$$typeof) {
                case So2:
                  u2 = 10;
                  break e;
                case ko2:
                  u2 = 9;
                  break e;
                case ki2:
                  u2 = 11;
                  break e;
                case Ei2:
                  u2 = 14;
                  break e;
                case He3:
                  u2 = 16, r2 = null;
                  break e;
              }
            throw Error(v3(130, e == null ? e : typeof e, ""));
        }
    return n12 = me4(u2, t, n12, l2), n12.elementType = e, n12.type = r2, n12.lanes = i, n12;
  }
  function vn2(e, n12, t, r2) {
    return e = me4(7, e, r2, n12), e.lanes = t, e;
  }
  function Zr2(e, n12, t, r2) {
    return e = me4(22, e, r2, n12), e.elementType = Eo2, e.lanes = t, e.stateNode = { isHidden: false }, e;
  }
  function Cl2(e, n12, t) {
    return e = me4(6, e, null, n12), e.lanes = t, e;
  }
  function xl2(e, n12, t) {
    return n12 = me4(4, e.children !== null ? e.children : [], e.key, n12), n12.lanes = t, n12.stateNode = { containerInfo: e.containerInfo, pendingChildren: null, implementation: e.implementation }, n12;
  }
  function xf(e, n12, t, r2, l2) {
    this.tag = n12, this.containerInfo = e, this.finishedWork = this.pingCache = this.current = this.pendingChildren = null, this.timeoutHandle = -1, this.callbackNode = this.pendingContext = this.context = null, this.callbackPriority = 0, this.eventTimes = ol2(0), this.expirationTimes = ol2(-1), this.entangledLanes = this.finishedLanes = this.mutableReadLanes = this.expiredLanes = this.pingedLanes = this.suspendedLanes = this.pendingLanes = 0, this.entanglements = ol2(0), this.identifierPrefix = r2, this.onRecoverableError = l2, this.mutableSourceEagerHydrationData = null;
  }
  function ru(e, n12, t, r2, l2, i, u2, o, s2) {
    return e = new xf(e, n12, t, o, s2), n12 === 1 ? (n12 = 1, i === true && (n12 |= 8)) : n12 = 0, i = me4(3, null, null, n12), e.current = i, i.stateNode = e, i.memoizedState = { element: r2, isDehydrated: t, cache: null, transitions: null, pendingSuspenseBoundaries: null }, Ai2(i), e;
  }
  function Nf(e, n12, t) {
    var r2 = 3 < arguments.length && arguments[3] !== void 0 ? arguments[3] : null;
    return { $$typeof: _n2, key: r2 == null ? null : "" + r2, children: e, containerInfo: n12, implementation: t };
  }
  function ma2(e) {
    if (!e)
      return rn2;
    e = e._reactInternals;
    e: {
      if (Cn2(e) !== e || e.tag !== 1)
        throw Error(v3(170));
      var n12 = e;
      do {
        switch (n12.tag) {
          case 3:
            n12 = n12.stateNode.context;
            break e;
          case 1:
            if (le4(n12.type)) {
              n12 = n12.stateNode.__reactInternalMemoizedMergedChildContext;
              break e;
            }
        }
        n12 = n12.return;
      } while (n12 !== null);
      throw Error(v3(171));
    }
    if (e.tag === 1) {
      var t = e.type;
      if (le4(t))
        return ps2(e, t, n12);
    }
    return n12;
  }
  function ha2(e, n12, t, r2, l2, i, u2, o, s2) {
    return e = ru(t, r2, true, e, l2, i, u2, o, s2), e.context = ma2(null), t = e.current, r2 = b(), l2 = en2(t), i = Re3(r2, l2), i.callback = n12 ?? null, qe3(t, i, l2), e.current.lanes = l2, Ut2(e, l2, r2), ie3(e, r2), e;
  }
  function Jr2(e, n12, t, r2) {
    var l2 = n12.current, i = b(), u2 = en2(l2);
    return t = ma2(t), n12.context === null ? n12.context = t : n12.pendingContext = t, n12 = Re3(i, u2), n12.payload = { element: e }, r2 = r2 === void 0 ? null : r2, r2 !== null && (n12.callback = r2), e = qe3(l2, n12, u2), e !== null && (Ce4(e, l2, u2, i), ar2(e, l2, u2)), u2;
  }
  function Vr2(e) {
    if (e = e.current, !e.child)
      return null;
    switch (e.child.tag) {
      case 5:
        return e.child.stateNode;
      default:
        return e.child.stateNode;
    }
  }
  function vo2(e, n12) {
    if (e = e.memoizedState, e !== null && e.dehydrated !== null) {
      var t = e.retryLane;
      e.retryLane = t !== 0 && t < n12 ? t : n12;
    }
  }
  function lu(e, n12) {
    vo2(e, n12), (e = e.alternate) && vo2(e, n12);
  }
  function _f() {
    return null;
  }
  var va2 = typeof reportError == "function" ? reportError : function(e) {
    console.error(e);
  };
  function iu(e) {
    this._internalRoot = e;
  }
  qr2.prototype.render = iu.prototype.render = function(e) {
    var n12 = this._internalRoot;
    if (n12 === null)
      throw Error(v3(409));
    Jr2(e, n12, null, null);
  };
  qr2.prototype.unmount = iu.prototype.unmount = function() {
    var e = this._internalRoot;
    if (e !== null) {
      this._internalRoot = null;
      var n12 = e.containerInfo;
      kn2(function() {
        Jr2(null, e, null, null);
      }), n12[Ie4] = null;
    }
  };
  function qr2(e) {
    this._internalRoot = e;
  }
  qr2.prototype.unstable_scheduleHydration = function(e) {
    if (e) {
      var n12 = Ko2();
      e = { blockedOn: null, target: e, priority: n12 };
      for (var t = 0; t < Qe.length && n12 !== 0 && n12 < Qe[t].priority; t++)
        ;
      Qe.splice(t, 0, e), t === 0 && Xo2(e);
    }
  };
  function uu(e) {
    return !(!e || e.nodeType !== 1 && e.nodeType !== 9 && e.nodeType !== 11);
  }
  function br2(e) {
    return !(!e || e.nodeType !== 1 && e.nodeType !== 9 && e.nodeType !== 11 && (e.nodeType !== 8 || e.nodeValue !== " react-mount-point-unstable "));
  }
  function yo2() {
  }
  function zf(e, n12, t, r2, l2) {
    if (l2) {
      if (typeof r2 == "function") {
        var i = r2;
        r2 = function() {
          var d3 = Vr2(u2);
          i.call(d3);
        };
      }
      var u2 = ha2(n12, r2, e, 0, null, false, false, "", yo2);
      return e._reactRootContainer = u2, e[Ie4] = u2.current, Pt2(e.nodeType === 8 ? e.parentNode : e), kn2(), u2;
    }
    for (; l2 = e.lastChild; )
      e.removeChild(l2);
    if (typeof r2 == "function") {
      var o = r2;
      r2 = function() {
        var d3 = Vr2(s2);
        o.call(d3);
      };
    }
    var s2 = ru(e, 0, false, null, null, false, false, "", yo2);
    return e._reactRootContainer = s2, e[Ie4] = s2.current, Pt2(e.nodeType === 8 ? e.parentNode : e), kn2(function() {
      Jr2(n12, s2, t, r2);
    }), s2;
  }
  function el2(e, n12, t, r2, l2) {
    var i = t._reactRootContainer;
    if (i) {
      var u2 = i;
      if (typeof l2 == "function") {
        var o = l2;
        l2 = function() {
          var s2 = Vr2(u2);
          o.call(s2);
        };
      }
      Jr2(n12, u2, e, l2);
    } else
      u2 = zf(t, n12, e, l2, r2);
    return Vr2(u2);
  }
  Qo2 = function(e) {
    switch (e.tag) {
      case 3:
        var n12 = e.stateNode;
        if (n12.current.memoizedState.isDehydrated) {
          var t = at2(n12.pendingLanes);
          t !== 0 && (Ni2(n12, t | 1), ie3(n12, j()), !(_ & 6) && (Zn2 = j() + 500, on2()));
        }
        break;
      case 13:
        kn2(function() {
          var r2 = Ue3(e, 1);
          if (r2 !== null) {
            var l2 = b();
            Ce4(r2, e, 1, l2);
          }
        }), lu(e, 1);
    }
  };
  _i2 = function(e) {
    if (e.tag === 13) {
      var n12 = Ue3(e, 134217728);
      if (n12 !== null) {
        var t = b();
        Ce4(n12, e, 134217728, t);
      }
      lu(e, 134217728);
    }
  };
  $o2 = function(e) {
    if (e.tag === 13) {
      var n12 = en2(e), t = Ue3(e, n12);
      if (t !== null) {
        var r2 = b();
        Ce4(t, e, n12, r2);
      }
      lu(e, n12);
    }
  };
  Ko2 = function() {
    return P2;
  };
  Yo2 = function(e, n12) {
    var t = P2;
    try {
      return P2 = e, n12();
    } finally {
      P2 = t;
    }
  };
  jl2 = function(e, n12, t) {
    switch (n12) {
      case "input":
        if (Ml2(e, t), n12 = t.name, t.type === "radio" && n12 != null) {
          for (t = e; t.parentNode; )
            t = t.parentNode;
          for (t = t.querySelectorAll("input[name=" + JSON.stringify("" + n12) + '][type="radio"]'), n12 = 0; n12 < t.length; n12++) {
            var r2 = t[n12];
            if (r2 !== e && r2.form === e.form) {
              var l2 = Qr2(r2);
              if (!l2)
                throw Error(v3(90));
              xo2(r2), Ml2(r2, l2);
            }
          }
        }
        break;
      case "textarea":
        _o2(e, t);
        break;
      case "select":
        n12 = t.value, n12 != null && jn2(e, !!t.multiple, n12, false);
    }
  };
  Oo2 = bi2;
  Ro2 = kn2;
  var Pf = { usingClientEntryPoint: false, Events: [Vt2, Mn2, Qr2, Mo2, Do2, bi2] }, it2 = { findFiberByHostInstance: dn2, bundleType: 0, version: "18.2.0", rendererPackageName: "react-dom" }, Lf = { bundleType: it2.bundleType, version: it2.version, rendererPackageName: it2.rendererPackageName, rendererConfig: it2.rendererConfig, overrideHookState: null, overrideHookStateDeletePath: null, overrideHookStateRenamePath: null, overrideProps: null, overridePropsDeletePath: null, overridePropsRenamePath: null, setErrorHandler: null, setSuspenseHandler: null, scheduleUpdate: null, currentDispatcherRef: Ve3.ReactCurrentDispatcher, findHostInstanceByFiber: function(e) {
    return e = Uo2(e), e === null ? null : e.stateNode;
  }, findFiberByHostInstance: it2.findFiberByHostInstance || _f, findHostInstancesForRefresh: null, scheduleRefresh: null, scheduleRoot: null, setRefreshHandler: null, getCurrentFiber: null, reconcilerVersion: "18.2.0-next-9e3b772b8-20220608" };
  if (typeof __REACT_DEVTOOLS_GLOBAL_HOOK__ < "u" && (ut2 = __REACT_DEVTOOLS_GLOBAL_HOOK__, !ut2.isDisabled && ut2.supportsFiber))
    try {
      Ar2 = ut2.inject(Lf), Pe4 = ut2;
    } catch {
    }
  var ut2;
  fe3.__SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED = Pf;
  fe3.createPortal = function(e, n12) {
    var t = 2 < arguments.length && arguments[2] !== void 0 ? arguments[2] : null;
    if (!uu(n12))
      throw Error(v3(200));
    return Nf(e, n12, null, t);
  };
  fe3.createRoot = function(e, n12) {
    if (!uu(e))
      throw Error(v3(299));
    var t = false, r2 = "", l2 = va2;
    return n12 != null && (n12.unstable_strictMode === true && (t = true), n12.identifierPrefix !== void 0 && (r2 = n12.identifierPrefix), n12.onRecoverableError !== void 0 && (l2 = n12.onRecoverableError)), n12 = ru(e, 1, false, null, null, t, false, r2, l2), e[Ie4] = n12.current, Pt2(e.nodeType === 8 ? e.parentNode : e), new iu(n12);
  };
  fe3.findDOMNode = function(e) {
    if (e == null)
      return null;
    if (e.nodeType === 1)
      return e;
    var n12 = e._reactInternals;
    if (n12 === void 0)
      throw typeof e.render == "function" ? Error(v3(188)) : (e = Object.keys(e).join(","), Error(v3(268, e)));
    return e = Uo2(n12), e = e === null ? null : e.stateNode, e;
  };
  fe3.flushSync = function(e) {
    return kn2(e);
  };
  fe3.hydrate = function(e, n12, t) {
    if (!br2(n12))
      throw Error(v3(200));
    return el2(null, e, n12, true, t);
  };
  fe3.hydrateRoot = function(e, n12, t) {
    if (!uu(e))
      throw Error(v3(405));
    var r2 = t != null && t.hydratedSources || null, l2 = false, i = "", u2 = va2;
    if (t != null && (t.unstable_strictMode === true && (l2 = true), t.identifierPrefix !== void 0 && (i = t.identifierPrefix), t.onRecoverableError !== void 0 && (u2 = t.onRecoverableError)), n12 = ha2(n12, null, e, 1, t ?? null, l2, false, i, u2), e[Ie4] = n12.current, Pt2(e), r2)
      for (e = 0; e < r2.length; e++)
        t = r2[e], l2 = t._getVersion, l2 = l2(t._source), n12.mutableSourceEagerHydrationData == null ? n12.mutableSourceEagerHydrationData = [t, l2] : n12.mutableSourceEagerHydrationData.push(t, l2);
    return new qr2(n12);
  };
  fe3.render = function(e, n12, t) {
    if (!br2(n12))
      throw Error(v3(200));
    return el2(null, e, n12, false, t);
  };
  fe3.unmountComponentAtNode = function(e) {
    if (!br2(e))
      throw Error(v3(40));
    return e._reactRootContainer ? (kn2(function() {
      el2(null, null, e, false, function() {
        e._reactRootContainer = null, e[Ie4] = null;
      });
    }), true) : false;
  };
  fe3.unstable_batchedUpdates = bi2;
  fe3.unstable_renderSubtreeIntoContainer = function(e, n12, t, r2) {
    if (!br2(t))
      throw Error(v3(200));
    if (e == null || e._reactInternals === void 0)
      throw Error(v3(38));
    return el2(e, n12, t, false, r2);
  };
  fe3.version = "18.2.0-next-9e3b772b8-20220608";
});
var ou = au((Kf, wa2) => {
  "use strict";
  function ga2() {
    if (!(typeof __REACT_DEVTOOLS_GLOBAL_HOOK__ > "u" || typeof __REACT_DEVTOOLS_GLOBAL_HOOK__.checkDCE != "function"))
      try {
        __REACT_DEVTOOLS_GLOBAL_HOOK__.checkDCE(ga2);
      } catch (e) {
        console.error(e);
      }
  }
  ga2(), wa2.exports = ya();
});
var sn = {};
Pa(sn, { __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: () => Tf, createPortal: () => Mf, createRoot: () => Df, default: () => Wf, findDOMNode: () => Of, flushSync: () => Rf, hydrate: () => Ff, hydrateRoot: () => If, render: () => Uf, unmountComponentAtNode: () => jf, unstable_batchedUpdates: () => Vf, unstable_renderSubtreeIntoContainer: () => Af, version: () => Bf });
var ka = cu(ou());
an(sn, cu(ou()));
var { __SECRET_INTERNALS_DO_NOT_USE_OR_YOU_WILL_BE_FIRED: Tf, createPortal: Mf, createRoot: Df, findDOMNode: Of, flushSync: Rf, hydrate: Ff, hydrateRoot: If, render: Uf, unmountComponentAtNode: jf, unstable_batchedUpdates: Vf, unstable_renderSubtreeIntoContainer: Af, version: Bf } = ka;
var { default: Sa, ...Hf } = ka;
var Wf = Sa !== void 0 ? Sa : Hf;

// https://esm.sh/v135/react-dom@18.2.0/denonext/client.js
var require3 = (n12) => {
  const e = (m3) => typeof m3.default < "u" ? m3.default : m3, c = (m3) => Object.assign({}, m3);
  switch (n12) {
    case "react-dom":
      return e(react_dom_exports);
    default:
      throw new Error('module "' + n12 + '" not found');
  }
};
var d2 = Object.create;
var u = Object.defineProperty;
var E3 = Object.getOwnPropertyDescriptor;
var m = Object.getOwnPropertyNames;
var p2 = Object.getPrototypeOf;
var h = Object.prototype.hasOwnProperty;
var x2 = ((t) => typeof require3 < "u" ? require3 : typeof Proxy < "u" ? new Proxy(t, { get: (e, o) => (typeof require3 < "u" ? require3 : e)[o] }) : t)(function(t) {
  if (typeof require3 < "u")
    return require3.apply(this, arguments);
  throw Error('Dynamic require of "' + t + '" is not supported');
});
var C = (t, e) => () => (e || t((e = { exports: {} }).exports, e), e.exports);
var N = (t, e) => {
  for (var o in e)
    u(t, o, { get: e[o], enumerable: true });
};
var a = (t, e, o, c) => {
  if (e && typeof e == "object" || typeof e == "function")
    for (let i of m(e))
      !h.call(t, i) && i !== o && u(t, i, { get: () => e[i], enumerable: !(c = E3(e, i)) || c.enumerable });
  return t;
};
var n = (t, e, o) => (a(t, e, "default"), o && a(o, e, "default"));
var l = (t, e, o) => (o = t != null ? d2(p2(t)) : {}, a(e || !t || !t.__esModule ? u(o, "default", { value: t, enumerable: true }) : o, t));
var s = C((_) => {
  "use strict";
  var R = x2("react-dom");
  _.createRoot = R.createRoot, _.hydrateRoot = R.hydrateRoot;
  var I;
});
var r = {};
N(r, { createRoot: () => O3, default: () => v, hydrateRoot: () => g });
var y2 = l(s());
n(r, l(s()));
var { createRoot: O3, hydrateRoot: g } = y2;
var { default: f2, ...P } = y2;
var v = f2 !== void 0 ? f2 : P;

// NiconicomeWeb/src/watch/ui/jsWatchInfo/watchInfoHandler.ts
var JsWatchInfoHandlerImpl = class {
  getData() {
    const element = document.querySelector("#jsWatchInfo");
    if (element === null)
      return null;
    if (!(element instanceof HTMLElement)) {
      return null;
    }
    const raw = element.dataset["jsWatchInfo"];
    if (raw === "" || raw === void 0)
      return null;
    return JSON.parse(raw);
  }
};

// NiconicomeWeb/src/watch/ui/state/videoState.ts
var InitialData = {
  canPlay: false,
  video: void 0,
  jsWatchInfo: void 0,
  isPlaying: false,
  isCommentVisible: true,
  commentManager: void 0,
  comments: void 0,
  isSystemMessageVisible: false,
  isShortcutVisible: false,
  contextMenu: {
    open: false,
    left: 0,
    top: 0
  },
  ngHandler: void 0
};
var VideoStateContext = Se({});
var reduceFunc = (state, action) => {
  switch (action.type) {
    case "canPlay":
      return { ...state, canPlay: action.payload };
    case "video":
      return { ...state, video: action.payload };
    case "jsWatchInfo":
      return { ...state, jsWatchInfo: action.payload };
    case "isPlaying":
      return { ...state, isPlaying: action.payload };
    case "commentManager":
      return { ...state, commentManager: action.payload };
    case "isCommentVisible":
      return { ...state, isCommentVisible: action.payload };
    case "comments":
      return { ...state, comments: action.payload };
    case "systemMessage":
      return { ...state, isSystemMessageVisible: action.payload };
    case "contextMenu":
      return {
        ...state,
        contextMenu: action.payload
      };
    case "shortcut":
      return { ...state, isShortcutVisible: action.payload };
  }
};

// NiconicomeWeb/src/watch/ui/componetnts/comment/commentItem.tsx
var CommentItem = ({ comment }) => {
  function setCurrentTime(vposMS) {
    if (state.video)
      state.video.currentTime = vposMS / 1e3;
  }
  const { state, dispatch } = Ie(VideoStateContext);
  const postedAt = new Date(comment.postedAt);
  const postedAtString = `${postedAt.getFullYear()}/${postedAt.getMonth() + 1}/${postedAt.getDate()} ${postedAt.getHours()}:${postedAt.getMinutes()}`;
  const vposMinutes = Math.floor(comment.vposMS / 6e4);
  const vposSeconds = Math.floor(comment.vposMS % 6e4 / 1e3);
  const time = `${vposMinutes.toString().padStart(2, "0")}:${vposSeconds.toString().padStart(2, "0")}`;
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "commentItem",
      onDoubleClick: () => setCurrentTime(comment.vposMS)
    },
    /* @__PURE__ */ We.createElement("div", { className: "commentContent", title: comment.body }, comment.body),
    /* @__PURE__ */ We.createElement("div", { className: "commentTime" }, time),
    /* @__PURE__ */ We.createElement("div", { className: "commentNumber" }, comment.number),
    /* @__PURE__ */ We.createElement("div", { className: "commentPostedAt" }, postedAtString)
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/comment/ng/ngItem.tsx
var NGItem = ({ data }) => {
  let typeStr = "";
  switch (data.ngType) {
    case "word":
      typeStr = "\u30B3\u30E1\u30F3\u30C8";
      break;
    case "user":
      typeStr = "\u30E6\u30FC\u30B6\u30FC";
      break;
    case "command":
      typeStr = "\u30B3\u30DE\u30F3\u30C9";
  }
  return /* @__PURE__ */ We.createElement("div", { className: "ngItem" }, /* @__PURE__ */ We.createElement("div", { className: "content" }, /* @__PURE__ */ We.createElement("div", { className: "type" }, typeStr), /* @__PURE__ */ We.createElement("div", { className: "value" }, data.value)), /* @__PURE__ */ We.createElement("div", { className: "remove", onClick: data.remove }, /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-x" })));
};

// NiconicomeWeb/src/shared/Extension/arrayExtension.ts
Object.defineProperty(Array.prototype, "pushRange", {
  enumerable: false,
  configurable: true,
  writable: true,
  value: function(source) {
    source.forEach((x3) => {
      this.push(x3);
    });
  }
});

// NiconicomeWeb/src/watch/ui/componetnts/comment/ng/ngInput.tsx
var NGInput = ({ onAdd }) => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [selectValue, setSelectValue] = Ae("word");
  const [inputValue, setInputValue] = Ae("");
  async function addNG() {
    if (inputValue === "")
      return;
    await state.ngHandler.addNG(selectValue, inputValue);
    setInputValue("");
    onAdd();
  }
  return /* @__PURE__ */ We.createElement("div", { className: "ngInput" }, /* @__PURE__ */ We.createElement(
    "select",
    {
      className: "form-select ngSelect",
      value: selectValue,
      onChange: (e) => setSelectValue(e.target.value)
    },
    /* @__PURE__ */ We.createElement("option", { selected: true }, "NG\u306E\u7A2E\u985E"),
    /* @__PURE__ */ We.createElement("option", { value: "word" }, "\u30B3\u30E1\u30F3\u30C8"),
    /* @__PURE__ */ We.createElement("option", { value: "command" }, "\u30B3\u30DE\u30F3\u30C9"),
    /* @__PURE__ */ We.createElement("option", { value: "user" }, "\u30E6\u30FC\u30B6\u30FCID")
  ), /* @__PURE__ */ We.createElement("div", { className: "input-group mb-3" }, /* @__PURE__ */ We.createElement(
    "input",
    {
      type: "text",
      className: "form-control",
      placeholder: `${selectValue === "user" ? "\u30E6\u30FC\u30B6\u30FCID" : selectValue === "command" ? "\u30B3\u30DE\u30F3\u30C9" : "\u30B3\u30E1\u30F3\u30C8"}\u3092\u5165\u529B`,
      value: inputValue,
      onChange: (e) => setInputValue(e.target.value)
    }
  ), /* @__PURE__ */ We.createElement(
    "button",
    {
      className: "btn btn-outline-secondary",
      type: "button",
      onClick: addNG
    },
    "\u8FFD\u52A0"
  )));
};

// NiconicomeWeb/src/watch/ui/componetnts/comment/ng/ngList.tsx
var NGList = ({ isExpanded, close }) => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [ng, setNG] = Ae([]);
  const [isInitialized, setIsInitialized] = Ae(false);
  Te(() => {
    const getNGs = async () => {
      const rawData = await state.ngHandler.getNGData();
      const formated = [];
      function formatNG(source, type) {
        const formated2 = source.map((x3) => ({
          ngType: type,
          value: x3,
          remove: async () => {
            await state.ngHandler.removeNG(type, x3);
            setIsInitialized(false);
          }
        }));
        return formated2;
      }
      formated.pushRange(formatNG(rawData.words, "word"));
      formated.pushRange(formatNG(rawData.users, "user"));
      formated.pushRange(formatNG(rawData.commands, "command"));
      setIsInitialized(true);
      setNG(formated);
    };
    getNGs();
  }, [isInitialized]);
  return /* @__PURE__ */ We.createElement("div", { className: `ngWrapper ${isExpanded ? "" : "collapsed"}` }, /* @__PURE__ */ We.createElement("div", { className: "listControler" }, /* @__PURE__ */ We.createElement("p", null, "NG\u8A2D\u5B9A"), /* @__PURE__ */ We.createElement("p", { className: "close", onClick: close }, /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-x" }))), /* @__PURE__ */ We.createElement("div", { className: "ngList" }, ng.map((x3) => /* @__PURE__ */ We.createElement(NGItem, { key: `${x3.value}-${x3.ngType}`, data: x3 }))), /* @__PURE__ */ We.createElement(NGInput, { onAdd: () => setIsInitialized(false) }));
};

// NiconicomeWeb/src/watch/ui/componetnts/comment/comment.tsx
var isCommentScrollEnabled = true;
var Comment = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [isExpanded, setIsExpanded] = Ae(true);
  const [isNGExpanded, setIsNGExpanded] = Ae(false);
  const [autoScroll, setIsAutoScroll] = Ae(true);
  const ref = qe(null);
  const currentVideo = qe("");
  const lastWheelTime = qe(Date.now());
  isCommentScrollEnabled = autoScroll;
  function sourceChanged() {
    if (currentVideo.current === "")
      return true;
    if (state.commentManager === void 0)
      return false;
    return currentVideo.current !== state.commentManager.currentID;
  }
  function onWheel() {
    if (!isCommentScrollEnabled)
      return;
    lastWheelTime.current = Date.now();
    setIsAutoScroll(false);
    setTimeout(() => {
      if (Date.now() - lastWheelTime.current < 1e4) {
        return;
      }
      setIsAutoScroll(true);
    }, 1e4);
  }
  Te(() => {
    if (!sourceChanged())
      return;
    if (state.commentManager === void 0)
      return;
    state.commentManager.on("commentAdded", (comment) => {
      if (!isCommentScrollEnabled)
        return;
      let y3 = 30 * (comment.innnerIndex + 1 - 12);
      if (y3 < 0) {
        y3 = 0;
      }
      ref.current?.scrollTo(0, y3);
    });
    currentVideo.current = state.jsWatchInfo?.video.niconicoID ?? "";
  });
  const elements = state.comments?.comments.map((comment) => /* @__PURE__ */ We.createElement(
    CommentItem,
    {
      key: comment.number,
      comment
    }
  ));
  return /* @__PURE__ */ We.createElement("div", { className: "commentWrapper" }, /* @__PURE__ */ We.createElement("div", { className: "listControler", onClick: () => setIsExpanded(!isExpanded) }, /* @__PURE__ */ We.createElement("p", null, "\u30B3\u30E1\u30F3\u30C8\u30EA\u30B9\u30C8"), /* @__PURE__ */ We.createElement("p", null, isExpanded ? "\u2227" : "\u2228")), /* @__PURE__ */ We.createElement(NGList, { isExpanded: isNGExpanded, close: () => setIsNGExpanded(false) }), /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `autoScroll form-check form-switch form-check-reverse ${isExpanded ? "" : "collapsed"}`
    },
    /* @__PURE__ */ We.createElement(
      "input",
      {
        className: "form-check-input",
        type: "checkbox",
        role: "switch",
        id: "commentScrollSwitch",
        checked: autoScroll,
        onChange: () => setIsAutoScroll(!autoScroll)
      }
    ),
    /* @__PURE__ */ We.createElement("label", { className: "form-check-label", htmlFor: "commentScrollSwitch" }, "\u81EA\u52D5\u30B9\u30AF\u30ED\u30FC\u30EB"),
    /* @__PURE__ */ We.createElement(
      "p",
      {
        className: "ngToggle",
        title: "NG\u8A2D\u5B9A",
        onClick: () => setIsNGExpanded(true)
      },
      /* @__PURE__ */ We.createElement("i", { className: "fa-regular fa-thumbs-down fa-lg" })
    )
  ), /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `${isExpanded ? "commentList" : "commentList collapsed"}`,
      ref,
      onWheel
    },
    /* @__PURE__ */ We.createElement("div", { className: "commentItem commentHeader" }, /* @__PURE__ */ We.createElement("div", { className: "commentContent" }, "\u30B3\u30E1\u30F3\u30C8"), /* @__PURE__ */ We.createElement("div", { className: "commentTime" }, "\u518D\u751F\u6642\u9593"), /* @__PURE__ */ We.createElement("div", { className: "commentNumber" }, "\u30B3\u30E1\u756A"), /* @__PURE__ */ We.createElement("div", { className: "commentPostedAt" }, "\u6295\u7A3F\u6642\u523B")),
    elements
  ));
};

// NiconicomeWeb/src/watch/ui/state/logger.ts
var LoggerImpl = class {
  listeners = {};
  _messages = [];
  get messages() {
    return this._messages;
  }
  log(message, space) {
    const log = this.createLog("log", message, space);
    this.writeLog(log);
  }
  error(message) {
    const log = this.createLog("error", message);
    this.writeLog(log);
  }
  warn(message) {
    const log = this.createLog("warn", message);
    this.writeLog(log);
  }
  debug(message) {
    if (false)
      return;
    const log = this.createLog("debug", message);
    this.writeLog(log);
  }
  clear() {
    this._messages.splice(0);
    if (this.listeners["write"] !== void 0) {
      this.listeners["write"].forEach((listener) => listener(null));
    }
  }
  addEventListner(event, listener) {
    if (this.listeners[event] === void 0) {
      this.listeners[event] = [];
    }
    this.listeners[event].push(listener);
  }
  removeEventListner(event, listener) {
    if (this.listeners[event] === void 0) {
      return;
    }
    this.listeners[event] = this.listeners[event].filter((l2) => l2 !== listener);
  }
  createLog(type, message, space) {
    return {
      message,
      type,
      time: /* @__PURE__ */ new Date(),
      border: space ?? false
    };
  }
  writeLog(log) {
    if (this.listeners["write"] !== void 0) {
      this.listeners["write"].forEach((listener) => listener(log));
    }
    this._messages.push(log);
  }
};
var Logger = new LoggerImpl();

// NiconicomeWeb/src/watch/ui/componetnts/playlist/playlistItem.tsx
var PlaylistItem = ({ video, baseURL }) => {
  const { state, dispatch } = Ie(VideoStateContext);
  const uploadedAt = new Date(video.uploadedAt);
  const formattedDate = `${uploadedAt.getFullYear()}/${(uploadedAt.getMonth() + 1).toString().padStart(2, "0")}/${uploadedAt.getDate().toString().padStart(2, "0")} ${uploadedAt.getHours().toString().padStart(2, "0")}:${uploadedAt.getMinutes().toString().padStart(2, "0")}`;
  let viewcount;
  if (video.viewCount > 1e4) {
    viewcount = `${Math.floor(video.viewCount / 1e3) / 10}\u4E07`;
  } else {
    viewcount = video.viewCount.toString();
  }
  function handleClick(niconicoID) {
    Logger.clear();
    globalThis.scrollTo(0, 0);
    Logger.log(`${niconicoID}\u306E\u8AAD\u307F\u8FBC\u307F\u3092\u958B\u59CB\u3057\u307E\u3059\u3002`, true);
    const url = `${baseURL}${niconicoID}.json`;
    fetch(url).then((res) => res.json()).then((data) => {
      dispatch({ "type": "jsWatchInfo", "payload": data });
      dispatch({ "type": "isPlaying", "payload": false });
    }).catch(() => {
      return;
    });
  }
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "playlistVideo",
      onClick: () => handleClick(video.niconicoID)
    },
    /* @__PURE__ */ We.createElement("div", { className: "thumbnail" }, /* @__PURE__ */ We.createElement("img", { src: video.thumbnailURL })),
    /* @__PURE__ */ We.createElement("div", { className: "videoInfo" }, /* @__PURE__ */ We.createElement("p", { className: "title" }, video.title), /* @__PURE__ */ We.createElement("p", { className: "other" }, /* @__PURE__ */ We.createElement("span", null, formattedDate, "\u6295\u7A3F\u30FB", viewcount, "\u56DE\u518D\u751F")))
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/playlist/playlist.tsx
var Playlist = ({ videos, baseURL }) => {
  return /* @__PURE__ */ We.createElement("div", { className: "playlist" }, videos.map((video) => /* @__PURE__ */ We.createElement(PlaylistItem, { video, baseURL })));
};

// NiconicomeWeb/src/watch/ui/componetnts/videoContextMenu/contextMenu.tsx
var ContextMenu = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  function onClick(e) {
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 }
    });
  }
  function openSystemMessage() {
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 }
    });
    dispatch({ type: "systemMessage", payload: true });
  }
  function openShortcut() {
    dispatch({
      type: "contextMenu",
      payload: { open: false, left: 0, top: 0 }
    });
    dispatch({ type: "shortcut", payload: true });
  }
  return /* @__PURE__ */ We.createElement("div", { className: "contextMenuWrapper", onClick: (e) => onClick(e) }, /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `contextMenu ${state.contextMenu.open ? "" : "hide"}`,
      style: {
        left: `${state.contextMenu.left}px`,
        top: `${state.contextMenu.top}px`
      }
    },
    /* @__PURE__ */ We.createElement("div", { className: "menuItem", onClick: openSystemMessage }, "\u30B7\u30B9\u30C6\u30E0\u30E1\u30C3\u30BB\u30FC\u30B8\u3092\u958B\u304F"),
    /* @__PURE__ */ We.createElement("div", { className: "menuItem", onClick: openShortcut }, "\u30B7\u30E7\u30FC\u30C8\u30AB\u30C3\u30C8\u4E00\u89A7\u3092\u958B\u304F")
  ));
};

// NiconicomeWeb/src/watch/ui/componetnts/shortcut/shortcut.tsx
var Shortcut = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  function closeShortcut() {
    dispatch({ type: "shortcut", payload: false });
  }
  return /* @__PURE__ */ We.createElement("div", { className: `shortcut ${state.isShortcutVisible ? "" : "hidden"}` }, /* @__PURE__ */ We.createElement("div", { className: "header" }, /* @__PURE__ */ We.createElement("p", null, "\u30B7\u30E7\u30FC\u30C8\u30AB\u30C3\u30C8\u30AD\u30FC \u4E00\u89A7"), /* @__PURE__ */ We.createElement("p", { className: "closeButton", onClick: closeShortcut }, /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-xmark fa-lg" }))), /* @__PURE__ */ We.createElement("div", { className: "content" }, /* @__PURE__ */ We.createElement("p", { className: "section" }, "\u52D5\u753B\u518D\u751F"), /* @__PURE__ */ We.createElement("div", { className: "item" }, /* @__PURE__ */ We.createElement("p", null, "\u518D\u751F\u30FB\u4E00\u6642\u505C\u6B62"), /* @__PURE__ */ We.createElement("p", null, "space / k")), /* @__PURE__ */ We.createElement("div", { className: "item" }, /* @__PURE__ */ We.createElement("p", null, "\u30B3\u30E1\u30F3\u30C8\u8868\u793A\u30FB\u975E\u8868\u793A"), /* @__PURE__ */ We.createElement("p", null, "c")), /* @__PURE__ */ We.createElement("div", { className: "item" }, /* @__PURE__ */ We.createElement("p", null, "10\u79D2\u9032\u3081\u308B"), /* @__PURE__ */ We.createElement("p", null, "\u2192 / l")), /* @__PURE__ */ We.createElement("div", { className: "item" }, /* @__PURE__ */ We.createElement("p", null, "10\u79D2\u623B\u3059"), /* @__PURE__ */ We.createElement("p", null, "\u2190 / j")), /* @__PURE__ */ We.createElement("div", { className: "item" }, /* @__PURE__ */ We.createElement("p", null, "\u52D5\u753B\u306E\u6700\u521D\u306B\u79FB\u52D5"), /* @__PURE__ */ We.createElement("p", null, "home"))));
};

// NiconicomeWeb/src/shared/Collection/unique.ts
function unique(array) {
  return array.filter((x3, i, self2) => self2.indexOf(x3) === i);
}

// NiconicomeWeb/src/watch/ui/comment/ng/ngHandler.ts
var NGHandlerImpl = class {
  _ngDataFetcher;
  _ngData;
  constructor(ngDataFetcher) {
    this._ngDataFetcher = ngDataFetcher;
  }
  async addNG(type, value) {
    await this._ngDataFetcher.addNG(type, value);
    if (this._ngData === void 0) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }
    if (type === "word") {
      this._ngData.words = unique([...this._ngData.words, value]);
    } else if (type === "user") {
      this._ngData.users = unique([...this._ngData.users, value]);
    } else if (type === "command") {
      this._ngData.commands = unique([...this._ngData.commands, value]);
    }
    Logger.log(`NG\u8A2D\u5B9A\u3092\u8FFD\u52A0\u3057\u307E\u3057\u305F\u3002\u7A2E\u985E:${type} \u5024:${value}`);
    return this._ngData;
  }
  async removeNG(type, value) {
    await this._ngDataFetcher.removeNG(type, value);
    if (this._ngData === void 0) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }
    if (type === "word") {
      this._ngData.words = this._ngData.words.filter((v3) => v3 !== value);
    } else if (type === "user") {
      this._ngData.users = this._ngData.users.filter((v3) => v3 !== value);
    } else if (type === "command") {
      this._ngData.commands = this._ngData.commands.filter((v3) => v3 !== value);
    }
    Logger.log(`NG\u8A2D\u5B9A\u3092\u524A\u9664\u3057\u307E\u3057\u305F\u3002\u7A2E\u985E:${type} \u5024:${value}`);
    return this._ngData;
  }
  async getNGData() {
    if (this._ngData === void 0) {
      this._ngData = await this._ngDataFetcher.getNGData();
    }
    return this._ngData;
  }
  async filterNG(comments) {
    if (this._ngData === void 0) {
      this._ngData = await this.getNGData();
    }
    let hit = 0;
    const filtered = comments.filter((comment) => {
      let result = true;
      this._ngData.words.forEach((word) => {
        if (comment.body.includes(word)) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${word}", type: word`);
          return;
        }
      });
      if (!result)
        return false;
      this._ngData.users.forEach((user) => {
        if (comment.userID === user) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${user}", type: user`);
          return;
        }
      });
      if (!result)
        return false;
      this._ngData.commands.forEach((command) => {
        if (comment.mail.includes(command)) {
          result = false;
          hit++;
          Logger.debug(`filtered: "${comment.body}" due to rule "${command}", type: command`);
          return;
        }
      });
      if (!result)
        return false;
      return true;
    });
    Logger.log(`NG\u30D5\u30A3\u30EB\u30BF\u30EA\u30F3\u30B0\u3092\u5B9F\u884C\u3057\u307E\u3057\u305F\u3002${comments.length}\u4EF6\u4E2D${hit}\u4EF6\u304C\u30D5\u30A3\u30EB\u30BF\u30EA\u30F3\u30B0\u3055\u308C\u307E\u3057\u305F\u3002`);
    return filtered.map((comment, i) => {
      comment.innnerIndex = i;
      return comment;
    });
  }
};

// NiconicomeWeb/src/watch/ui/comment/ng/ngDataFethcer.ts
var NGDataFetcherImpl = class {
  constructor(baseURL) {
    this._baseURL = baseURL;
  }
  _baseURL;
  async addNG(type, value) {
    const url = new URL(`${this._baseURL}/set`);
    const request = {
      type,
      value
    };
    await fetch(url.toString(), {
      method: "POST",
      body: JSON.stringify(request)
    });
  }
  async removeNG(type, value) {
    const url = new URL(`${this._baseURL}/delete`);
    const request = {
      type,
      value
    };
    await fetch(url.toString(), {
      method: "POST",
      body: JSON.stringify(request)
    });
  }
  async getNGData() {
    const url = new URL(`${this._baseURL}/get`);
    const response = await fetch(url.toString());
    const data = await response.json();
    return {
      words: data.words,
      users: data.users,
      commands: data.commands
    };
  }
};

// https://esm.sh/v135/hls.js@1.5.8/denonext/hls.mjs
function Kn(n12) {
  return n12 && n12.__esModule && Object.prototype.hasOwnProperty.call(n12, "default") ? n12.default : n12;
}
var Br = { exports: {} };
(function(n12, e) {
  (function(t) {
    var s2 = /^(?=((?:[a-zA-Z0-9+\-.]+:)?))\1(?=((?:\/\/[^\/?#]*)?))\2(?=((?:(?:[^?#\/]*\/)*[^;?#\/]*)?))\3((?:;[^?#]*)?)(\?[^#]*)?(#[^]*)?$/, i = /^(?=([^\/?#]*))\1([^]*)$/, r2 = /(?:\/|^)\.(?=\/)/g, a2 = /(?:\/|^)\.\.\/(?!\.\.\/)[^\/]*(?=\/)/g, o = { buildAbsoluteURL: function(l2, c, h2) {
      if (h2 = h2 || {}, l2 = l2.trim(), c = c.trim(), !c) {
        if (!h2.alwaysNormalize)
          return l2;
        var u2 = o.parseURL(l2);
        if (!u2)
          throw new Error("Error trying to parse base URL.");
        return u2.path = o.normalizePath(u2.path), o.buildURLFromParts(u2);
      }
      var d3 = o.parseURL(c);
      if (!d3)
        throw new Error("Error trying to parse relative URL.");
      if (d3.scheme)
        return h2.alwaysNormalize ? (d3.path = o.normalizePath(d3.path), o.buildURLFromParts(d3)) : c;
      var f3 = o.parseURL(l2);
      if (!f3)
        throw new Error("Error trying to parse base URL.");
      if (!f3.netLoc && f3.path && f3.path[0] !== "/") {
        var g2 = i.exec(f3.path);
        f3.netLoc = g2[1], f3.path = g2[2];
      }
      f3.netLoc && !f3.path && (f3.path = "/");
      var p3 = { scheme: f3.scheme, netLoc: d3.netLoc, path: null, params: d3.params, query: d3.query, fragment: d3.fragment };
      if (!d3.netLoc && (p3.netLoc = f3.netLoc, d3.path[0] !== "/"))
        if (!d3.path)
          p3.path = f3.path, d3.params || (p3.params = f3.params, d3.query || (p3.query = f3.query));
        else {
          var T2 = f3.path, E4 = T2.substring(0, T2.lastIndexOf("/") + 1) + d3.path;
          p3.path = o.normalizePath(E4);
        }
      return p3.path === null && (p3.path = h2.alwaysNormalize ? o.normalizePath(d3.path) : d3.path), o.buildURLFromParts(p3);
    }, parseURL: function(l2) {
      var c = s2.exec(l2);
      return c ? { scheme: c[1] || "", netLoc: c[2] || "", path: c[3] || "", params: c[4] || "", query: c[5] || "", fragment: c[6] || "" } : null;
    }, normalizePath: function(l2) {
      for (l2 = l2.split("").reverse().join("").replace(r2, ""); l2.length !== (l2 = l2.replace(a2, "")).length; )
        ;
      return l2.split("").reverse().join("");
    }, buildURLFromParts: function(l2) {
      return l2.scheme + l2.netLoc + l2.path + l2.params + l2.query + l2.fragment;
    } };
    n12.exports = o;
  })();
})(Br);
var wi = Br.exports;
function Ki(n12, e) {
  var t = Object.keys(n12);
  if (Object.getOwnPropertySymbols) {
    var s2 = Object.getOwnPropertySymbols(n12);
    e && (s2 = s2.filter(function(i) {
      return Object.getOwnPropertyDescriptor(n12, i).enumerable;
    })), t.push.apply(t, s2);
  }
  return t;
}
function le3(n12) {
  for (var e = 1; e < arguments.length; e++) {
    var t = arguments[e] != null ? arguments[e] : {};
    e % 2 ? Ki(Object(t), true).forEach(function(s2) {
      Wn(n12, s2, t[s2]);
    }) : Object.getOwnPropertyDescriptors ? Object.defineProperties(n12, Object.getOwnPropertyDescriptors(t)) : Ki(Object(t)).forEach(function(s2) {
      Object.defineProperty(n12, s2, Object.getOwnPropertyDescriptor(t, s2));
    });
  }
  return n12;
}
function Hn(n12, e) {
  if (typeof n12 != "object" || !n12)
    return n12;
  var t = n12[Symbol.toPrimitive];
  if (t !== void 0) {
    var s2 = t.call(n12, e || "default");
    if (typeof s2 != "object")
      return s2;
    throw new TypeError("@@toPrimitive must return a primitive value.");
  }
  return (e === "string" ? String : Number)(n12);
}
function Vn(n12) {
  var e = Hn(n12, "string");
  return typeof e == "symbol" ? e : String(e);
}
function Wn(n12, e, t) {
  return e = Vn(e), e in n12 ? Object.defineProperty(n12, e, { value: t, enumerable: true, configurable: true, writable: true }) : n12[e] = t, n12;
}
function te2() {
  return te2 = Object.assign ? Object.assign.bind() : function(n12) {
    for (var e = 1; e < arguments.length; e++) {
      var t = arguments[e];
      for (var s2 in t)
        Object.prototype.hasOwnProperty.call(t, s2) && (n12[s2] = t[s2]);
    }
    return n12;
  }, te2.apply(this, arguments);
}
var F = Number.isFinite || function(n12) {
  return typeof n12 == "number" && isFinite(n12);
};
var Yn = Number.isSafeInteger || function(n12) {
  return typeof n12 == "number" && Math.abs(n12) <= qn;
};
var qn = Number.MAX_SAFE_INTEGER || 9007199254740991;
var m2 = function(n12) {
  return n12.MEDIA_ATTACHING = "hlsMediaAttaching", n12.MEDIA_ATTACHED = "hlsMediaAttached", n12.MEDIA_DETACHING = "hlsMediaDetaching", n12.MEDIA_DETACHED = "hlsMediaDetached", n12.BUFFER_RESET = "hlsBufferReset", n12.BUFFER_CODECS = "hlsBufferCodecs", n12.BUFFER_CREATED = "hlsBufferCreated", n12.BUFFER_APPENDING = "hlsBufferAppending", n12.BUFFER_APPENDED = "hlsBufferAppended", n12.BUFFER_EOS = "hlsBufferEos", n12.BUFFER_FLUSHING = "hlsBufferFlushing", n12.BUFFER_FLUSHED = "hlsBufferFlushed", n12.MANIFEST_LOADING = "hlsManifestLoading", n12.MANIFEST_LOADED = "hlsManifestLoaded", n12.MANIFEST_PARSED = "hlsManifestParsed", n12.LEVEL_SWITCHING = "hlsLevelSwitching", n12.LEVEL_SWITCHED = "hlsLevelSwitched", n12.LEVEL_LOADING = "hlsLevelLoading", n12.LEVEL_LOADED = "hlsLevelLoaded", n12.LEVEL_UPDATED = "hlsLevelUpdated", n12.LEVEL_PTS_UPDATED = "hlsLevelPtsUpdated", n12.LEVELS_UPDATED = "hlsLevelsUpdated", n12.AUDIO_TRACKS_UPDATED = "hlsAudioTracksUpdated", n12.AUDIO_TRACK_SWITCHING = "hlsAudioTrackSwitching", n12.AUDIO_TRACK_SWITCHED = "hlsAudioTrackSwitched", n12.AUDIO_TRACK_LOADING = "hlsAudioTrackLoading", n12.AUDIO_TRACK_LOADED = "hlsAudioTrackLoaded", n12.SUBTITLE_TRACKS_UPDATED = "hlsSubtitleTracksUpdated", n12.SUBTITLE_TRACKS_CLEARED = "hlsSubtitleTracksCleared", n12.SUBTITLE_TRACK_SWITCH = "hlsSubtitleTrackSwitch", n12.SUBTITLE_TRACK_LOADING = "hlsSubtitleTrackLoading", n12.SUBTITLE_TRACK_LOADED = "hlsSubtitleTrackLoaded", n12.SUBTITLE_FRAG_PROCESSED = "hlsSubtitleFragProcessed", n12.CUES_PARSED = "hlsCuesParsed", n12.NON_NATIVE_TEXT_TRACKS_FOUND = "hlsNonNativeTextTracksFound", n12.INIT_PTS_FOUND = "hlsInitPtsFound", n12.FRAG_LOADING = "hlsFragLoading", n12.FRAG_LOAD_EMERGENCY_ABORTED = "hlsFragLoadEmergencyAborted", n12.FRAG_LOADED = "hlsFragLoaded", n12.FRAG_DECRYPTED = "hlsFragDecrypted", n12.FRAG_PARSING_INIT_SEGMENT = "hlsFragParsingInitSegment", n12.FRAG_PARSING_USERDATA = "hlsFragParsingUserdata", n12.FRAG_PARSING_METADATA = "hlsFragParsingMetadata", n12.FRAG_PARSED = "hlsFragParsed", n12.FRAG_BUFFERED = "hlsFragBuffered", n12.FRAG_CHANGED = "hlsFragChanged", n12.FPS_DROP = "hlsFpsDrop", n12.FPS_DROP_LEVEL_CAPPING = "hlsFpsDropLevelCapping", n12.MAX_AUTO_LEVEL_UPDATED = "hlsMaxAutoLevelUpdated", n12.ERROR = "hlsError", n12.DESTROYING = "hlsDestroying", n12.KEY_LOADING = "hlsKeyLoading", n12.KEY_LOADED = "hlsKeyLoaded", n12.LIVE_BACK_BUFFER_REACHED = "hlsLiveBackBufferReached", n12.BACK_BUFFER_REACHED = "hlsBackBufferReached", n12.STEERING_MANIFEST_LOADED = "hlsSteeringManifestLoaded", n12;
}({});
var B2 = function(n12) {
  return n12.NETWORK_ERROR = "networkError", n12.MEDIA_ERROR = "mediaError", n12.KEY_SYSTEM_ERROR = "keySystemError", n12.MUX_ERROR = "muxError", n12.OTHER_ERROR = "otherError", n12;
}({});
var A2 = function(n12) {
  return n12.KEY_SYSTEM_NO_KEYS = "keySystemNoKeys", n12.KEY_SYSTEM_NO_ACCESS = "keySystemNoAccess", n12.KEY_SYSTEM_NO_SESSION = "keySystemNoSession", n12.KEY_SYSTEM_NO_CONFIGURED_LICENSE = "keySystemNoConfiguredLicense", n12.KEY_SYSTEM_LICENSE_REQUEST_FAILED = "keySystemLicenseRequestFailed", n12.KEY_SYSTEM_SERVER_CERTIFICATE_REQUEST_FAILED = "keySystemServerCertificateRequestFailed", n12.KEY_SYSTEM_SERVER_CERTIFICATE_UPDATE_FAILED = "keySystemServerCertificateUpdateFailed", n12.KEY_SYSTEM_SESSION_UPDATE_FAILED = "keySystemSessionUpdateFailed", n12.KEY_SYSTEM_STATUS_OUTPUT_RESTRICTED = "keySystemStatusOutputRestricted", n12.KEY_SYSTEM_STATUS_INTERNAL_ERROR = "keySystemStatusInternalError", n12.MANIFEST_LOAD_ERROR = "manifestLoadError", n12.MANIFEST_LOAD_TIMEOUT = "manifestLoadTimeOut", n12.MANIFEST_PARSING_ERROR = "manifestParsingError", n12.MANIFEST_INCOMPATIBLE_CODECS_ERROR = "manifestIncompatibleCodecsError", n12.LEVEL_EMPTY_ERROR = "levelEmptyError", n12.LEVEL_LOAD_ERROR = "levelLoadError", n12.LEVEL_LOAD_TIMEOUT = "levelLoadTimeOut", n12.LEVEL_PARSING_ERROR = "levelParsingError", n12.LEVEL_SWITCH_ERROR = "levelSwitchError", n12.AUDIO_TRACK_LOAD_ERROR = "audioTrackLoadError", n12.AUDIO_TRACK_LOAD_TIMEOUT = "audioTrackLoadTimeOut", n12.SUBTITLE_LOAD_ERROR = "subtitleTrackLoadError", n12.SUBTITLE_TRACK_LOAD_TIMEOUT = "subtitleTrackLoadTimeOut", n12.FRAG_LOAD_ERROR = "fragLoadError", n12.FRAG_LOAD_TIMEOUT = "fragLoadTimeOut", n12.FRAG_DECRYPT_ERROR = "fragDecryptError", n12.FRAG_PARSING_ERROR = "fragParsingError", n12.FRAG_GAP = "fragGap", n12.REMUX_ALLOC_ERROR = "remuxAllocError", n12.KEY_LOAD_ERROR = "keyLoadError", n12.KEY_LOAD_TIMEOUT = "keyLoadTimeOut", n12.BUFFER_ADD_CODEC_ERROR = "bufferAddCodecError", n12.BUFFER_INCOMPATIBLE_CODECS_ERROR = "bufferIncompatibleCodecsError", n12.BUFFER_APPEND_ERROR = "bufferAppendError", n12.BUFFER_APPENDING_ERROR = "bufferAppendingError", n12.BUFFER_STALLED_ERROR = "bufferStalledError", n12.BUFFER_FULL_ERROR = "bufferFullError", n12.BUFFER_SEEK_OVER_HOLE = "bufferSeekOverHole", n12.BUFFER_NUDGE_ON_STALL = "bufferNudgeOnStall", n12.INTERNAL_EXCEPTION = "internalException", n12.INTERNAL_ABORTED = "aborted", n12.UNKNOWN = "unknown", n12;
}({});
var Fe2 = function() {
};
var xs = { trace: Fe2, debug: Fe2, log: Fe2, warn: Fe2, info: Fe2, error: Fe2 };
var Ze = xs;
function jn(n12) {
  let e = self.console[n12];
  return e ? e.bind(self.console, `[${n12}] >`) : Fe2;
}
function zn(n12, ...e) {
  e.forEach(function(t) {
    Ze[t] = n12[t] ? n12[t].bind(n12) : jn(t);
  });
}
function Xn(n12, e) {
  if (typeof console == "object" && n12 === true || typeof n12 == "object") {
    zn(n12, "debug", "log", "info", "warn", "error");
    try {
      Ze.log(`Debug logs enabled for "${e}" in hls.js version 1.5.8`);
    } catch {
      Ze = xs;
    }
  } else
    Ze = xs;
}
var v2 = Ze;
var Qn = /^(\d+)x(\d+)$/;
var Hi = /(.+?)=(".*?"|.*?)(?:,|$)/g;
var Z2 = class n2 {
  constructor(e) {
    typeof e == "string" && (e = n2.parseAttrList(e)), te2(this, e);
  }
  get clientAttrs() {
    return Object.keys(this).filter((e) => e.substring(0, 2) === "X-");
  }
  decimalInteger(e) {
    let t = parseInt(this[e], 10);
    return t > Number.MAX_SAFE_INTEGER ? 1 / 0 : t;
  }
  hexadecimalInteger(e) {
    if (this[e]) {
      let t = (this[e] || "0x").slice(2);
      t = (t.length & 1 ? "0" : "") + t;
      let s2 = new Uint8Array(t.length / 2);
      for (let i = 0; i < t.length / 2; i++)
        s2[i] = parseInt(t.slice(i * 2, i * 2 + 2), 16);
      return s2;
    } else
      return null;
  }
  hexadecimalIntegerAsNumber(e) {
    let t = parseInt(this[e], 16);
    return t > Number.MAX_SAFE_INTEGER ? 1 / 0 : t;
  }
  decimalFloatingPoint(e) {
    return parseFloat(this[e]);
  }
  optionalFloat(e, t) {
    let s2 = this[e];
    return s2 ? parseFloat(s2) : t;
  }
  enumeratedString(e) {
    return this[e];
  }
  bool(e) {
    return this[e] === "YES";
  }
  decimalResolution(e) {
    let t = Qn.exec(this[e]);
    if (t !== null)
      return { width: parseInt(t[1], 10), height: parseInt(t[2], 10) };
  }
  static parseAttrList(e) {
    let t, s2 = {}, i = '"';
    for (Hi.lastIndex = 0; (t = Hi.exec(e)) !== null; ) {
      let r2 = t[2];
      r2.indexOf(i) === 0 && r2.lastIndexOf(i) === r2.length - 1 && (r2 = r2.slice(1, -1));
      let a2 = t[1].trim();
      s2[a2] = r2;
    }
    return s2;
  }
};
function Jn(n12) {
  return n12 !== "ID" && n12 !== "CLASS" && n12 !== "START-DATE" && n12 !== "DURATION" && n12 !== "END-DATE" && n12 !== "END-ON-NEXT";
}
function Zn(n12) {
  return n12 === "SCTE35-OUT" || n12 === "SCTE35-IN";
}
var bt = class {
  constructor(e, t) {
    if (this.attr = void 0, this._startDate = void 0, this._endDate = void 0, this._badValueForSameId = void 0, t) {
      let s2 = t.attr;
      for (let i in s2)
        if (Object.prototype.hasOwnProperty.call(e, i) && e[i] !== s2[i]) {
          v2.warn(`DATERANGE tag attribute: "${i}" does not match for tags with ID: "${e.ID}"`), this._badValueForSameId = i;
          break;
        }
      e = te2(new Z2({}), s2, e);
    }
    if (this.attr = e, this._startDate = new Date(e["START-DATE"]), "END-DATE" in this.attr) {
      let s2 = new Date(this.attr["END-DATE"]);
      F(s2.getTime()) && (this._endDate = s2);
    }
  }
  get id() {
    return this.attr.ID;
  }
  get class() {
    return this.attr.CLASS;
  }
  get startDate() {
    return this._startDate;
  }
  get endDate() {
    if (this._endDate)
      return this._endDate;
    let e = this.duration;
    return e !== null ? new Date(this._startDate.getTime() + e * 1e3) : null;
  }
  get duration() {
    if ("DURATION" in this.attr) {
      let e = this.attr.decimalFloatingPoint("DURATION");
      if (F(e))
        return e;
    } else if (this._endDate)
      return (this._endDate.getTime() - this._startDate.getTime()) / 1e3;
    return null;
  }
  get plannedDuration() {
    return "PLANNED-DURATION" in this.attr ? this.attr.decimalFloatingPoint("PLANNED-DURATION") : null;
  }
  get endOnNext() {
    return this.attr.bool("END-ON-NEXT");
  }
  get isValid() {
    return !!this.id && !this._badValueForSameId && F(this.startDate.getTime()) && (this.duration === null || this.duration >= 0) && (!this.endOnNext || !!this.class);
  }
};
var je2 = class {
  constructor() {
    this.aborted = false, this.loaded = 0, this.retry = 0, this.total = 0, this.chunkCount = 0, this.bwEstimate = 0, this.loading = { start: 0, first: 0, end: 0 }, this.parsing = { start: 0, end: 0 }, this.buffering = { start: 0, first: 0, end: 0 };
  }
};
var z2 = { AUDIO: "audio", VIDEO: "video", AUDIOVIDEO: "audiovideo" };
var Dt = class {
  constructor(e) {
    this._byteRange = null, this._url = null, this.baseurl = void 0, this.relurl = void 0, this.elementaryStreams = { [z2.AUDIO]: null, [z2.VIDEO]: null, [z2.AUDIOVIDEO]: null }, this.baseurl = e;
  }
  setByteRange(e, t) {
    let s2 = e.split("@", 2), i;
    s2.length === 1 ? i = t?.byteRangeEndOffset || 0 : i = parseInt(s2[1]), this._byteRange = [i, parseInt(s2[0]) + i];
  }
  get byteRange() {
    return this._byteRange ? this._byteRange : [];
  }
  get byteRangeStartOffset() {
    return this.byteRange[0];
  }
  get byteRangeEndOffset() {
    return this.byteRange[1];
  }
  get url() {
    return !this._url && this.baseurl && this.relurl && (this._url = wi.buildAbsoluteURL(this.baseurl, this.relurl, { alwaysNormalize: true })), this._url || "";
  }
  set url(e) {
    this._url = e;
  }
};
var et = class extends Dt {
  constructor(e, t) {
    super(t), this._decryptdata = null, this.rawProgramDateTime = null, this.programDateTime = null, this.tagList = [], this.duration = 0, this.sn = 0, this.levelkeys = void 0, this.type = void 0, this.loader = null, this.keyLoader = null, this.level = -1, this.cc = 0, this.startPTS = void 0, this.endPTS = void 0, this.startDTS = void 0, this.endDTS = void 0, this.start = 0, this.deltaPTS = void 0, this.maxStartPTS = void 0, this.minEndPTS = void 0, this.stats = new je2(), this.data = void 0, this.bitrateTest = false, this.title = null, this.initSegment = null, this.endList = void 0, this.gap = void 0, this.urlId = 0, this.type = e;
  }
  get decryptdata() {
    let { levelkeys: e } = this;
    if (!e && !this._decryptdata)
      return null;
    if (!this._decryptdata && this.levelkeys && !this.levelkeys.NONE) {
      let t = this.levelkeys.identity;
      if (t)
        this._decryptdata = t.getDecryptData(this.sn);
      else {
        let s2 = Object.keys(this.levelkeys);
        if (s2.length === 1)
          return this._decryptdata = this.levelkeys[s2[0]].getDecryptData(this.sn);
      }
    }
    return this._decryptdata;
  }
  get end() {
    return this.start + this.duration;
  }
  get endProgramDateTime() {
    if (this.programDateTime === null || !F(this.programDateTime))
      return null;
    let e = F(this.duration) ? this.duration : 0;
    return this.programDateTime + e * 1e3;
  }
  get encrypted() {
    var e;
    if ((e = this._decryptdata) != null && e.encrypted)
      return true;
    if (this.levelkeys) {
      let t = Object.keys(this.levelkeys), s2 = t.length;
      if (s2 > 1 || s2 === 1 && this.levelkeys[t[0]].encrypted)
        return true;
    }
    return false;
  }
  setKeyFormat(e) {
    if (this.levelkeys) {
      let t = this.levelkeys[e];
      t && !this._decryptdata && (this._decryptdata = t.getDecryptData(this.sn));
    }
  }
  abortRequests() {
    var e, t;
    (e = this.loader) == null || e.abort(), (t = this.keyLoader) == null || t.abort();
  }
  setElementaryStreamInfo(e, t, s2, i, r2, a2 = false) {
    let { elementaryStreams: o } = this, l2 = o[e];
    if (!l2) {
      o[e] = { startPTS: t, endPTS: s2, startDTS: i, endDTS: r2, partial: a2 };
      return;
    }
    l2.startPTS = Math.min(l2.startPTS, t), l2.endPTS = Math.max(l2.endPTS, s2), l2.startDTS = Math.min(l2.startDTS, i), l2.endDTS = Math.max(l2.endDTS, r2);
  }
  clearElementaryStreamInfo() {
    let { elementaryStreams: e } = this;
    e[z2.AUDIO] = null, e[z2.VIDEO] = null, e[z2.AUDIOVIDEO] = null;
  }
};
var Ss = class extends Dt {
  constructor(e, t, s2, i, r2) {
    super(s2), this.fragOffset = 0, this.duration = 0, this.gap = false, this.independent = false, this.relurl = void 0, this.fragment = void 0, this.index = void 0, this.stats = new je2(), this.duration = e.decimalFloatingPoint("DURATION"), this.gap = e.bool("GAP"), this.independent = e.bool("INDEPENDENT"), this.relurl = e.enumeratedString("URI"), this.fragment = t, this.index = i;
    let a2 = e.enumeratedString("BYTERANGE");
    a2 && this.setByteRange(a2, r2), r2 && (this.fragOffset = r2.fragOffset + r2.duration);
  }
  get start() {
    return this.fragment.start + this.fragOffset;
  }
  get end() {
    return this.start + this.duration;
  }
  get loaded() {
    let { elementaryStreams: e } = this;
    return !!(e.audio || e.video || e.audiovideo);
  }
};
var ea = 10;
var vs = class {
  constructor(e) {
    this.PTSKnown = false, this.alignedSliding = false, this.averagetargetduration = void 0, this.endCC = 0, this.endSN = 0, this.fragments = void 0, this.fragmentHint = void 0, this.partList = null, this.dateRanges = void 0, this.live = true, this.ageHeader = 0, this.advancedDateTime = void 0, this.updated = true, this.advanced = true, this.availabilityDelay = void 0, this.misses = 0, this.startCC = 0, this.startSN = 0, this.startTimeOffset = null, this.targetduration = 0, this.totalduration = 0, this.type = null, this.url = void 0, this.m3u8 = "", this.version = null, this.canBlockReload = false, this.canSkipUntil = 0, this.canSkipDateRanges = false, this.skippedSegments = 0, this.recentlyRemovedDateranges = void 0, this.partHoldBack = 0, this.holdBack = 0, this.partTarget = 0, this.preloadHint = void 0, this.renditionReports = void 0, this.tuneInGoal = 0, this.deltaUpdateFailed = void 0, this.driftStartTime = 0, this.driftEndTime = 0, this.driftStart = 0, this.driftEnd = 0, this.encryptedFragments = void 0, this.playlistParsingError = null, this.variableList = null, this.hasVariableRefs = false, this.fragments = [], this.encryptedFragments = [], this.dateRanges = {}, this.url = e;
  }
  reloaded(e) {
    if (!e) {
      this.advanced = true, this.updated = true;
      return;
    }
    let t = this.lastPartSn - e.lastPartSn, s2 = this.lastPartIndex - e.lastPartIndex;
    this.updated = this.endSN !== e.endSN || !!s2 || !!t || !this.live, this.advanced = this.endSN > e.endSN || t > 0 || t === 0 && s2 > 0, this.updated || this.advanced ? this.misses = Math.floor(e.misses * 0.6) : this.misses = e.misses + 1, this.availabilityDelay = e.availabilityDelay;
  }
  get hasProgramDateTime() {
    return this.fragments.length ? F(this.fragments[this.fragments.length - 1].programDateTime) : false;
  }
  get levelTargetDuration() {
    return this.averagetargetduration || this.targetduration || ea;
  }
  get drift() {
    let e = this.driftEndTime - this.driftStartTime;
    return e > 0 ? (this.driftEnd - this.driftStart) * 1e3 / e : 1;
  }
  get edge() {
    return this.partEnd || this.fragmentEnd;
  }
  get partEnd() {
    var e;
    return (e = this.partList) != null && e.length ? this.partList[this.partList.length - 1].end : this.fragmentEnd;
  }
  get fragmentEnd() {
    var e;
    return (e = this.fragments) != null && e.length ? this.fragments[this.fragments.length - 1].end : 0;
  }
  get age() {
    return this.advancedDateTime ? Math.max(Date.now() - this.advancedDateTime, 0) / 1e3 : 0;
  }
  get lastPartIndex() {
    var e;
    return (e = this.partList) != null && e.length ? this.partList[this.partList.length - 1].index : -1;
  }
  get lastPartSn() {
    var e;
    return (e = this.partList) != null && e.length ? this.partList[this.partList.length - 1].fragment.sn : this.endSN;
  }
};
function _i(n12) {
  return Uint8Array.from(atob(n12), (e) => e.charCodeAt(0));
}
function ta(n12) {
  let e = As(n12).subarray(0, 16), t = new Uint8Array(16);
  return t.set(e, 16 - e.length), t;
}
function sa(n12) {
  let e = function(s2, i, r2) {
    let a2 = s2[i];
    s2[i] = s2[r2], s2[r2] = a2;
  };
  e(n12, 0, 3), e(n12, 1, 2), e(n12, 4, 5), e(n12, 6, 7);
}
function ia(n12) {
  let e = n12.split(":"), t = null;
  if (e[0] === "data" && e.length === 2) {
    let s2 = e[1].split(";"), i = s2[s2.length - 1].split(",");
    if (i.length === 2) {
      let r2 = i[0] === "base64", a2 = i[1];
      r2 ? (s2.splice(-1, 1), t = _i(a2)) : t = ta(a2);
    }
  }
  return t;
}
function As(n12) {
  return Uint8Array.from(unescape(encodeURIComponent(n12)), (e) => e.charCodeAt(0));
}
var ze2 = typeof self < "u" ? self : void 0;
var J = { CLEARKEY: "org.w3.clearkey", FAIRPLAY: "com.apple.fps", PLAYREADY: "com.microsoft.playready", WIDEVINE: "com.widevine.alpha" };
var fe2 = { CLEARKEY: "org.w3.clearkey", FAIRPLAY: "com.apple.streamingkeydelivery", PLAYREADY: "com.microsoft.playready", WIDEVINE: "urn:uuid:edef8ba9-79d6-4ace-a3c8-27dcd51d21ed" };
function Vi(n12) {
  switch (n12) {
    case fe2.FAIRPLAY:
      return J.FAIRPLAY;
    case fe2.PLAYREADY:
      return J.PLAYREADY;
    case fe2.WIDEVINE:
      return J.WIDEVINE;
    case fe2.CLEARKEY:
      return J.CLEARKEY;
  }
}
var $r = { WIDEVINE: "edef8ba979d64acea3c827dcd51d21ed" };
function ra(n12) {
  if (n12 === $r.WIDEVINE)
    return J.WIDEVINE;
}
function Wi(n12) {
  switch (n12) {
    case J.FAIRPLAY:
      return fe2.FAIRPLAY;
    case J.PLAYREADY:
      return fe2.PLAYREADY;
    case J.WIDEVINE:
      return fe2.WIDEVINE;
    case J.CLEARKEY:
      return fe2.CLEARKEY;
  }
}
function ts(n12) {
  let { drmSystems: e, widevineLicenseUrl: t } = n12, s2 = e ? [J.FAIRPLAY, J.WIDEVINE, J.PLAYREADY, J.CLEARKEY].filter((i) => !!e[i]) : [];
  return !s2[J.WIDEVINE] && t && s2.push(J.WIDEVINE), s2;
}
var Gr = function(n12) {
  return ze2 != null && (n12 = ze2.navigator) != null && n12.requestMediaKeySystemAccess ? self.navigator.requestMediaKeySystemAccess.bind(self.navigator) : null;
}();
function na(n12, e, t, s2) {
  let i;
  switch (n12) {
    case J.FAIRPLAY:
      i = ["cenc", "sinf"];
      break;
    case J.WIDEVINE:
    case J.PLAYREADY:
      i = ["cenc"];
      break;
    case J.CLEARKEY:
      i = ["cenc", "keyids"];
      break;
    default:
      throw new Error(`Unknown key-system: ${n12}`);
  }
  return aa(i, e, t, s2);
}
function aa(n12, e, t, s2) {
  return [{ initDataTypes: n12, persistentState: s2.persistentState || "optional", distinctiveIdentifier: s2.distinctiveIdentifier || "optional", sessionTypes: s2.sessionTypes || [s2.sessionType || "temporary"], audioCapabilities: e.map((r2) => ({ contentType: `audio/mp4; codecs="${r2}"`, robustness: s2.audioRobustness || "", encryptionScheme: s2.audioEncryptionScheme || null })), videoCapabilities: t.map((r2) => ({ contentType: `video/mp4; codecs="${r2}"`, robustness: s2.videoRobustness || "", encryptionScheme: s2.videoEncryptionScheme || null })) }];
}
function Ne2(n12, e, t) {
  return Uint8Array.prototype.slice ? n12.slice(e, t) : new Uint8Array(Array.prototype.slice.call(n12, e, t));
}
var Pi = (n12, e) => e + 10 <= n12.length && n12[e] === 73 && n12[e + 1] === 68 && n12[e + 2] === 51 && n12[e + 3] < 255 && n12[e + 4] < 255 && n12[e + 6] < 128 && n12[e + 7] < 128 && n12[e + 8] < 128 && n12[e + 9] < 128;
var Kr = (n12, e) => e + 10 <= n12.length && n12[e] === 51 && n12[e + 1] === 68 && n12[e + 2] === 73 && n12[e + 3] < 255 && n12[e + 4] < 255 && n12[e + 6] < 128 && n12[e + 7] < 128 && n12[e + 8] < 128 && n12[e + 9] < 128;
var st = (n12, e) => {
  let t = e, s2 = 0;
  for (; Pi(n12, e); ) {
    s2 += 10;
    let i = Zt(n12, e + 6);
    s2 += i, Kr(n12, e + 10) && (s2 += 10), e += s2;
  }
  if (s2 > 0)
    return n12.subarray(t, t + s2);
};
var Zt = (n12, e) => {
  let t = 0;
  return t = (n12[e] & 127) << 21, t |= (n12[e + 1] & 127) << 14, t |= (n12[e + 2] & 127) << 7, t |= n12[e + 3] & 127, t;
};
var oa = (n12, e) => Pi(n12, e) && Zt(n12, e + 6) + 10 <= n12.length - e;
var ki = (n12) => {
  let e = Vr(n12);
  for (let t = 0; t < e.length; t++) {
    let s2 = e[t];
    if (Hr(s2))
      return fa(s2);
  }
};
var Hr = (n12) => n12 && n12.key === "PRIV" && n12.info === "com.apple.streaming.transportStreamTimestamp";
var la = (n12) => {
  let e = String.fromCharCode(n12[0], n12[1], n12[2], n12[3]), t = Zt(n12, 4), s2 = 10;
  return { type: e, size: t, data: n12.subarray(s2, s2 + t) };
};
var Vr = (n12) => {
  let e = 0, t = [];
  for (; Pi(n12, e); ) {
    let s2 = Zt(n12, e + 6);
    e += 10;
    let i = e + s2;
    for (; e + 8 < i; ) {
      let r2 = la(n12.subarray(e)), a2 = ca(r2);
      a2 && t.push(a2), e += r2.size + 10;
    }
    Kr(n12, e) && (e += 10);
  }
  return t;
};
var ca = (n12) => n12.type === "PRIV" ? ha(n12) : n12.type[0] === "W" ? da(n12) : ua(n12);
var ha = (n12) => {
  if (n12.size < 2)
    return;
  let e = Re2(n12.data, true), t = new Uint8Array(n12.data.subarray(e.length + 1));
  return { key: n12.type, info: e, data: t.buffer };
};
var ua = (n12) => {
  if (n12.size < 2)
    return;
  if (n12.type === "TXXX") {
    let t = 1, s2 = Re2(n12.data.subarray(t), true);
    t += s2.length + 1;
    let i = Re2(n12.data.subarray(t));
    return { key: n12.type, info: s2, data: i };
  }
  let e = Re2(n12.data.subarray(1));
  return { key: n12.type, data: e };
};
var da = (n12) => {
  if (n12.type === "WXXX") {
    if (n12.size < 2)
      return;
    let t = 1, s2 = Re2(n12.data.subarray(t), true);
    t += s2.length + 1;
    let i = Re2(n12.data.subarray(t));
    return { key: n12.type, info: s2, data: i };
  }
  let e = Re2(n12.data);
  return { key: n12.type, data: e };
};
var fa = (n12) => {
  if (n12.data.byteLength === 8) {
    let e = new Uint8Array(n12.data), t = e[3] & 1, s2 = (e[4] << 23) + (e[5] << 15) + (e[6] << 7) + e[7];
    return s2 /= 45, t && (s2 += 4772185884e-2), Math.round(s2);
  }
};
var Re2 = (n12, e = false) => {
  let t = ga();
  if (t) {
    let c = t.decode(n12);
    if (e) {
      let h2 = c.indexOf("\0");
      return h2 !== -1 ? c.substring(0, h2) : c;
    }
    return c.replace(/\0/g, "");
  }
  let s2 = n12.length, i, r2, a2, o = "", l2 = 0;
  for (; l2 < s2; ) {
    if (i = n12[l2++], i === 0 && e)
      return o;
    if (i === 0 || i === 3)
      continue;
    switch (i >> 4) {
      case 0:
      case 1:
      case 2:
      case 3:
      case 4:
      case 5:
      case 6:
      case 7:
        o += String.fromCharCode(i);
        break;
      case 12:
      case 13:
        r2 = n12[l2++], o += String.fromCharCode((i & 31) << 6 | r2 & 63);
        break;
      case 14:
        r2 = n12[l2++], a2 = n12[l2++], o += String.fromCharCode((i & 15) << 12 | (r2 & 63) << 6 | (a2 & 63) << 0);
        break;
    }
  }
  return o;
};
var ss;
function ga() {
  if (!navigator.userAgent.includes("PlayStation 4"))
    return !ss && typeof self.TextDecoder < "u" && (ss = new self.TextDecoder("utf-8")), ss;
}
var ve3 = { hexDump: function(n12) {
  let e = "";
  for (let t = 0; t < n12.length; t++) {
    let s2 = n12[t].toString(16);
    s2.length < 2 && (s2 = "0" + s2), e += s2;
  }
  return e;
} };
var Ct = Math.pow(2, 32) - 1;
var ma = [].push;
var Wr = { video: 1, audio: 2, id3: 3, text: 4 };
function se2(n12) {
  return String.fromCharCode.apply(null, n12);
}
function Yr(n12, e) {
  let t = n12[e] << 8 | n12[e + 1];
  return t < 0 ? 65536 + t : t;
}
function O4(n12, e) {
  let t = qr(n12, e);
  return t < 0 ? 4294967296 + t : t;
}
function Yi(n12, e) {
  let t = O4(n12, e);
  return t *= Math.pow(2, 32), t += O4(n12, e + 4), t;
}
function qr(n12, e) {
  return n12[e] << 24 | n12[e + 1] << 16 | n12[e + 2] << 8 | n12[e + 3];
}
function is(n12, e, t) {
  n12[e] = t >> 24, n12[e + 1] = t >> 16 & 255, n12[e + 2] = t >> 8 & 255, n12[e + 3] = t & 255;
}
function pa(n12) {
  let e = n12.byteLength;
  for (let t = 0; t < e; ) {
    let s2 = O4(n12, t);
    if (s2 > 8 && n12[t + 4] === 109 && n12[t + 5] === 111 && n12[t + 6] === 111 && n12[t + 7] === 102)
      return true;
    t = s2 > 1 ? t + s2 : e;
  }
  return false;
}
function H2(n12, e) {
  let t = [];
  if (!e.length)
    return t;
  let s2 = n12.byteLength;
  for (let i = 0; i < s2; ) {
    let r2 = O4(n12, i), a2 = se2(n12.subarray(i + 4, i + 8)), o = r2 > 1 ? i + r2 : s2;
    if (a2 === e[0])
      if (e.length === 1)
        t.push(n12.subarray(i + 8, o));
      else {
        let l2 = H2(n12.subarray(i + 8, o), e.slice(1));
        l2.length && ma.apply(t, l2);
      }
    i = o;
  }
  return t;
}
function Ta(n12) {
  let e = [], t = n12[0], s2 = 8, i = O4(n12, s2);
  s2 += 4;
  let r2 = 0, a2 = 0;
  t === 0 ? (r2 = O4(n12, s2), a2 = O4(n12, s2 + 4), s2 += 8) : (r2 = Yi(n12, s2), a2 = Yi(n12, s2 + 8), s2 += 16), s2 += 2;
  let o = n12.length + a2, l2 = Yr(n12, s2);
  s2 += 2;
  for (let c = 0; c < l2; c++) {
    let h2 = s2, u2 = O4(n12, h2);
    h2 += 4;
    let d3 = u2 & 2147483647;
    if ((u2 & 2147483648) >>> 31 === 1)
      return v2.warn("SIDX has hierarchical references (not supported)"), null;
    let g2 = O4(n12, h2);
    h2 += 4, e.push({ referenceSize: d3, subsegmentDuration: g2, info: { duration: g2 / i, start: o, end: o + d3 - 1 } }), o += d3, h2 += 4, s2 = h2;
  }
  return { earliestPresentationTime: r2, timescale: i, version: t, referencesCount: l2, references: e };
}
function jr(n12) {
  let e = [], t = H2(n12, ["moov", "trak"]);
  for (let i = 0; i < t.length; i++) {
    let r2 = t[i], a2 = H2(r2, ["tkhd"])[0];
    if (a2) {
      let o = a2[0], l2 = O4(a2, o === 0 ? 12 : 20), c = H2(r2, ["mdia", "mdhd"])[0];
      if (c) {
        o = c[0];
        let h2 = O4(c, o === 0 ? 12 : 20), u2 = H2(r2, ["mdia", "hdlr"])[0];
        if (u2) {
          let d3 = se2(u2.subarray(8, 12)), f3 = { soun: z2.AUDIO, vide: z2.VIDEO }[d3];
          if (f3) {
            let g2 = H2(r2, ["mdia", "minf", "stbl", "stsd"])[0], p3 = Ea(g2);
            e[l2] = { timescale: h2, type: f3 }, e[f3] = le3({ timescale: h2, id: l2 }, p3);
          }
        }
      }
    }
  }
  return H2(n12, ["moov", "mvex", "trex"]).forEach((i) => {
    let r2 = O4(i, 4), a2 = e[r2];
    a2 && (a2.default = { duration: O4(i, 12), flags: O4(i, 20) });
  }), e;
}
function Ea(n12) {
  let e = n12.subarray(8), t = e.subarray(86), s2 = se2(e.subarray(4, 8)), i = s2, r2 = s2 === "enca" || s2 === "encv";
  if (r2) {
    let o = H2(e, [s2])[0].subarray(s2 === "enca" ? 28 : 78);
    H2(o, ["sinf"]).forEach((c) => {
      let h2 = H2(c, ["schm"])[0];
      if (h2) {
        let u2 = se2(h2.subarray(4, 8));
        if (u2 === "cbcs" || u2 === "cenc") {
          let d3 = H2(c, ["frma"])[0];
          d3 && (i = se2(d3));
        }
      }
    });
  }
  switch (i) {
    case "avc1":
    case "avc2":
    case "avc3":
    case "avc4": {
      let a2 = H2(t, ["avcC"])[0];
      i += "." + ut(a2[1]) + ut(a2[2]) + ut(a2[3]);
      break;
    }
    case "mp4a": {
      let a2 = H2(e, [s2])[0], o = H2(a2.subarray(28), ["esds"])[0];
      if (o && o.length > 12) {
        let l2 = 4;
        if (o[l2++] !== 3)
          break;
        l2 = rs(o, l2), l2 += 2;
        let c = o[l2++];
        if (c & 128 && (l2 += 2), c & 64 && (l2 += o[l2++]), o[l2++] !== 4)
          break;
        l2 = rs(o, l2);
        let h2 = o[l2++];
        if (h2 === 64)
          i += "." + ut(h2);
        else
          break;
        if (l2 += 12, o[l2++] !== 5)
          break;
        l2 = rs(o, l2);
        let u2 = o[l2++], d3 = (u2 & 248) >> 3;
        d3 === 31 && (d3 += 1 + ((u2 & 7) << 3) + ((o[l2] & 224) >> 5)), i += "." + d3;
      }
      break;
    }
    case "hvc1":
    case "hev1": {
      let a2 = H2(t, ["hvcC"])[0], o = a2[1], l2 = ["", "A", "B", "C"][o >> 6], c = o & 31, h2 = O4(a2, 2), u2 = (o & 32) >> 5 ? "H" : "L", d3 = a2[12], f3 = a2.subarray(6, 12);
      i += "." + l2 + c, i += "." + h2.toString(16).toUpperCase(), i += "." + u2 + d3;
      let g2 = "";
      for (let p3 = f3.length; p3--; ) {
        let T2 = f3[p3];
        (T2 || g2) && (g2 = "." + T2.toString(16).toUpperCase() + g2);
      }
      i += g2;
      break;
    }
    case "dvh1":
    case "dvhe": {
      let a2 = H2(t, ["dvcC"])[0], o = a2[2] >> 1 & 127, l2 = a2[2] << 5 & 32 | a2[3] >> 3 & 31;
      i += "." + Se2(o) + "." + Se2(l2);
      break;
    }
    case "vp09": {
      let a2 = H2(t, ["vpcC"])[0], o = a2[4], l2 = a2[5], c = a2[6] >> 4 & 15;
      i += "." + Se2(o) + "." + Se2(l2) + "." + Se2(c);
      break;
    }
    case "av01": {
      let a2 = H2(t, ["av1C"])[0], o = a2[1] >>> 5, l2 = a2[1] & 31, c = a2[2] >>> 7 ? "H" : "M", h2 = (a2[2] & 64) >> 6, u2 = (a2[2] & 32) >> 5, d3 = o === 2 && h2 ? u2 ? 12 : 10 : h2 ? 10 : 8, f3 = (a2[2] & 16) >> 4, g2 = (a2[2] & 8) >> 3, p3 = (a2[2] & 4) >> 2, T2 = a2[2] & 3;
      i += "." + o + "." + Se2(l2) + c + "." + Se2(d3) + "." + f3 + "." + g2 + p3 + T2 + "." + Se2(1) + "." + Se2(1) + "." + Se2(1) + ".0";
      break;
    }
  }
  return { codec: i, encrypted: r2 };
}
function rs(n12, e) {
  let t = e + 5;
  for (; n12[e++] & 128 && e < t; )
    ;
  return e;
}
function ut(n12) {
  return ("0" + n12.toString(16).toUpperCase()).slice(-2);
}
function Se2(n12) {
  return (n12 < 10 ? "0" : "") + n12;
}
function ya2(n12, e) {
  if (!n12 || !e)
    return n12;
  let t = e.keyId;
  return t && e.isCommonEncryption && H2(n12, ["moov", "trak"]).forEach((i) => {
    let a2 = H2(i, ["mdia", "minf", "stbl", "stsd"])[0].subarray(8), o = H2(a2, ["enca"]), l2 = o.length > 0;
    l2 || (o = H2(a2, ["encv"])), o.forEach((c) => {
      let h2 = l2 ? c.subarray(28) : c.subarray(78);
      H2(h2, ["sinf"]).forEach((d3) => {
        let f3 = zr(d3);
        if (f3) {
          let g2 = f3.subarray(8, 24);
          g2.some((p3) => p3 !== 0) || (v2.log(`[eme] Patching keyId in 'enc${l2 ? "a" : "v"}>sinf>>tenc' box: ${ve3.hexDump(g2)} -> ${ve3.hexDump(t)}`), f3.set(t, 8));
        }
      });
    });
  }), n12;
}
function zr(n12) {
  let e = H2(n12, ["schm"])[0];
  if (e) {
    let t = se2(e.subarray(4, 8));
    if (t === "cbcs" || t === "cenc")
      return H2(n12, ["schi", "tenc"])[0];
  }
  return v2.error("[eme] missing 'schm' box"), null;
}
function xa2(n12, e) {
  return H2(e, ["moof", "traf"]).reduce((t, s2) => {
    let i = H2(s2, ["tfdt"])[0], r2 = i[0], a2 = H2(s2, ["tfhd"]).reduce((o, l2) => {
      let c = O4(l2, 4), h2 = n12[c];
      if (h2) {
        let u2 = O4(i, 4);
        if (r2 === 1) {
          if (u2 === Ct)
            return v2.warn("[mp4-demuxer]: Ignoring assumed invalid signed 64-bit track fragment decode time"), o;
          u2 *= Ct + 1, u2 += O4(i, 8);
        }
        let d3 = h2.timescale || 9e4, f3 = u2 / d3;
        if (F(f3) && (o === null || f3 < o))
          return f3;
      }
      return o;
    }, null);
    return a2 !== null && F(a2) && (t === null || a2 < t) ? a2 : t;
  }, null);
}
function Sa2(n12, e) {
  let t = 0, s2 = 0, i = 0, r2 = H2(n12, ["moof", "traf"]);
  for (let a2 = 0; a2 < r2.length; a2++) {
    let o = r2[a2], l2 = H2(o, ["tfhd"])[0], c = O4(l2, 4), h2 = e[c];
    if (!h2)
      continue;
    let u2 = h2.default, d3 = O4(l2, 0) | u2?.flags, f3 = u2?.duration;
    d3 & 8 && (d3 & 2 ? f3 = O4(l2, 12) : f3 = O4(l2, 8));
    let g2 = h2.timescale || 9e4, p3 = H2(o, ["trun"]);
    for (let T2 = 0; T2 < p3.length; T2++) {
      if (t = va(p3[T2]), !t && f3) {
        let E4 = O4(p3[T2], 4);
        t = f3 * E4;
      }
      h2.type === z2.VIDEO ? s2 += t / g2 : h2.type === z2.AUDIO && (i += t / g2);
    }
  }
  if (s2 === 0 && i === 0) {
    let a2 = 1 / 0, o = 0, l2 = 0, c = H2(n12, ["sidx"]);
    for (let h2 = 0; h2 < c.length; h2++) {
      let u2 = Ta(c[h2]);
      if (u2 != null && u2.references) {
        a2 = Math.min(a2, u2.earliestPresentationTime / u2.timescale);
        let d3 = u2.references.reduce((f3, g2) => f3 + g2.info.duration || 0, 0);
        o = Math.max(o, d3 + u2.earliestPresentationTime / u2.timescale), l2 = o - a2;
      }
    }
    if (l2 && F(l2))
      return l2;
  }
  return s2 || i;
}
function va(n12) {
  let e = O4(n12, 0), t = 8;
  e & 1 && (t += 4), e & 4 && (t += 4);
  let s2 = 0, i = O4(n12, 4);
  for (let r2 = 0; r2 < i; r2++) {
    if (e & 256) {
      let a2 = O4(n12, t);
      s2 += a2, t += 4;
    }
    e & 512 && (t += 4), e & 1024 && (t += 4), e & 2048 && (t += 4);
  }
  return s2;
}
function Aa(n12, e, t) {
  H2(e, ["moof", "traf"]).forEach((s2) => {
    H2(s2, ["tfhd"]).forEach((i) => {
      let r2 = O4(i, 4), a2 = n12[r2];
      if (!a2)
        return;
      let o = a2.timescale || 9e4;
      H2(s2, ["tfdt"]).forEach((l2) => {
        let c = l2[0], h2 = t * o;
        if (h2) {
          let u2 = O4(l2, 4);
          if (c === 0)
            u2 -= h2, u2 = Math.max(u2, 0), is(l2, 4, u2);
          else {
            u2 *= Math.pow(2, 32), u2 += O4(l2, 8), u2 -= h2, u2 = Math.max(u2, 0);
            let d3 = Math.floor(u2 / (Ct + 1)), f3 = Math.floor(u2 % (Ct + 1));
            is(l2, 4, d3), is(l2, 8, f3);
          }
        }
      });
    });
  });
}
function La(n12) {
  let e = { valid: null, remainder: null }, t = H2(n12, ["moof"]);
  if (t.length < 2)
    return e.remainder = n12, e;
  let s2 = t[t.length - 1];
  return e.valid = Ne2(n12, 0, s2.byteOffset - 8), e.remainder = Ne2(n12, s2.byteOffset - 8), e;
}
function pe3(n12, e) {
  let t = new Uint8Array(n12.length + e.length);
  return t.set(n12), t.set(e, n12.length), t;
}
function qi(n12, e) {
  let t = [], s2 = e.samples, i = e.timescale, r2 = e.id, a2 = false;
  return H2(s2, ["moof"]).map((l2) => {
    let c = l2.byteOffset - 8;
    H2(l2, ["traf"]).map((u2) => {
      let d3 = H2(u2, ["tfdt"]).map((f3) => {
        let g2 = f3[0], p3 = O4(f3, 4);
        return g2 === 1 && (p3 *= Math.pow(2, 32), p3 += O4(f3, 8)), p3 / i;
      })[0];
      return d3 !== void 0 && (n12 = d3), H2(u2, ["tfhd"]).map((f3) => {
        let g2 = O4(f3, 4), p3 = O4(f3, 0) & 16777215, T2 = (p3 & 1) !== 0, E4 = (p3 & 2) !== 0, x3 = (p3 & 8) !== 0, y3 = 0, R = (p3 & 16) !== 0, S2 = 0, b = (p3 & 32) !== 0, L = 8;
        g2 === r2 && (T2 && (L += 8), E4 && (L += 4), x3 && (y3 = O4(f3, L), L += 4), R && (S2 = O4(f3, L), L += 4), b && (L += 4), e.type === "video" && (a2 = Ra(e.codec)), H2(u2, ["trun"]).map((C2) => {
          let _ = C2[0], I = O4(C2, 0) & 16777215, w = (I & 1) !== 0, K = 0, P2 = (I & 4) !== 0, G2 = (I & 256) !== 0, $3 = 0, U3 = (I & 512) !== 0, Y3 = 0, X = (I & 1024) !== 0, M2 = (I & 2048) !== 0, k = 0, q = O4(C2, 4), V = 8;
          w && (K = O4(C2, V), V += 4), P2 && (V += 4);
          let j = K + c;
          for (let ee2 = 0; ee2 < q; ee2++) {
            if (G2 ? ($3 = O4(C2, V), V += 4) : $3 = y3, U3 ? (Y3 = O4(C2, V), V += 4) : Y3 = S2, X && (V += 4), M2 && (_ === 0 ? k = O4(C2, V) : k = qr(C2, V), V += 4), e.type === z2.VIDEO) {
              let re2 = 0;
              for (; re2 < Y3; ) {
                let ce2 = O4(s2, j);
                if (j += 4, Ia(a2, s2[j])) {
                  let ge3 = s2.subarray(j, j + ce2);
                  Xr(ge3, a2 ? 2 : 1, n12 + k / i, t);
                }
                j += ce2, re2 += ce2 + 4;
              }
            }
            n12 += $3 / i;
          }
        }));
      });
    });
  }), t;
}
function Ra(n12) {
  if (!n12)
    return false;
  let e = n12.indexOf("."), t = e < 0 ? n12 : n12.substring(0, e);
  return t === "hvc1" || t === "hev1" || t === "dvh1" || t === "dvhe";
}
function Ia(n12, e) {
  if (n12) {
    let t = e >> 1 & 63;
    return t === 39 || t === 40;
  } else
    return (e & 31) === 6;
}
function Xr(n12, e, t, s2) {
  let i = Qr(n12), r2 = 0;
  r2 += e;
  let a2 = 0, o = 0, l2 = 0;
  for (; r2 < i.length; ) {
    a2 = 0;
    do {
      if (r2 >= i.length)
        break;
      l2 = i[r2++], a2 += l2;
    } while (l2 === 255);
    o = 0;
    do {
      if (r2 >= i.length)
        break;
      l2 = i[r2++], o += l2;
    } while (l2 === 255);
    let c = i.length - r2, h2 = r2;
    if (o < c)
      r2 += o;
    else if (o > c) {
      v2.error(`Malformed SEI payload. ${o} is too small, only ${c} bytes left to parse.`);
      break;
    }
    if (a2 === 4) {
      if (i[h2++] === 181) {
        let d3 = Yr(i, h2);
        if (h2 += 2, d3 === 49) {
          let f3 = O4(i, h2);
          if (h2 += 4, f3 === 1195456820) {
            let g2 = i[h2++];
            if (g2 === 3) {
              let p3 = i[h2++], T2 = 31 & p3, E4 = 64 & p3, x3 = E4 ? 2 + T2 * 3 : 0, y3 = new Uint8Array(x3);
              if (E4) {
                y3[0] = p3;
                for (let R = 1; R < x3; R++)
                  y3[R] = i[h2++];
              }
              s2.push({ type: g2, payloadType: a2, pts: t, bytes: y3 });
            }
          }
        }
      }
    } else if (a2 === 5 && o > 16) {
      let u2 = [];
      for (let g2 = 0; g2 < 16; g2++) {
        let p3 = i[h2++].toString(16);
        u2.push(p3.length == 1 ? "0" + p3 : p3), (g2 === 3 || g2 === 5 || g2 === 7 || g2 === 9) && u2.push("-");
      }
      let d3 = o - 16, f3 = new Uint8Array(d3);
      for (let g2 = 0; g2 < d3; g2++)
        f3[g2] = i[h2++];
      s2.push({ payloadType: a2, pts: t, uuid: u2.join(""), userData: Re2(f3), userDataBytes: f3 });
    }
  }
}
function Qr(n12) {
  let e = n12.byteLength, t = [], s2 = 1;
  for (; s2 < e - 2; )
    n12[s2] === 0 && n12[s2 + 1] === 0 && n12[s2 + 2] === 3 ? (t.push(s2 + 2), s2 += 2) : s2++;
  if (t.length === 0)
    return n12;
  let i = e - t.length, r2 = new Uint8Array(i), a2 = 0;
  for (s2 = 0; s2 < i; a2++, s2++)
    a2 === t[0] && (a2++, t.shift()), r2[s2] = n12[a2];
  return r2;
}
function ba(n12) {
  let e = n12[0], t = "", s2 = "", i = 0, r2 = 0, a2 = 0, o = 0, l2 = 0, c = 0;
  if (e === 0) {
    for (; se2(n12.subarray(c, c + 1)) !== "\0"; )
      t += se2(n12.subarray(c, c + 1)), c += 1;
    for (t += se2(n12.subarray(c, c + 1)), c += 1; se2(n12.subarray(c, c + 1)) !== "\0"; )
      s2 += se2(n12.subarray(c, c + 1)), c += 1;
    s2 += se2(n12.subarray(c, c + 1)), c += 1, i = O4(n12, 12), r2 = O4(n12, 16), o = O4(n12, 20), l2 = O4(n12, 24), c = 28;
  } else if (e === 1) {
    c += 4, i = O4(n12, c), c += 4;
    let u2 = O4(n12, c);
    c += 4;
    let d3 = O4(n12, c);
    for (c += 4, a2 = 2 ** 32 * u2 + d3, Yn(a2) || (a2 = Number.MAX_SAFE_INTEGER, v2.warn("Presentation time exceeds safe integer limit and wrapped to max safe integer in parsing emsg box")), o = O4(n12, c), c += 4, l2 = O4(n12, c), c += 4; se2(n12.subarray(c, c + 1)) !== "\0"; )
      t += se2(n12.subarray(c, c + 1)), c += 1;
    for (t += se2(n12.subarray(c, c + 1)), c += 1; se2(n12.subarray(c, c + 1)) !== "\0"; )
      s2 += se2(n12.subarray(c, c + 1)), c += 1;
    s2 += se2(n12.subarray(c, c + 1)), c += 1;
  }
  let h2 = n12.subarray(c, n12.byteLength);
  return { schemeIdUri: t, value: s2, timeScale: i, presentationTime: a2, presentationTimeDelta: r2, eventDuration: o, id: l2, payload: h2 };
}
function Da(n12, ...e) {
  let t = e.length, s2 = 8, i = t;
  for (; i--; )
    s2 += e[i].byteLength;
  let r2 = new Uint8Array(s2);
  for (r2[0] = s2 >> 24 & 255, r2[1] = s2 >> 16 & 255, r2[2] = s2 >> 8 & 255, r2[3] = s2 & 255, r2.set(n12, 4), i = 0, s2 = 8; i < t; i++)
    r2.set(e[i], s2), s2 += e[i].byteLength;
  return r2;
}
function Ca2(n12, e, t) {
  if (n12.byteLength !== 16)
    throw new RangeError("Invalid system id");
  let s2, i;
  if (e) {
    s2 = 1, i = new Uint8Array(e.length * 16);
    for (let o = 0; o < e.length; o++) {
      let l2 = e[o];
      if (l2.byteLength !== 16)
        throw new RangeError("Invalid key");
      i.set(l2, o * 16);
    }
  } else
    s2 = 0, i = new Uint8Array();
  let r2;
  s2 > 0 ? (r2 = new Uint8Array(4), e.length > 0 && new DataView(r2.buffer).setUint32(0, e.length, false)) : r2 = new Uint8Array();
  let a2 = new Uint8Array(4);
  return t && t.byteLength > 0 && new DataView(a2.buffer).setUint32(0, t.byteLength, false), Da([112, 115, 115, 104], new Uint8Array([s2, 0, 0, 0]), n12, r2, i, a2, t || new Uint8Array());
}
function wa(n12) {
  if (!(n12 instanceof ArrayBuffer) || n12.byteLength < 32)
    return null;
  let e = { version: 0, systemId: "", kids: null, data: null }, t = new DataView(n12), s2 = t.getUint32(0);
  if (n12.byteLength !== s2 && s2 > 44 || t.getUint32(4) !== 1886614376 || (e.version = t.getUint32(8) >>> 24, e.version > 1))
    return null;
  e.systemId = ve3.hexDump(new Uint8Array(n12, 12, 16));
  let r2 = t.getUint32(28);
  if (e.version === 0) {
    if (s2 - 32 < r2)
      return null;
    e.data = new Uint8Array(n12, 32, r2);
  } else if (e.version === 1) {
    e.kids = [];
    for (let a2 = 0; a2 < r2; a2++)
      e.kids.push(new Uint8Array(n12, 32 + a2 * 16, 16));
  }
  return e;
}
var dt = {};
var it = class n3 {
  static clearKeyUriToKeyIdMap() {
    dt = {};
  }
  constructor(e, t, s2, i = [1], r2 = null) {
    this.uri = void 0, this.method = void 0, this.keyFormat = void 0, this.keyFormatVersions = void 0, this.encrypted = void 0, this.isCommonEncryption = void 0, this.iv = null, this.key = null, this.keyId = null, this.pssh = null, this.method = e, this.uri = t, this.keyFormat = s2, this.keyFormatVersions = i, this.iv = r2, this.encrypted = e ? e !== "NONE" : false, this.isCommonEncryption = this.encrypted && e !== "AES-128";
  }
  isSupported() {
    if (this.method) {
      if (this.method === "AES-128" || this.method === "NONE")
        return true;
      if (this.keyFormat === "identity")
        return this.method === "SAMPLE-AES";
      switch (this.keyFormat) {
        case fe2.FAIRPLAY:
        case fe2.WIDEVINE:
        case fe2.PLAYREADY:
        case fe2.CLEARKEY:
          return ["ISO-23001-7", "SAMPLE-AES", "SAMPLE-AES-CENC", "SAMPLE-AES-CTR"].indexOf(this.method) !== -1;
      }
    }
    return false;
  }
  getDecryptData(e) {
    if (!this.encrypted || !this.uri)
      return null;
    if (this.method === "AES-128" && this.uri && !this.iv) {
      typeof e != "number" && (this.method === "AES-128" && !this.iv && v2.warn(`missing IV for initialization segment with method="${this.method}" - compliance issue`), e = 0);
      let s2 = _a2(e);
      return new n3(this.method, this.uri, "identity", this.keyFormatVersions, s2);
    }
    let t = ia(this.uri);
    if (t)
      switch (this.keyFormat) {
        case fe2.WIDEVINE:
          this.pssh = t, t.length >= 22 && (this.keyId = t.subarray(t.length - 22, t.length - 6));
          break;
        case fe2.PLAYREADY: {
          let s2 = new Uint8Array([154, 4, 240, 121, 152, 64, 66, 134, 171, 146, 230, 91, 224, 136, 95, 149]);
          this.pssh = Ca2(s2, null, t);
          let i = new Uint16Array(t.buffer, t.byteOffset, t.byteLength / 2), r2 = String.fromCharCode.apply(null, Array.from(i)), a2 = r2.substring(r2.indexOf("<"), r2.length), c = new DOMParser().parseFromString(a2, "text/xml").getElementsByTagName("KID")[0];
          if (c) {
            let h2 = c.childNodes[0] ? c.childNodes[0].nodeValue : c.getAttribute("VALUE");
            if (h2) {
              let u2 = _i(h2).subarray(0, 16);
              sa(u2), this.keyId = u2;
            }
          }
          break;
        }
        default: {
          let s2 = t.subarray(0, 16);
          if (s2.length !== 16) {
            let i = new Uint8Array(16);
            i.set(s2, 16 - s2.length), s2 = i;
          }
          this.keyId = s2;
          break;
        }
      }
    if (!this.keyId || this.keyId.byteLength !== 16) {
      let s2 = dt[this.uri];
      if (!s2) {
        let i = Object.keys(dt).length % Number.MAX_SAFE_INTEGER;
        s2 = new Uint8Array(16), new DataView(s2.buffer, 12, 4).setUint32(0, i), dt[this.uri] = s2;
      }
      this.keyId = s2;
    }
    return this;
  }
};
function _a2(n12) {
  let e = new Uint8Array(16);
  for (let t = 12; t < 16; t++)
    e[t] = n12 >> 8 * (15 - t) & 255;
  return e;
}
var Jr = /\{\$([a-zA-Z0-9-_]+)\}/g;
function ji(n12) {
  return Jr.test(n12);
}
function ue(n12, e, t) {
  if (n12.variableList !== null || n12.hasVariableRefs)
    for (let s2 = t.length; s2--; ) {
      let i = t[s2], r2 = e[i];
      r2 && (e[i] = Ls(n12, r2));
    }
}
function Ls(n12, e) {
  if (n12.variableList !== null || n12.hasVariableRefs) {
    let t = n12.variableList;
    return e.replace(Jr, (s2) => {
      let i = s2.substring(2, s2.length - 1), r2 = t?.[i];
      return r2 === void 0 ? (n12.playlistParsingError || (n12.playlistParsingError = new Error(`Missing preceding EXT-X-DEFINE tag for Variable Reference: "${i}"`)), s2) : r2;
    });
  }
  return e;
}
function zi(n12, e, t) {
  let s2 = n12.variableList;
  s2 || (n12.variableList = s2 = {});
  let i, r2;
  if ("QUERYPARAM" in e) {
    i = e.QUERYPARAM;
    try {
      let a2 = new self.URL(t).searchParams;
      if (a2.has(i))
        r2 = a2.get(i);
      else
        throw new Error(`"${i}" does not match any query parameter in URI: "${t}"`);
    } catch (a2) {
      n12.playlistParsingError || (n12.playlistParsingError = new Error(`EXT-X-DEFINE QUERYPARAM: ${a2.message}`));
    }
  } else
    i = e.NAME, r2 = e.VALUE;
  i in s2 ? n12.playlistParsingError || (n12.playlistParsingError = new Error(`EXT-X-DEFINE duplicate Variable Name declarations: "${i}"`)) : s2[i] = r2 || "";
}
function Pa2(n12, e, t) {
  let s2 = e.IMPORT;
  if (t && s2 in t) {
    let i = n12.variableList;
    i || (n12.variableList = i = {}), i[s2] = t[s2];
  } else
    n12.playlistParsingError || (n12.playlistParsingError = new Error(`EXT-X-DEFINE IMPORT attribute not found in Multivariant Playlist: "${s2}"`));
}
function Ue2(n12 = true) {
  return typeof self > "u" ? void 0 : (n12 || !self.MediaSource) && self.ManagedMediaSource || self.MediaSource || self.WebKitMediaSource;
}
function ka2(n12) {
  return typeof self < "u" && n12 === self.ManagedMediaSource;
}
var wt = { audio: { a3ds: 1, "ac-3": 0.95, "ac-4": 1, alac: 0.9, alaw: 1, dra1: 1, "dts+": 1, "dts-": 1, dtsc: 1, dtse: 1, dtsh: 1, "ec-3": 0.9, enca: 1, fLaC: 0.9, flac: 0.9, FLAC: 0.9, g719: 1, g726: 1, m4ae: 1, mha1: 1, mha2: 1, mhm1: 1, mhm2: 1, mlpa: 1, mp4a: 1, "raw ": 1, Opus: 1, opus: 1, samr: 1, sawb: 1, sawp: 1, sevc: 1, sqcp: 1, ssmv: 1, twos: 1, ulaw: 1 }, video: { avc1: 1, avc2: 1, avc3: 1, avc4: 1, avcp: 1, av01: 0.8, drac: 1, dva1: 1, dvav: 1, dvh1: 0.7, dvhe: 0.7, encv: 1, hev1: 0.75, hvc1: 0.75, mjp2: 1, mp4v: 1, mvc1: 1, mvc2: 1, mvc3: 1, mvc4: 1, resv: 1, rv60: 1, s263: 1, svc1: 1, svc2: 1, "vc-1": 1, vp08: 1, vp09: 0.9 }, text: { stpp: 1, wvtt: 1 } };
function Fa(n12, e) {
  let t = wt[e];
  return !!t && !!t[n12.slice(0, 4)];
}
function ns(n12, e, t = true) {
  return !n12.split(",").some((s2) => !Zr(s2, e, t));
}
function Zr(n12, e, t = true) {
  var s2;
  let i = Ue2(t);
  return (s2 = i?.isTypeSupported(rt(n12, e))) != null ? s2 : false;
}
function rt(n12, e) {
  return `${e}/mp4;codecs="${n12}"`;
}
function Xi(n12) {
  if (n12) {
    let e = n12.substring(0, 4);
    return wt.video[e];
  }
  return 2;
}
function _t(n12) {
  return n12.split(",").reduce((e, t) => {
    let s2 = wt.video[t];
    return s2 ? (s2 * 2 + e) / (e ? 3 : 2) : (wt.audio[t] + e) / (e ? 2 : 1);
  }, 0);
}
var as = {};
function Ma(n12, e = true) {
  if (as[n12])
    return as[n12];
  let t = { flac: ["flac", "fLaC", "FLAC"], opus: ["opus", "Opus"] }[n12];
  for (let s2 = 0; s2 < t.length; s2++)
    if (Zr(t[s2], "audio", e))
      return as[n12] = t[s2], t[s2];
  return n12;
}
var Oa = /flac|opus/i;
function Pt(n12, e = true) {
  return n12.replace(Oa, (t) => Ma(t.toLowerCase(), e));
}
function Qi(n12, e) {
  return n12 && n12 !== "mp4a" ? n12 : e && e.split(",")[0];
}
function Na2(n12) {
  let e = n12.split(".");
  if (e.length > 2) {
    let t = e.shift() + ".";
    return t += parseInt(e.shift()).toString(16), t += ("000" + parseInt(e.shift()).toString(16)).slice(-4), t;
  }
  return n12;
}
var Ji = /#EXT-X-STREAM-INF:([^\r\n]*)(?:[\r\n](?:#[^\r\n]*)?)*([^\r\n]+)|#EXT-X-(SESSION-DATA|SESSION-KEY|DEFINE|CONTENT-STEERING|START):([^\r\n]*)[\r\n]+/g;
var Zi = /#EXT-X-MEDIA:(.*)/g;
var Ua = /^#EXT(?:INF|-X-TARGETDURATION):/m;
var er = new RegExp([/#EXTINF:\s*(\d*(?:\.\d+)?)(?:,(.*)\s+)?/.source, /(?!#) *(\S[\S ]*)/.source, /#EXT-X-BYTERANGE:*(.+)/.source, /#EXT-X-PROGRAM-DATE-TIME:(.+)/.source, /#.*/.source].join("|"), "g");
var Ba = new RegExp([/#(EXTM3U)/.source, /#EXT-X-(DATERANGE|DEFINE|KEY|MAP|PART|PART-INF|PLAYLIST-TYPE|PRELOAD-HINT|RENDITION-REPORT|SERVER-CONTROL|SKIP|START):(.+)/.source, /#EXT-X-(BITRATE|DISCONTINUITY-SEQUENCE|MEDIA-SEQUENCE|TARGETDURATION|VERSION): *(\d+)/.source, /#EXT-X-(DISCONTINUITY|ENDLIST|GAP|INDEPENDENT-SEGMENTS)/.source, /(#)([^:]*):(.*)/.source, /(#)(.*)(?:.*)\r?\n?/.source].join("|"));
var Me2 = class n4 {
  static findGroup(e, t) {
    for (let s2 = 0; s2 < e.length; s2++) {
      let i = e[s2];
      if (i.id === t)
        return i;
    }
  }
  static resolve(e, t) {
    return wi.buildAbsoluteURL(t, e, { alwaysNormalize: true });
  }
  static isMediaPlaylist(e) {
    return Ua.test(e);
  }
  static parseMasterPlaylist(e, t) {
    let s2 = ji(e), i = { contentSteering: null, levels: [], playlistParsingError: null, sessionData: null, sessionKeys: null, startTimeOffset: null, variableList: null, hasVariableRefs: s2 }, r2 = [];
    Ji.lastIndex = 0;
    let a2;
    for (; (a2 = Ji.exec(e)) != null; )
      if (a2[1]) {
        var o;
        let c = new Z2(a2[1]);
        ue(i, c, ["CODECS", "SUPPLEMENTAL-CODECS", "ALLOWED-CPC", "PATHWAY-ID", "STABLE-VARIANT-ID", "AUDIO", "VIDEO", "SUBTITLES", "CLOSED-CAPTIONS", "NAME"]);
        let h2 = Ls(i, a2[2]), u2 = { attrs: c, bitrate: c.decimalInteger("BANDWIDTH") || c.decimalInteger("AVERAGE-BANDWIDTH"), name: c.NAME, url: n4.resolve(h2, t) }, d3 = c.decimalResolution("RESOLUTION");
        d3 && (u2.width = d3.width, u2.height = d3.height), $a(c.CODECS, u2), (o = u2.unknownCodecs) != null && o.length || r2.push(u2), i.levels.push(u2);
      } else if (a2[3]) {
        let c = a2[3], h2 = a2[4];
        switch (c) {
          case "SESSION-DATA": {
            let u2 = new Z2(h2);
            ue(i, u2, ["DATA-ID", "LANGUAGE", "VALUE", "URI"]);
            let d3 = u2["DATA-ID"];
            d3 && (i.sessionData === null && (i.sessionData = {}), i.sessionData[d3] = u2);
            break;
          }
          case "SESSION-KEY": {
            let u2 = tr(h2, t, i);
            u2.encrypted && u2.isSupported() ? (i.sessionKeys === null && (i.sessionKeys = []), i.sessionKeys.push(u2)) : v2.warn(`[Keys] Ignoring invalid EXT-X-SESSION-KEY tag: "${h2}"`);
            break;
          }
          case "DEFINE": {
            {
              let u2 = new Z2(h2);
              ue(i, u2, ["NAME", "VALUE", "QUERYPARAM"]), zi(i, u2, t);
            }
            break;
          }
          case "CONTENT-STEERING": {
            let u2 = new Z2(h2);
            ue(i, u2, ["SERVER-URI", "PATHWAY-ID"]), i.contentSteering = { uri: n4.resolve(u2["SERVER-URI"], t), pathwayId: u2["PATHWAY-ID"] || "." };
            break;
          }
          case "START": {
            i.startTimeOffset = sr(h2);
            break;
          }
        }
      }
    let l2 = r2.length > 0 && r2.length < i.levels.length;
    return i.levels = l2 ? r2 : i.levels, i.levels.length === 0 && (i.playlistParsingError = new Error("no levels found in manifest")), i;
  }
  static parseMasterPlaylistMedia(e, t, s2) {
    let i, r2 = {}, a2 = s2.levels, o = { AUDIO: a2.map((c) => ({ id: c.attrs.AUDIO, audioCodec: c.audioCodec })), SUBTITLES: a2.map((c) => ({ id: c.attrs.SUBTITLES, textCodec: c.textCodec })), "CLOSED-CAPTIONS": [] }, l2 = 0;
    for (Zi.lastIndex = 0; (i = Zi.exec(e)) !== null; ) {
      let c = new Z2(i[1]), h2 = c.TYPE;
      if (h2) {
        let u2 = o[h2], d3 = r2[h2] || [];
        r2[h2] = d3, ue(s2, c, ["URI", "GROUP-ID", "LANGUAGE", "ASSOC-LANGUAGE", "STABLE-RENDITION-ID", "NAME", "INSTREAM-ID", "CHARACTERISTICS", "CHANNELS"]);
        let f3 = c.LANGUAGE, g2 = c["ASSOC-LANGUAGE"], p3 = c.CHANNELS, T2 = c.CHARACTERISTICS, E4 = c["INSTREAM-ID"], x3 = { attrs: c, bitrate: 0, id: l2++, groupId: c["GROUP-ID"] || "", name: c.NAME || f3 || "", type: h2, default: c.bool("DEFAULT"), autoselect: c.bool("AUTOSELECT"), forced: c.bool("FORCED"), lang: f3, url: c.URI ? n4.resolve(c.URI, t) : "" };
        if (g2 && (x3.assocLang = g2), p3 && (x3.channels = p3), T2 && (x3.characteristics = T2), E4 && (x3.instreamId = E4), u2 != null && u2.length) {
          let y3 = n4.findGroup(u2, x3.groupId) || u2[0];
          ir(x3, y3, "audioCodec"), ir(x3, y3, "textCodec");
        }
        d3.push(x3);
      }
    }
    return r2;
  }
  static parseLevelPlaylist(e, t, s2, i, r2, a2) {
    let o = new vs(t), l2 = o.fragments, c = null, h2 = 0, u2 = 0, d3 = 0, f3 = 0, g2 = null, p3 = new et(i, t), T2, E4, x3, y3 = -1, R = false, S2 = null;
    for (er.lastIndex = 0, o.m3u8 = e, o.hasVariableRefs = ji(e); (T2 = er.exec(e)) !== null; ) {
      R && (R = false, p3 = new et(i, t), p3.start = d3, p3.sn = h2, p3.cc = f3, p3.level = s2, c && (p3.initSegment = c, p3.rawProgramDateTime = c.rawProgramDateTime, c.rawProgramDateTime = null, S2 && (p3.setByteRange(S2), S2 = null)));
      let _ = T2[1];
      if (_) {
        p3.duration = parseFloat(_);
        let I = (" " + T2[2]).slice(1);
        p3.title = I || null, p3.tagList.push(I ? ["INF", _, I] : ["INF", _]);
      } else if (T2[3]) {
        if (F(p3.duration)) {
          p3.start = d3, x3 && ar(p3, x3, o), p3.sn = h2, p3.level = s2, p3.cc = f3, l2.push(p3);
          let I = (" " + T2[3]).slice(1);
          p3.relurl = Ls(o, I), rr(p3, g2), g2 = p3, d3 += p3.duration, h2++, u2 = 0, R = true;
        }
      } else if (T2[4]) {
        let I = (" " + T2[4]).slice(1);
        g2 ? p3.setByteRange(I, g2) : p3.setByteRange(I);
      } else if (T2[5])
        p3.rawProgramDateTime = (" " + T2[5]).slice(1), p3.tagList.push(["PROGRAM-DATE-TIME", p3.rawProgramDateTime]), y3 === -1 && (y3 = l2.length);
      else {
        if (T2 = T2[0].match(Ba), !T2) {
          v2.warn("No matches on slow regex match for level playlist!");
          continue;
        }
        for (E4 = 1; E4 < T2.length && !(typeof T2[E4] < "u"); E4++)
          ;
        let I = (" " + T2[E4]).slice(1), w = (" " + T2[E4 + 1]).slice(1), K = T2[E4 + 2] ? (" " + T2[E4 + 2]).slice(1) : "";
        switch (I) {
          case "PLAYLIST-TYPE":
            o.type = w.toUpperCase();
            break;
          case "MEDIA-SEQUENCE":
            h2 = o.startSN = parseInt(w);
            break;
          case "SKIP": {
            let P2 = new Z2(w);
            ue(o, P2, ["RECENTLY-REMOVED-DATERANGES"]);
            let G2 = P2.decimalInteger("SKIPPED-SEGMENTS");
            if (F(G2)) {
              o.skippedSegments = G2;
              for (let U3 = G2; U3--; )
                l2.unshift(null);
              h2 += G2;
            }
            let $3 = P2.enumeratedString("RECENTLY-REMOVED-DATERANGES");
            $3 && (o.recentlyRemovedDateranges = $3.split("	"));
            break;
          }
          case "TARGETDURATION":
            o.targetduration = Math.max(parseInt(w), 1);
            break;
          case "VERSION":
            o.version = parseInt(w);
            break;
          case "INDEPENDENT-SEGMENTS":
          case "EXTM3U":
            break;
          case "ENDLIST":
            o.live = false;
            break;
          case "#":
            (w || K) && p3.tagList.push(K ? [w, K] : [w]);
            break;
          case "DISCONTINUITY":
            f3++, p3.tagList.push(["DIS"]);
            break;
          case "GAP":
            p3.gap = true, p3.tagList.push([I]);
            break;
          case "BITRATE":
            p3.tagList.push([I, w]);
            break;
          case "DATERANGE": {
            let P2 = new Z2(w);
            ue(o, P2, ["ID", "CLASS", "START-DATE", "END-DATE", "SCTE35-CMD", "SCTE35-OUT", "SCTE35-IN"]), ue(o, P2, P2.clientAttrs);
            let G2 = new bt(P2, o.dateRanges[P2.ID]);
            G2.isValid || o.skippedSegments ? o.dateRanges[G2.id] = G2 : v2.warn(`Ignoring invalid DATERANGE tag: "${w}"`), p3.tagList.push(["EXT-X-DATERANGE", w]);
            break;
          }
          case "DEFINE": {
            {
              let P2 = new Z2(w);
              ue(o, P2, ["NAME", "VALUE", "IMPORT", "QUERYPARAM"]), "IMPORT" in P2 ? Pa2(o, P2, a2) : zi(o, P2, t);
            }
            break;
          }
          case "DISCONTINUITY-SEQUENCE":
            f3 = parseInt(w);
            break;
          case "KEY": {
            let P2 = tr(w, t, o);
            if (P2.isSupported()) {
              if (P2.method === "NONE") {
                x3 = void 0;
                break;
              }
              x3 || (x3 = {}), x3[P2.keyFormat] && (x3 = te2({}, x3)), x3[P2.keyFormat] = P2;
            } else
              v2.warn(`[Keys] Ignoring invalid EXT-X-KEY tag: "${w}"`);
            break;
          }
          case "START":
            o.startTimeOffset = sr(w);
            break;
          case "MAP": {
            let P2 = new Z2(w);
            if (ue(o, P2, ["BYTERANGE", "URI"]), p3.duration) {
              let G2 = new et(i, t);
              nr(G2, P2, s2, x3), c = G2, p3.initSegment = c, c.rawProgramDateTime && !p3.rawProgramDateTime && (p3.rawProgramDateTime = c.rawProgramDateTime);
            } else {
              let G2 = p3.byteRangeEndOffset;
              if (G2) {
                let $3 = p3.byteRangeStartOffset;
                S2 = `${G2 - $3}@${$3}`;
              } else
                S2 = null;
              nr(p3, P2, s2, x3), c = p3, R = true;
            }
            break;
          }
          case "SERVER-CONTROL": {
            let P2 = new Z2(w);
            o.canBlockReload = P2.bool("CAN-BLOCK-RELOAD"), o.canSkipUntil = P2.optionalFloat("CAN-SKIP-UNTIL", 0), o.canSkipDateRanges = o.canSkipUntil > 0 && P2.bool("CAN-SKIP-DATERANGES"), o.partHoldBack = P2.optionalFloat("PART-HOLD-BACK", 0), o.holdBack = P2.optionalFloat("HOLD-BACK", 0);
            break;
          }
          case "PART-INF": {
            let P2 = new Z2(w);
            o.partTarget = P2.decimalFloatingPoint("PART-TARGET");
            break;
          }
          case "PART": {
            let P2 = o.partList;
            P2 || (P2 = o.partList = []);
            let G2 = u2 > 0 ? P2[P2.length - 1] : void 0, $3 = u2++, U3 = new Z2(w);
            ue(o, U3, ["BYTERANGE", "URI"]);
            let Y3 = new Ss(U3, p3, t, $3, G2);
            P2.push(Y3), p3.duration += Y3.duration;
            break;
          }
          case "PRELOAD-HINT": {
            let P2 = new Z2(w);
            ue(o, P2, ["URI"]), o.preloadHint = P2;
            break;
          }
          case "RENDITION-REPORT": {
            let P2 = new Z2(w);
            ue(o, P2, ["URI"]), o.renditionReports = o.renditionReports || [], o.renditionReports.push(P2);
            break;
          }
          default:
            v2.warn(`line parsed but not handled: ${T2}`);
            break;
        }
      }
    }
    g2 && !g2.relurl ? (l2.pop(), d3 -= g2.duration, o.partList && (o.fragmentHint = g2)) : o.partList && (rr(p3, g2), p3.cc = f3, o.fragmentHint = p3, x3 && ar(p3, x3, o));
    let b = l2.length, L = l2[0], C2 = l2[b - 1];
    if (d3 += o.skippedSegments * o.targetduration, d3 > 0 && b && C2) {
      o.averagetargetduration = d3 / b;
      let _ = C2.sn;
      o.endSN = _ !== "initSegment" ? _ : 0, o.live || (C2.endList = true), L && (o.startCC = L.cc);
    } else
      o.endSN = 0, o.startCC = 0;
    return o.fragmentHint && (d3 += o.fragmentHint.duration), o.totalduration = d3, o.endCC = f3, y3 > 0 && Ga(l2, y3), o;
  }
};
function tr(n12, e, t) {
  var s2, i;
  let r2 = new Z2(n12);
  ue(t, r2, ["KEYFORMAT", "KEYFORMATVERSIONS", "URI", "IV", "URI"]);
  let a2 = (s2 = r2.METHOD) != null ? s2 : "", o = r2.URI, l2 = r2.hexadecimalInteger("IV"), c = r2.KEYFORMATVERSIONS, h2 = (i = r2.KEYFORMAT) != null ? i : "identity";
  o && r2.IV && !l2 && v2.error(`Invalid IV: ${r2.IV}`);
  let u2 = o ? Me2.resolve(o, e) : "", d3 = (c || "1").split("/").map(Number).filter(Number.isFinite);
  return new it(a2, u2, h2, d3, l2);
}
function sr(n12) {
  let t = new Z2(n12).decimalFloatingPoint("TIME-OFFSET");
  return F(t) ? t : null;
}
function $a(n12, e) {
  let t = (n12 || "").split(/[ ,]+/).filter((s2) => s2);
  ["video", "audio", "text"].forEach((s2) => {
    let i = t.filter((r2) => Fa(r2, s2));
    i.length && (e[`${s2}Codec`] = i.join(","), t = t.filter((r2) => i.indexOf(r2) === -1));
  }), e.unknownCodecs = t;
}
function ir(n12, e, t) {
  let s2 = e[t];
  s2 && (n12[t] = s2);
}
function Ga(n12, e) {
  let t = n12[e];
  for (let s2 = e; s2--; ) {
    let i = n12[s2];
    if (!i)
      return;
    i.programDateTime = t.programDateTime - i.duration * 1e3, t = i;
  }
}
function rr(n12, e) {
  n12.rawProgramDateTime ? n12.programDateTime = Date.parse(n12.rawProgramDateTime) : e != null && e.programDateTime && (n12.programDateTime = e.endProgramDateTime), F(n12.programDateTime) || (n12.programDateTime = null, n12.rawProgramDateTime = null);
}
function nr(n12, e, t, s2) {
  n12.relurl = e.URI, e.BYTERANGE && n12.setByteRange(e.BYTERANGE), n12.level = t, n12.sn = "initSegment", s2 && (n12.levelkeys = s2), n12.initSegment = null;
}
function ar(n12, e, t) {
  n12.levelkeys = e;
  let { encryptedFragments: s2 } = t;
  (!s2.length || s2[s2.length - 1].levelkeys !== e) && Object.keys(e).some((i) => e[i].isCommonEncryption) && s2.push(n12);
}
var W3 = { MANIFEST: "manifest", LEVEL: "level", AUDIO_TRACK: "audioTrack", SUBTITLE_TRACK: "subtitleTrack" };
var N2 = { MAIN: "main", AUDIO: "audio", SUBTITLE: "subtitle" };
function or(n12) {
  let { type: e } = n12;
  switch (e) {
    case W3.AUDIO_TRACK:
      return N2.AUDIO;
    case W3.SUBTITLE_TRACK:
      return N2.SUBTITLE;
    default:
      return N2.MAIN;
  }
}
function os(n12, e) {
  let t = n12.url;
  return (t === void 0 || t.indexOf("data:") === 0) && (t = e.url), t;
}
var Rs = class {
  constructor(e) {
    this.hls = void 0, this.loaders = /* @__PURE__ */ Object.create(null), this.variableList = null, this.hls = e, this.registerListeners();
  }
  startLoad(e) {
  }
  stopLoad() {
    this.destroyInternalLoaders();
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.LEVEL_LOADING, this.onLevelLoading, this), e.on(m2.AUDIO_TRACK_LOADING, this.onAudioTrackLoading, this), e.on(m2.SUBTITLE_TRACK_LOADING, this.onSubtitleTrackLoading, this);
  }
  unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.LEVEL_LOADING, this.onLevelLoading, this), e.off(m2.AUDIO_TRACK_LOADING, this.onAudioTrackLoading, this), e.off(m2.SUBTITLE_TRACK_LOADING, this.onSubtitleTrackLoading, this);
  }
  createInternalLoader(e) {
    let t = this.hls.config, s2 = t.pLoader, i = t.loader, r2 = s2 || i, a2 = new r2(t);
    return this.loaders[e.type] = a2, a2;
  }
  getInternalLoader(e) {
    return this.loaders[e.type];
  }
  resetInternalLoader(e) {
    this.loaders[e] && delete this.loaders[e];
  }
  destroyInternalLoaders() {
    for (let e in this.loaders) {
      let t = this.loaders[e];
      t && t.destroy(), this.resetInternalLoader(e);
    }
  }
  destroy() {
    this.variableList = null, this.unregisterListeners(), this.destroyInternalLoaders();
  }
  onManifestLoading(e, t) {
    let { url: s2 } = t;
    this.variableList = null, this.load({ id: null, level: 0, responseType: "text", type: W3.MANIFEST, url: s2, deliveryDirectives: null });
  }
  onLevelLoading(e, t) {
    let { id: s2, level: i, pathwayId: r2, url: a2, deliveryDirectives: o } = t;
    this.load({ id: s2, level: i, pathwayId: r2, responseType: "text", type: W3.LEVEL, url: a2, deliveryDirectives: o });
  }
  onAudioTrackLoading(e, t) {
    let { id: s2, groupId: i, url: r2, deliveryDirectives: a2 } = t;
    this.load({ id: s2, groupId: i, level: null, responseType: "text", type: W3.AUDIO_TRACK, url: r2, deliveryDirectives: a2 });
  }
  onSubtitleTrackLoading(e, t) {
    let { id: s2, groupId: i, url: r2, deliveryDirectives: a2 } = t;
    this.load({ id: s2, groupId: i, level: null, responseType: "text", type: W3.SUBTITLE_TRACK, url: r2, deliveryDirectives: a2 });
  }
  load(e) {
    var t;
    let s2 = this.hls.config, i = this.getInternalLoader(e);
    if (i) {
      let c = i.context;
      if (c && c.url === e.url && c.level === e.level) {
        v2.trace("[playlist-loader]: playlist request ongoing");
        return;
      }
      v2.log(`[playlist-loader]: aborting previous loader for type: ${e.type}`), i.abort();
    }
    let r2;
    if (e.type === W3.MANIFEST ? r2 = s2.manifestLoadPolicy.default : r2 = te2({}, s2.playlistLoadPolicy.default, { timeoutRetry: null, errorRetry: null }), i = this.createInternalLoader(e), F((t = e.deliveryDirectives) == null ? void 0 : t.part)) {
      let c;
      if (e.type === W3.LEVEL && e.level !== null ? c = this.hls.levels[e.level].details : e.type === W3.AUDIO_TRACK && e.id !== null ? c = this.hls.audioTracks[e.id].details : e.type === W3.SUBTITLE_TRACK && e.id !== null && (c = this.hls.subtitleTracks[e.id].details), c) {
        let h2 = c.partTarget, u2 = c.targetduration;
        if (h2 && u2) {
          let d3 = Math.max(h2 * 3, u2 * 0.8) * 1e3;
          r2 = te2({}, r2, { maxTimeToFirstByteMs: Math.min(d3, r2.maxTimeToFirstByteMs), maxLoadTimeMs: Math.min(d3, r2.maxTimeToFirstByteMs) });
        }
      }
    }
    let a2 = r2.errorRetry || r2.timeoutRetry || {}, o = { loadPolicy: r2, timeout: r2.maxLoadTimeMs, maxRetry: a2.maxNumRetry || 0, retryDelay: a2.retryDelayMs || 0, maxRetryDelay: a2.maxRetryDelayMs || 0 }, l2 = { onSuccess: (c, h2, u2, d3) => {
      let f3 = this.getInternalLoader(u2);
      this.resetInternalLoader(u2.type);
      let g2 = c.data;
      if (g2.indexOf("#EXTM3U") !== 0) {
        this.handleManifestParsingError(c, u2, new Error("no EXTM3U delimiter"), d3 || null, h2);
        return;
      }
      h2.parsing.start = performance.now(), Me2.isMediaPlaylist(g2) ? this.handleTrackOrLevelPlaylist(c, h2, u2, d3 || null, f3) : this.handleMasterPlaylist(c, h2, u2, d3);
    }, onError: (c, h2, u2, d3) => {
      this.handleNetworkError(h2, u2, false, c, d3);
    }, onTimeout: (c, h2, u2) => {
      this.handleNetworkError(h2, u2, true, void 0, c);
    } };
    i.load(e, o, l2);
  }
  handleMasterPlaylist(e, t, s2, i) {
    let r2 = this.hls, a2 = e.data, o = os(e, s2), l2 = Me2.parseMasterPlaylist(a2, o);
    if (l2.playlistParsingError) {
      this.handleManifestParsingError(e, s2, l2.playlistParsingError, i, t);
      return;
    }
    let { contentSteering: c, levels: h2, sessionData: u2, sessionKeys: d3, startTimeOffset: f3, variableList: g2 } = l2;
    this.variableList = g2;
    let { AUDIO: p3 = [], SUBTITLES: T2, "CLOSED-CAPTIONS": E4 } = Me2.parseMasterPlaylistMedia(a2, o, l2);
    p3.length && !p3.some((y3) => !y3.url) && h2[0].audioCodec && !h2[0].attrs.AUDIO && (v2.log("[playlist-loader]: audio codec signaled in quality level, but no embedded audio track signaled, create one"), p3.unshift({ type: "main", name: "main", groupId: "main", default: false, autoselect: false, forced: false, id: -1, attrs: new Z2({}), bitrate: 0, url: "" })), r2.trigger(m2.MANIFEST_LOADED, { levels: h2, audioTracks: p3, subtitles: T2, captions: E4, contentSteering: c, url: o, stats: t, networkDetails: i, sessionData: u2, sessionKeys: d3, startTimeOffset: f3, variableList: g2 });
  }
  handleTrackOrLevelPlaylist(e, t, s2, i, r2) {
    let a2 = this.hls, { id: o, level: l2, type: c } = s2, h2 = os(e, s2), u2 = 0, d3 = F(l2) ? l2 : F(o) ? o : 0, f3 = or(s2), g2 = Me2.parseLevelPlaylist(e.data, h2, d3, f3, u2, this.variableList);
    if (c === W3.MANIFEST) {
      let p3 = { attrs: new Z2({}), bitrate: 0, details: g2, name: "", url: h2 };
      a2.trigger(m2.MANIFEST_LOADED, { levels: [p3], audioTracks: [], url: h2, stats: t, networkDetails: i, sessionData: null, sessionKeys: null, contentSteering: null, startTimeOffset: null, variableList: null });
    }
    t.parsing.end = performance.now(), s2.levelDetails = g2, this.handlePlaylistLoaded(g2, e, t, s2, i, r2);
  }
  handleManifestParsingError(e, t, s2, i, r2) {
    this.hls.trigger(m2.ERROR, { type: B2.NETWORK_ERROR, details: A2.MANIFEST_PARSING_ERROR, fatal: t.type === W3.MANIFEST, url: e.url, err: s2, error: s2, reason: s2.message, response: e, context: t, networkDetails: i, stats: r2 });
  }
  handleNetworkError(e, t, s2 = false, i, r2) {
    let a2 = `A network ${s2 ? "timeout" : "error" + (i ? " (status " + i.code + ")" : "")} occurred while loading ${e.type}`;
    e.type === W3.LEVEL ? a2 += `: ${e.level} id: ${e.id}` : (e.type === W3.AUDIO_TRACK || e.type === W3.SUBTITLE_TRACK) && (a2 += ` id: ${e.id} group-id: "${e.groupId}"`);
    let o = new Error(a2);
    v2.warn(`[playlist-loader]: ${a2}`);
    let l2 = A2.UNKNOWN, c = false, h2 = this.getInternalLoader(e);
    switch (e.type) {
      case W3.MANIFEST:
        l2 = s2 ? A2.MANIFEST_LOAD_TIMEOUT : A2.MANIFEST_LOAD_ERROR, c = true;
        break;
      case W3.LEVEL:
        l2 = s2 ? A2.LEVEL_LOAD_TIMEOUT : A2.LEVEL_LOAD_ERROR, c = false;
        break;
      case W3.AUDIO_TRACK:
        l2 = s2 ? A2.AUDIO_TRACK_LOAD_TIMEOUT : A2.AUDIO_TRACK_LOAD_ERROR, c = false;
        break;
      case W3.SUBTITLE_TRACK:
        l2 = s2 ? A2.SUBTITLE_TRACK_LOAD_TIMEOUT : A2.SUBTITLE_LOAD_ERROR, c = false;
        break;
    }
    h2 && this.resetInternalLoader(e.type);
    let u2 = { type: B2.NETWORK_ERROR, details: l2, fatal: c, url: e.url, loader: h2, context: e, error: o, networkDetails: t, stats: r2 };
    if (i) {
      let d3 = t?.url || e.url;
      u2.response = le3({ url: d3, data: void 0 }, i);
    }
    this.hls.trigger(m2.ERROR, u2);
  }
  handlePlaylistLoaded(e, t, s2, i, r2, a2) {
    let o = this.hls, { type: l2, level: c, id: h2, groupId: u2, deliveryDirectives: d3 } = i, f3 = os(t, i), g2 = or(i), p3 = typeof i.level == "number" && g2 === N2.MAIN ? c : void 0;
    if (!e.fragments.length) {
      let E4 = new Error("No Segments found in Playlist");
      o.trigger(m2.ERROR, { type: B2.NETWORK_ERROR, details: A2.LEVEL_EMPTY_ERROR, fatal: false, url: f3, error: E4, reason: E4.message, response: t, context: i, level: p3, parent: g2, networkDetails: r2, stats: s2 });
      return;
    }
    e.targetduration || (e.playlistParsingError = new Error("Missing Target Duration"));
    let T2 = e.playlistParsingError;
    if (T2) {
      o.trigger(m2.ERROR, { type: B2.NETWORK_ERROR, details: A2.LEVEL_PARSING_ERROR, fatal: false, url: f3, error: T2, reason: T2.message, response: t, context: i, level: p3, parent: g2, networkDetails: r2, stats: s2 });
      return;
    }
    switch (e.live && a2 && (a2.getCacheAge && (e.ageHeader = a2.getCacheAge() || 0), (!a2.getCacheAge || isNaN(e.ageHeader)) && (e.ageHeader = 0)), l2) {
      case W3.MANIFEST:
      case W3.LEVEL:
        o.trigger(m2.LEVEL_LOADED, { details: e, level: p3 || 0, id: h2 || 0, stats: s2, networkDetails: r2, deliveryDirectives: d3 });
        break;
      case W3.AUDIO_TRACK:
        o.trigger(m2.AUDIO_TRACK_LOADED, { details: e, id: h2 || 0, groupId: u2 || "", stats: s2, networkDetails: r2, deliveryDirectives: d3 });
        break;
      case W3.SUBTITLE_TRACK:
        o.trigger(m2.SUBTITLE_TRACK_LOADED, { details: e, id: h2 || 0, groupId: u2 || "", stats: s2, networkDetails: r2, deliveryDirectives: d3 });
        break;
    }
  }
};
function en(n12, e) {
  let t;
  try {
    t = new Event("addtrack");
  } catch {
    t = document.createEvent("Event"), t.initEvent("addtrack", false, false);
  }
  t.track = n12, e.dispatchEvent(t);
}
function tn(n12, e) {
  let t = n12.mode;
  if (t === "disabled" && (n12.mode = "hidden"), n12.cues && !n12.cues.getCueById(e.id))
    try {
      if (n12.addCue(e), !n12.cues.getCueById(e.id))
        throw new Error(`addCue is failed for: ${e}`);
    } catch (s2) {
      v2.debug(`[texttrack-utils]: ${s2}`);
      try {
        let i = new self.TextTrackCue(e.startTime, e.endTime, e.text);
        i.id = e.id, n12.addCue(i);
      } catch (i) {
        v2.debug(`[texttrack-utils]: Legacy TextTrackCue fallback failed: ${i}`);
      }
    }
  t === "disabled" && (n12.mode = t);
}
function Ve2(n12) {
  let e = n12.mode;
  if (e === "disabled" && (n12.mode = "hidden"), n12.cues)
    for (let t = n12.cues.length; t--; )
      n12.removeCue(n12.cues[t]);
  e === "disabled" && (n12.mode = e);
}
function Is(n12, e, t, s2) {
  let i = n12.mode;
  if (i === "disabled" && (n12.mode = "hidden"), n12.cues && n12.cues.length > 0) {
    let r2 = Ha(n12.cues, e, t);
    for (let a2 = 0; a2 < r2.length; a2++)
      (!s2 || s2(r2[a2])) && n12.removeCue(r2[a2]);
  }
  i === "disabled" && (n12.mode = i);
}
function Ka(n12, e) {
  if (e < n12[0].startTime)
    return 0;
  let t = n12.length - 1;
  if (e > n12[t].endTime)
    return -1;
  let s2 = 0, i = t;
  for (; s2 <= i; ) {
    let r2 = Math.floor((i + s2) / 2);
    if (e < n12[r2].startTime)
      i = r2 - 1;
    else if (e > n12[r2].startTime && s2 < t)
      s2 = r2 + 1;
    else
      return r2;
  }
  return n12[s2].startTime - e < e - n12[i].startTime ? s2 : i;
}
function Ha(n12, e, t) {
  let s2 = [], i = Ka(n12, e);
  if (i > -1)
    for (let r2 = i, a2 = n12.length; r2 < a2; r2++) {
      let o = n12[r2];
      if (o.startTime >= e && o.endTime <= t)
        s2.push(o);
      else if (o.startTime > t)
        return s2;
    }
  return s2;
}
function xt(n12) {
  let e = [];
  for (let t = 0; t < n12.length; t++) {
    let s2 = n12[t];
    (s2.kind === "subtitles" || s2.kind === "captions") && s2.label && e.push(n12[t]);
  }
  return e;
}
var xe3 = { audioId3: "org.id3", dateRange: "com.apple.quicktime.HLS", emsg: "https://aomedia.org/emsg/ID3" };
var Va = 0.25;
function bs() {
  if (!(typeof self > "u"))
    return self.VTTCue || self.TextTrackCue;
}
function lr(n12, e, t, s2, i) {
  let r2 = new n12(e, t, "");
  try {
    r2.value = s2, i && (r2.type = i);
  } catch {
    r2 = new n12(e, t, JSON.stringify(i ? le3({ type: i }, s2) : s2));
  }
  return r2;
}
var ft = (() => {
  let n12 = bs();
  try {
    n12 && new n12(0, Number.POSITIVE_INFINITY, "");
  } catch {
    return Number.MAX_VALUE;
  }
  return Number.POSITIVE_INFINITY;
})();
function ls(n12, e) {
  return n12.getTime() / 1e3 - e;
}
function Wa(n12) {
  return Uint8Array.from(n12.replace(/^0x/, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" ")).buffer;
}
var Ds = class {
  constructor(e) {
    this.hls = void 0, this.id3Track = null, this.media = null, this.dateRangeCuesAppended = {}, this.hls = e, this._registerListeners();
  }
  destroy() {
    this._unregisterListeners(), this.id3Track = null, this.media = null, this.dateRangeCuesAppended = {}, this.hls = null;
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.FRAG_PARSING_METADATA, this.onFragParsingMetadata, this), e.on(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.on(m2.LEVEL_UPDATED, this.onLevelUpdated, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.FRAG_PARSING_METADATA, this.onFragParsingMetadata, this), e.off(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.off(m2.LEVEL_UPDATED, this.onLevelUpdated, this);
  }
  onMediaAttached(e, t) {
    this.media = t.media;
  }
  onMediaDetaching() {
    this.id3Track && (Ve2(this.id3Track), this.id3Track = null, this.media = null, this.dateRangeCuesAppended = {});
  }
  onManifestLoading() {
    this.dateRangeCuesAppended = {};
  }
  createTrack(e) {
    let t = this.getID3Track(e.textTracks);
    return t.mode = "hidden", t;
  }
  getID3Track(e) {
    if (this.media) {
      for (let t = 0; t < e.length; t++) {
        let s2 = e[t];
        if (s2.kind === "metadata" && s2.label === "id3")
          return en(s2, this.media), s2;
      }
      return this.media.addTextTrack("metadata", "id3");
    }
  }
  onFragParsingMetadata(e, t) {
    if (!this.media)
      return;
    let { hls: { config: { enableEmsgMetadataCues: s2, enableID3MetadataCues: i } } } = this;
    if (!s2 && !i)
      return;
    let { samples: r2 } = t;
    this.id3Track || (this.id3Track = this.createTrack(this.media));
    let a2 = bs();
    if (a2)
      for (let o = 0; o < r2.length; o++) {
        let l2 = r2[o].type;
        if (l2 === xe3.emsg && !s2 || !i)
          continue;
        let c = Vr(r2[o].data);
        if (c) {
          let h2 = r2[o].pts, u2 = h2 + r2[o].duration;
          u2 > ft && (u2 = ft), u2 - h2 <= 0 && (u2 = h2 + Va);
          for (let f3 = 0; f3 < c.length; f3++) {
            let g2 = c[f3];
            if (!Hr(g2)) {
              this.updateId3CueEnds(h2, l2);
              let p3 = lr(a2, h2, u2, g2, l2);
              p3 && this.id3Track.addCue(p3);
            }
          }
        }
      }
  }
  updateId3CueEnds(e, t) {
    var s2;
    let i = (s2 = this.id3Track) == null ? void 0 : s2.cues;
    if (i)
      for (let r2 = i.length; r2--; ) {
        let a2 = i[r2];
        a2.type === t && a2.startTime < e && a2.endTime === ft && (a2.endTime = e);
      }
  }
  onBufferFlushing(e, { startOffset: t, endOffset: s2, type: i }) {
    let { id3Track: r2, hls: a2 } = this;
    if (!a2)
      return;
    let { config: { enableEmsgMetadataCues: o, enableID3MetadataCues: l2 } } = a2;
    if (r2 && (o || l2)) {
      let c;
      i === "audio" ? c = (h2) => h2.type === xe3.audioId3 && l2 : i === "video" ? c = (h2) => h2.type === xe3.emsg && o : c = (h2) => h2.type === xe3.audioId3 && l2 || h2.type === xe3.emsg && o, Is(r2, t, s2, c);
    }
  }
  onLevelUpdated(e, { details: t }) {
    if (!this.media || !t.hasProgramDateTime || !this.hls.config.enableDateRangeMetadataCues)
      return;
    let { dateRangeCuesAppended: s2, id3Track: i } = this, { dateRanges: r2 } = t, a2 = Object.keys(r2);
    if (i) {
      let h2 = Object.keys(s2).filter((u2) => !a2.includes(u2));
      for (let u2 = h2.length; u2--; ) {
        let d3 = h2[u2];
        Object.keys(s2[d3].cues).forEach((f3) => {
          i.removeCue(s2[d3].cues[f3]);
        }), delete s2[d3];
      }
    }
    let o = t.fragments[t.fragments.length - 1];
    if (a2.length === 0 || !F(o?.programDateTime))
      return;
    this.id3Track || (this.id3Track = this.createTrack(this.media));
    let l2 = o.programDateTime / 1e3 - o.start, c = bs();
    for (let h2 = 0; h2 < a2.length; h2++) {
      let u2 = a2[h2], d3 = r2[u2], f3 = ls(d3.startDate, l2), g2 = s2[u2], p3 = g2?.cues || {}, T2 = g2?.durationKnown || false, E4 = ft, x3 = d3.endDate;
      if (x3)
        E4 = ls(x3, l2), T2 = true;
      else if (d3.endOnNext && !T2) {
        let R = a2.reduce((S2, b) => {
          if (b !== d3.id) {
            let L = r2[b];
            if (L.class === d3.class && L.startDate > d3.startDate && (!S2 || d3.startDate < S2.startDate))
              return L;
          }
          return S2;
        }, null);
        R && (E4 = ls(R.startDate, l2), T2 = true);
      }
      let y3 = Object.keys(d3.attr);
      for (let R = 0; R < y3.length; R++) {
        let S2 = y3[R];
        if (!Jn(S2))
          continue;
        let b = p3[S2];
        if (b)
          T2 && !g2.durationKnown && (b.endTime = E4);
        else if (c) {
          let L = d3.attr[S2];
          Zn(S2) && (L = Wa(L));
          let C2 = lr(c, f3, E4, { key: S2, data: L }, xe3.dateRange);
          C2 && (C2.id = u2, this.id3Track.addCue(C2), p3[S2] = C2);
        }
      }
      s2[u2] = { cues: p3, dateRange: d3, durationKnown: T2 };
    }
  }
};
var Cs = class {
  constructor(e) {
    this.hls = void 0, this.config = void 0, this.media = null, this.levelDetails = null, this.currentTime = 0, this.stallCount = 0, this._latency = null, this.timeupdateHandler = () => this.timeupdate(), this.hls = e, this.config = e.config, this.registerListeners();
  }
  get latency() {
    return this._latency || 0;
  }
  get maxLatency() {
    let { config: e, levelDetails: t } = this;
    return e.liveMaxLatencyDuration !== void 0 ? e.liveMaxLatencyDuration : t ? e.liveMaxLatencyDurationCount * t.targetduration : 0;
  }
  get targetLatency() {
    let { levelDetails: e } = this;
    if (e === null)
      return null;
    let { holdBack: t, partHoldBack: s2, targetduration: i } = e, { liveSyncDuration: r2, liveSyncDurationCount: a2, lowLatencyMode: o } = this.config, l2 = this.hls.userConfig, c = o && s2 || t;
    (l2.liveSyncDuration || l2.liveSyncDurationCount || c === 0) && (c = r2 !== void 0 ? r2 : a2 * i);
    let h2 = i;
    return c + Math.min(this.stallCount * 1, h2);
  }
  get liveSyncPosition() {
    let e = this.estimateLiveEdge(), t = this.targetLatency, s2 = this.levelDetails;
    if (e === null || t === null || s2 === null)
      return null;
    let i = s2.edge, r2 = e - t - this.edgeStalled, a2 = i - s2.totalduration, o = i - (this.config.lowLatencyMode && s2.partTarget || s2.targetduration);
    return Math.min(Math.max(a2, r2), o);
  }
  get drift() {
    let { levelDetails: e } = this;
    return e === null ? 1 : e.drift;
  }
  get edgeStalled() {
    let { levelDetails: e } = this;
    if (e === null)
      return 0;
    let t = (this.config.lowLatencyMode && e.partTarget || e.targetduration) * 3;
    return Math.max(e.age - t, 0);
  }
  get forwardBufferLength() {
    let { media: e, levelDetails: t } = this;
    if (!e || !t)
      return 0;
    let s2 = e.buffered.length;
    return (s2 ? e.buffered.end(s2 - 1) : t.edge) - this.currentTime;
  }
  destroy() {
    this.unregisterListeners(), this.onMediaDetaching(), this.levelDetails = null, this.hls = this.timeupdateHandler = null;
  }
  registerListeners() {
    this.hls.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), this.hls.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), this.hls.on(m2.LEVEL_UPDATED, this.onLevelUpdated, this), this.hls.on(m2.ERROR, this.onError, this);
  }
  unregisterListeners() {
    this.hls.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), this.hls.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), this.hls.off(m2.LEVEL_UPDATED, this.onLevelUpdated, this), this.hls.off(m2.ERROR, this.onError, this);
  }
  onMediaAttached(e, t) {
    this.media = t.media, this.media.addEventListener("timeupdate", this.timeupdateHandler);
  }
  onMediaDetaching() {
    this.media && (this.media.removeEventListener("timeupdate", this.timeupdateHandler), this.media = null);
  }
  onManifestLoading() {
    this.levelDetails = null, this._latency = null, this.stallCount = 0;
  }
  onLevelUpdated(e, { details: t }) {
    this.levelDetails = t, t.advanced && this.timeupdate(), !t.live && this.media && this.media.removeEventListener("timeupdate", this.timeupdateHandler);
  }
  onError(e, t) {
    var s2;
    t.details === A2.BUFFER_STALLED_ERROR && (this.stallCount++, (s2 = this.levelDetails) != null && s2.live && v2.warn("[playback-rate-controller]: Stall detected, adjusting target latency"));
  }
  timeupdate() {
    let { media: e, levelDetails: t } = this;
    if (!e || !t)
      return;
    this.currentTime = e.currentTime;
    let s2 = this.computeLatency();
    if (s2 === null)
      return;
    this._latency = s2;
    let { lowLatencyMode: i, maxLiveSyncPlaybackRate: r2 } = this.config;
    if (!i || r2 === 1 || !t.live)
      return;
    let a2 = this.targetLatency;
    if (a2 === null)
      return;
    let o = s2 - a2, l2 = Math.min(this.maxLatency, a2 + t.targetduration);
    if (o < l2 && o > 0.05 && this.forwardBufferLength > 1) {
      let h2 = Math.min(2, Math.max(1, r2)), u2 = Math.round(2 / (1 + Math.exp(-0.75 * o - this.edgeStalled)) * 20) / 20;
      e.playbackRate = Math.min(h2, Math.max(1, u2));
    } else
      e.playbackRate !== 1 && e.playbackRate !== 0 && (e.playbackRate = 1);
  }
  estimateLiveEdge() {
    let { levelDetails: e } = this;
    return e === null ? null : e.edge + e.age;
  }
  computeLatency() {
    let e = this.estimateLiveEdge();
    return e === null ? null : e - this.currentTime;
  }
};
var ws = ["NONE", "TYPE-0", "TYPE-1", null];
function Ya(n12) {
  return ws.indexOf(n12) > -1;
}
var kt = ["SDR", "PQ", "HLG"];
function qa(n12) {
  return !!n12 && kt.indexOf(n12) > -1;
}
var St = { No: "", Yes: "YES", v2: "v2" };
function cr(n12) {
  let { canSkipUntil: e, canSkipDateRanges: t, age: s2 } = n12, i = s2 < e / 2;
  return e && i ? t ? St.v2 : St.Yes : St.No;
}
var Ft = class {
  constructor(e, t, s2) {
    this.msn = void 0, this.part = void 0, this.skip = void 0, this.msn = e, this.part = t, this.skip = s2;
  }
  addDirectives(e) {
    let t = new self.URL(e);
    return this.msn !== void 0 && t.searchParams.set("_HLS_msn", this.msn.toString()), this.part !== void 0 && t.searchParams.set("_HLS_part", this.part.toString()), this.skip && t.searchParams.set("_HLS_skip", this.skip), t.href;
  }
};
var Pe3 = class {
  constructor(e) {
    this._attrs = void 0, this.audioCodec = void 0, this.bitrate = void 0, this.codecSet = void 0, this.url = void 0, this.frameRate = void 0, this.height = void 0, this.id = void 0, this.name = void 0, this.videoCodec = void 0, this.width = void 0, this.details = void 0, this.fragmentError = 0, this.loadError = 0, this.loaded = void 0, this.realBitrate = 0, this.supportedPromise = void 0, this.supportedResult = void 0, this._avgBitrate = 0, this._audioGroups = void 0, this._subtitleGroups = void 0, this._urlId = 0, this.url = [e.url], this._attrs = [e.attrs], this.bitrate = e.bitrate, e.details && (this.details = e.details), this.id = e.id || 0, this.name = e.name, this.width = e.width || 0, this.height = e.height || 0, this.frameRate = e.attrs.optionalFloat("FRAME-RATE", 0), this._avgBitrate = e.attrs.decimalInteger("AVERAGE-BANDWIDTH"), this.audioCodec = e.audioCodec, this.videoCodec = e.videoCodec, this.codecSet = [e.videoCodec, e.audioCodec].filter((t) => !!t).map((t) => t.substring(0, 4)).join(","), this.addGroupId("audio", e.attrs.AUDIO), this.addGroupId("text", e.attrs.SUBTITLES);
  }
  get maxBitrate() {
    return Math.max(this.realBitrate, this.bitrate);
  }
  get averageBitrate() {
    return this._avgBitrate || this.realBitrate || this.bitrate;
  }
  get attrs() {
    return this._attrs[0];
  }
  get codecs() {
    return this.attrs.CODECS || "";
  }
  get pathwayId() {
    return this.attrs["PATHWAY-ID"] || ".";
  }
  get videoRange() {
    return this.attrs["VIDEO-RANGE"] || "SDR";
  }
  get score() {
    return this.attrs.optionalFloat("SCORE", 0);
  }
  get uri() {
    return this.url[0] || "";
  }
  hasAudioGroup(e) {
    return hr(this._audioGroups, e);
  }
  hasSubtitleGroup(e) {
    return hr(this._subtitleGroups, e);
  }
  get audioGroups() {
    return this._audioGroups;
  }
  get subtitleGroups() {
    return this._subtitleGroups;
  }
  addGroupId(e, t) {
    if (t) {
      if (e === "audio") {
        let s2 = this._audioGroups;
        s2 || (s2 = this._audioGroups = []), s2.indexOf(t) === -1 && s2.push(t);
      } else if (e === "text") {
        let s2 = this._subtitleGroups;
        s2 || (s2 = this._subtitleGroups = []), s2.indexOf(t) === -1 && s2.push(t);
      }
    }
  }
  get urlId() {
    return 0;
  }
  set urlId(e) {
  }
  get audioGroupIds() {
    return this.audioGroups ? [this.audioGroupId] : void 0;
  }
  get textGroupIds() {
    return this.subtitleGroups ? [this.textGroupId] : void 0;
  }
  get audioGroupId() {
    var e;
    return (e = this.audioGroups) == null ? void 0 : e[0];
  }
  get textGroupId() {
    var e;
    return (e = this.subtitleGroups) == null ? void 0 : e[0];
  }
  addFallback() {
  }
};
function hr(n12, e) {
  return !e || !n12 ? false : n12.indexOf(e) !== -1;
}
function cs(n12, e) {
  let t = e.startPTS;
  if (F(t)) {
    let s2 = 0, i;
    e.sn > n12.sn ? (s2 = t - n12.start, i = n12) : (s2 = n12.start - t, i = e), i.duration !== s2 && (i.duration = s2);
  } else
    e.sn > n12.sn ? n12.cc === e.cc && n12.minEndPTS ? e.start = n12.start + (n12.minEndPTS - n12.start) : e.start = n12.start + n12.duration : e.start = Math.max(n12.start - e.duration, 0);
}
function sn2(n12, e, t, s2, i, r2) {
  s2 - t <= 0 && (v2.warn("Fragment should have a positive duration", e), s2 = t + e.duration, r2 = i + e.duration);
  let o = t, l2 = s2, c = e.startPTS, h2 = e.endPTS;
  if (F(c)) {
    let T2 = Math.abs(c - t);
    F(e.deltaPTS) ? e.deltaPTS = Math.max(T2, e.deltaPTS) : e.deltaPTS = T2, o = Math.max(t, c), t = Math.min(t, c), i = Math.min(i, e.startDTS), l2 = Math.min(s2, h2), s2 = Math.max(s2, h2), r2 = Math.max(r2, e.endDTS);
  }
  let u2 = t - e.start;
  e.start !== 0 && (e.start = t), e.duration = s2 - e.start, e.startPTS = t, e.maxStartPTS = o, e.startDTS = i, e.endPTS = s2, e.minEndPTS = l2, e.endDTS = r2;
  let d3 = e.sn;
  if (!n12 || d3 < n12.startSN || d3 > n12.endSN)
    return 0;
  let f3, g2 = d3 - n12.startSN, p3 = n12.fragments;
  for (p3[g2] = e, f3 = g2; f3 > 0; f3--)
    cs(p3[f3], p3[f3 - 1]);
  for (f3 = g2; f3 < p3.length - 1; f3++)
    cs(p3[f3], p3[f3 + 1]);
  return n12.fragmentHint && cs(p3[p3.length - 1], n12.fragmentHint), n12.PTSKnown = n12.alignedSliding = true, u2;
}
function ja(n12, e) {
  let t = null, s2 = n12.fragments;
  for (let l2 = s2.length - 1; l2 >= 0; l2--) {
    let c = s2[l2].initSegment;
    if (c) {
      t = c;
      break;
    }
  }
  n12.fragmentHint && delete n12.fragmentHint.endPTS;
  let i = 0, r2;
  if (Qa(n12, e, (l2, c) => {
    l2.relurl && (i = l2.cc - c.cc), F(l2.startPTS) && F(l2.endPTS) && (c.start = c.startPTS = l2.startPTS, c.startDTS = l2.startDTS, c.maxStartPTS = l2.maxStartPTS, c.endPTS = l2.endPTS, c.endDTS = l2.endDTS, c.minEndPTS = l2.minEndPTS, c.duration = l2.endPTS - l2.startPTS, c.duration && (r2 = c), e.PTSKnown = e.alignedSliding = true), c.elementaryStreams = l2.elementaryStreams, c.loader = l2.loader, c.stats = l2.stats, l2.initSegment && (c.initSegment = l2.initSegment, t = l2.initSegment);
  }), t && (e.fragmentHint ? e.fragments.concat(e.fragmentHint) : e.fragments).forEach((c) => {
    var h2;
    c && (!c.initSegment || c.initSegment.relurl === ((h2 = t) == null ? void 0 : h2.relurl)) && (c.initSegment = t);
  }), e.skippedSegments)
    if (e.deltaUpdateFailed = e.fragments.some((l2) => !l2), e.deltaUpdateFailed) {
      v2.warn("[level-helper] Previous playlist missing segments skipped in delta playlist");
      for (let l2 = e.skippedSegments; l2--; )
        e.fragments.shift();
      e.startSN = e.fragments[0].sn, e.startCC = e.fragments[0].cc;
    } else
      e.canSkipDateRanges && (e.dateRanges = za2(n12.dateRanges, e.dateRanges, e.recentlyRemovedDateranges));
  let a2 = e.fragments;
  if (i) {
    v2.warn("discontinuity sliding from playlist, take drift into account");
    for (let l2 = 0; l2 < a2.length; l2++)
      a2[l2].cc += i;
  }
  e.skippedSegments && (e.startCC = e.fragments[0].cc), Xa(n12.partList, e.partList, (l2, c) => {
    c.elementaryStreams = l2.elementaryStreams, c.stats = l2.stats;
  }), r2 ? sn2(e, r2, r2.startPTS, r2.endPTS, r2.startDTS, r2.endDTS) : rn(n12, e), a2.length && (e.totalduration = e.edge - a2[0].start), e.driftStartTime = n12.driftStartTime, e.driftStart = n12.driftStart;
  let o = e.advancedDateTime;
  if (e.advanced && o) {
    let l2 = e.edge;
    e.driftStart || (e.driftStartTime = o, e.driftStart = l2), e.driftEndTime = o, e.driftEnd = l2;
  } else
    e.driftEndTime = n12.driftEndTime, e.driftEnd = n12.driftEnd, e.advancedDateTime = n12.advancedDateTime;
}
function za2(n12, e, t) {
  let s2 = te2({}, n12);
  return t && t.forEach((i) => {
    delete s2[i];
  }), Object.keys(e).forEach((i) => {
    let r2 = new bt(e[i].attr, s2[i]);
    r2.isValid ? s2[i] = r2 : v2.warn(`Ignoring invalid Playlist Delta Update DATERANGE tag: "${JSON.stringify(e[i].attr)}"`);
  }), s2;
}
function Xa(n12, e, t) {
  if (n12 && e) {
    let s2 = 0;
    for (let i = 0, r2 = n12.length; i <= r2; i++) {
      let a2 = n12[i], o = e[i + s2];
      a2 && o && a2.index === o.index && a2.fragment.sn === o.fragment.sn ? t(a2, o) : s2--;
    }
  }
}
function Qa(n12, e, t) {
  let s2 = e.skippedSegments, i = Math.max(n12.startSN, e.startSN) - e.startSN, r2 = (n12.fragmentHint ? 1 : 0) + (s2 ? e.endSN : Math.min(n12.endSN, e.endSN)) - e.startSN, a2 = e.startSN - n12.startSN, o = e.fragmentHint ? e.fragments.concat(e.fragmentHint) : e.fragments, l2 = n12.fragmentHint ? n12.fragments.concat(n12.fragmentHint) : n12.fragments;
  for (let c = i; c <= r2; c++) {
    let h2 = l2[a2 + c], u2 = o[c];
    s2 && !u2 && c < s2 && (u2 = e.fragments[c] = h2), h2 && u2 && t(h2, u2);
  }
}
function rn(n12, e) {
  let t = e.startSN + e.skippedSegments - n12.startSN, s2 = n12.fragments;
  t < 0 || t >= s2.length || _s(e, s2[t].start);
}
function _s(n12, e) {
  if (e) {
    let t = n12.fragments;
    for (let s2 = n12.skippedSegments; s2 < t.length; s2++)
      t[s2].start += e;
    n12.fragmentHint && (n12.fragmentHint.start += e);
  }
}
function Ja(n12, e = 1 / 0) {
  let t = 1e3 * n12.targetduration;
  if (n12.updated) {
    let s2 = n12.fragments;
    if (s2.length && t * 4 > e) {
      let r2 = s2[s2.length - 1].duration * 1e3;
      r2 < t && (t = r2);
    }
  } else
    t /= 2;
  return Math.round(t);
}
function Za(n12, e, t) {
  if (!(n12 != null && n12.details))
    return null;
  let s2 = n12.details, i = s2.fragments[e - s2.startSN];
  return i || (i = s2.fragmentHint, i && i.sn === e) ? i : e < s2.startSN && t && t.sn === e ? t : null;
}
function ur(n12, e, t) {
  var s2;
  return n12 != null && n12.details ? nn((s2 = n12.details) == null ? void 0 : s2.partList, e, t) : null;
}
function nn(n12, e, t) {
  if (n12)
    for (let s2 = n12.length; s2--; ) {
      let i = n12[s2];
      if (i.index === t && i.fragment.sn === e)
        return i;
    }
  return null;
}
function an2(n12) {
  n12.forEach((e, t) => {
    let { details: s2 } = e;
    s2 != null && s2.fragments && s2.fragments.forEach((i) => {
      i.level = t;
    });
  });
}
function Mt(n12) {
  switch (n12.details) {
    case A2.FRAG_LOAD_TIMEOUT:
    case A2.KEY_LOAD_TIMEOUT:
    case A2.LEVEL_LOAD_TIMEOUT:
    case A2.MANIFEST_LOAD_TIMEOUT:
      return true;
  }
  return false;
}
function dr(n12, e) {
  let t = Mt(e);
  return n12.default[`${t ? "timeout" : "error"}Retry`];
}
function Fi(n12, e) {
  let t = n12.backoff === "linear" ? 1 : Math.pow(2, e);
  return Math.min(t * n12.retryDelayMs, n12.maxRetryDelayMs);
}
function fr(n12) {
  return le3(le3({}, n12), { errorRetry: null, timeoutRetry: null });
}
function Ot(n12, e, t, s2) {
  if (!n12)
    return false;
  let i = s2?.code, r2 = e < n12.maxNumRetry && (eo(i) || !!t);
  return n12.shouldRetry ? n12.shouldRetry(n12, e, t, s2, r2) : r2;
}
function eo(n12) {
  return n12 === 0 && navigator.onLine === false || !!n12 && (n12 < 400 || n12 > 499);
}
var on = { search: function(n12, e) {
  let t = 0, s2 = n12.length - 1, i = null, r2 = null;
  for (; t <= s2; ) {
    i = (t + s2) / 2 | 0, r2 = n12[i];
    let a2 = e(r2);
    if (a2 > 0)
      t = i + 1;
    else if (a2 < 0)
      s2 = i - 1;
    else
      return r2;
  }
  return null;
} };
function to(n12, e, t) {
  if (e === null || !Array.isArray(n12) || !n12.length || !F(e))
    return null;
  let s2 = n12[0].programDateTime;
  if (e < (s2 || 0))
    return null;
  let i = n12[n12.length - 1].endProgramDateTime;
  if (e >= (i || 0))
    return null;
  t = t || 0;
  for (let r2 = 0; r2 < n12.length; ++r2) {
    let a2 = n12[r2];
    if (so(e, t, a2))
      return a2;
  }
  return null;
}
function Nt(n12, e, t = 0, s2 = 0) {
  let i = null;
  if (n12) {
    i = e[n12.sn - e[0].sn + 1] || null;
    let a2 = n12.endDTS - t;
    a2 > 0 && a2 < 15e-7 && (t += 15e-7);
  } else
    t === 0 && e[0].start === 0 && (i = e[0]);
  if (i && (!n12 || n12.level === i.level) && Ps(t, s2, i) === 0)
    return i;
  let r2 = on.search(e, Ps.bind(null, t, s2));
  return r2 && (r2 !== n12 || !i) ? r2 : i;
}
function Ps(n12 = 0, e = 0, t) {
  if (t.start <= n12 && t.start + t.duration > n12)
    return 0;
  let s2 = Math.min(e, t.duration + (t.deltaPTS ? t.deltaPTS : 0));
  return t.start + t.duration - s2 <= n12 ? 1 : t.start - s2 > n12 && t.start ? -1 : 0;
}
function so(n12, e, t) {
  let s2 = Math.min(e, t.duration + (t.deltaPTS ? t.deltaPTS : 0)) * 1e3;
  return (t.endProgramDateTime || 0) - s2 > n12;
}
function io(n12, e) {
  return on.search(n12, (t) => t.cc < e ? 1 : t.cc > e ? -1 : 0);
}
var ae3 = { DoNothing: 0, SendEndCallback: 1, SendAlternateToPenaltyBox: 2, RemoveAlternatePermanently: 3, InsertDiscontinuity: 4, RetryRequest: 5 };
var Te2 = { None: 0, MoveAllAlternatesMatchingHost: 1, MoveAllAlternatesMatchingHDCP: 2, SwitchToSDR: 4 };
var ks = class {
  constructor(e) {
    this.hls = void 0, this.playlistError = 0, this.penalizedRenditions = {}, this.log = void 0, this.warn = void 0, this.error = void 0, this.hls = e, this.log = v2.log.bind(v2, "[info]:"), this.warn = v2.warn.bind(v2, "[warning]:"), this.error = v2.error.bind(v2, "[error]:"), this.registerListeners();
  }
  registerListeners() {
    let e = this.hls;
    e.on(m2.ERROR, this.onError, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.LEVEL_UPDATED, this.onLevelUpdated, this);
  }
  unregisterListeners() {
    let e = this.hls;
    e && (e.off(m2.ERROR, this.onError, this), e.off(m2.ERROR, this.onErrorOut, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.LEVEL_UPDATED, this.onLevelUpdated, this));
  }
  destroy() {
    this.unregisterListeners(), this.hls = null, this.penalizedRenditions = {};
  }
  startLoad(e) {
  }
  stopLoad() {
    this.playlistError = 0;
  }
  getVariantLevelIndex(e) {
    return e?.type === N2.MAIN ? e.level : this.hls.loadLevel;
  }
  onManifestLoading() {
    this.playlistError = 0, this.penalizedRenditions = {};
  }
  onLevelUpdated() {
    this.playlistError = 0;
  }
  onError(e, t) {
    var s2, i;
    if (t.fatal)
      return;
    let r2 = this.hls, a2 = t.context;
    switch (t.details) {
      case A2.FRAG_LOAD_ERROR:
      case A2.FRAG_LOAD_TIMEOUT:
      case A2.KEY_LOAD_ERROR:
      case A2.KEY_LOAD_TIMEOUT:
        t.errorAction = this.getFragRetryOrSwitchAction(t);
        return;
      case A2.FRAG_PARSING_ERROR:
        if ((s2 = t.frag) != null && s2.gap) {
          t.errorAction = { action: ae3.DoNothing, flags: Te2.None };
          return;
        }
      case A2.FRAG_GAP:
      case A2.FRAG_DECRYPT_ERROR: {
        t.errorAction = this.getFragRetryOrSwitchAction(t), t.errorAction.action = ae3.SendAlternateToPenaltyBox;
        return;
      }
      case A2.LEVEL_EMPTY_ERROR:
      case A2.LEVEL_PARSING_ERROR:
        {
          var o, l2;
          let c = t.parent === N2.MAIN ? t.level : r2.loadLevel;
          t.details === A2.LEVEL_EMPTY_ERROR && ((o = t.context) != null && (l2 = o.levelDetails) != null && l2.live) ? t.errorAction = this.getPlaylistRetryOrSwitchAction(t, c) : (t.levelRetry = false, t.errorAction = this.getLevelSwitchAction(t, c));
        }
        return;
      case A2.LEVEL_LOAD_ERROR:
      case A2.LEVEL_LOAD_TIMEOUT:
        typeof a2?.level == "number" && (t.errorAction = this.getPlaylistRetryOrSwitchAction(t, a2.level));
        return;
      case A2.AUDIO_TRACK_LOAD_ERROR:
      case A2.AUDIO_TRACK_LOAD_TIMEOUT:
      case A2.SUBTITLE_LOAD_ERROR:
      case A2.SUBTITLE_TRACK_LOAD_TIMEOUT:
        if (a2) {
          let c = r2.levels[r2.loadLevel];
          if (c && (a2.type === W3.AUDIO_TRACK && c.hasAudioGroup(a2.groupId) || a2.type === W3.SUBTITLE_TRACK && c.hasSubtitleGroup(a2.groupId))) {
            t.errorAction = this.getPlaylistRetryOrSwitchAction(t, r2.loadLevel), t.errorAction.action = ae3.SendAlternateToPenaltyBox, t.errorAction.flags = Te2.MoveAllAlternatesMatchingHost;
            return;
          }
        }
        return;
      case A2.KEY_SYSTEM_STATUS_OUTPUT_RESTRICTED:
        {
          let c = r2.levels[r2.loadLevel], h2 = c?.attrs["HDCP-LEVEL"];
          h2 ? t.errorAction = { action: ae3.SendAlternateToPenaltyBox, flags: Te2.MoveAllAlternatesMatchingHDCP, hdcpLevel: h2 } : this.keySystemError(t);
        }
        return;
      case A2.BUFFER_ADD_CODEC_ERROR:
      case A2.REMUX_ALLOC_ERROR:
      case A2.BUFFER_APPEND_ERROR:
        t.errorAction = this.getLevelSwitchAction(t, (i = t.level) != null ? i : r2.loadLevel);
        return;
      case A2.INTERNAL_EXCEPTION:
      case A2.BUFFER_APPENDING_ERROR:
      case A2.BUFFER_FULL_ERROR:
      case A2.LEVEL_SWITCH_ERROR:
      case A2.BUFFER_STALLED_ERROR:
      case A2.BUFFER_SEEK_OVER_HOLE:
      case A2.BUFFER_NUDGE_ON_STALL:
        t.errorAction = { action: ae3.DoNothing, flags: Te2.None };
        return;
    }
    t.type === B2.KEY_SYSTEM_ERROR && this.keySystemError(t);
  }
  keySystemError(e) {
    let t = this.getVariantLevelIndex(e.frag);
    e.levelRetry = false, e.errorAction = this.getLevelSwitchAction(e, t);
  }
  getPlaylistRetryOrSwitchAction(e, t) {
    let s2 = this.hls, i = dr(s2.config.playlistLoadPolicy, e), r2 = this.playlistError++;
    if (Ot(i, r2, Mt(e), e.response))
      return { action: ae3.RetryRequest, flags: Te2.None, retryConfig: i, retryCount: r2 };
    let o = this.getLevelSwitchAction(e, t);
    return i && (o.retryConfig = i, o.retryCount = r2), o;
  }
  getFragRetryOrSwitchAction(e) {
    let t = this.hls, s2 = this.getVariantLevelIndex(e.frag), i = t.levels[s2], { fragLoadPolicy: r2, keyLoadPolicy: a2 } = t.config, o = dr(e.details.startsWith("key") ? a2 : r2, e), l2 = t.levels.reduce((h2, u2) => h2 + u2.fragmentError, 0);
    if (i && (e.details !== A2.FRAG_GAP && i.fragmentError++, Ot(o, l2, Mt(e), e.response)))
      return { action: ae3.RetryRequest, flags: Te2.None, retryConfig: o, retryCount: l2 };
    let c = this.getLevelSwitchAction(e, s2);
    return o && (c.retryConfig = o, c.retryCount = l2), c;
  }
  getLevelSwitchAction(e, t) {
    let s2 = this.hls;
    t == null && (t = s2.loadLevel);
    let i = this.hls.levels[t];
    if (i) {
      var r2, a2;
      let c = e.details;
      i.loadError++, c === A2.BUFFER_APPEND_ERROR && i.fragmentError++;
      let h2 = -1, { levels: u2, loadLevel: d3, minAutoLevel: f3, maxAutoLevel: g2 } = s2;
      s2.autoLevelEnabled || (s2.loadLevel = -1);
      let p3 = (r2 = e.frag) == null ? void 0 : r2.type, E4 = (p3 === N2.AUDIO && c === A2.FRAG_PARSING_ERROR || e.sourceBufferName === "audio" && (c === A2.BUFFER_ADD_CODEC_ERROR || c === A2.BUFFER_APPEND_ERROR)) && u2.some(({ audioCodec: b }) => i.audioCodec !== b), y3 = e.sourceBufferName === "video" && (c === A2.BUFFER_ADD_CODEC_ERROR || c === A2.BUFFER_APPEND_ERROR) && u2.some(({ codecSet: b, audioCodec: L }) => i.codecSet !== b && i.audioCodec === L), { type: R, groupId: S2 } = (a2 = e.context) != null ? a2 : {};
      for (let b = u2.length; b--; ) {
        let L = (b + d3) % u2.length;
        if (L !== d3 && L >= f3 && L <= g2 && u2[L].loadError === 0) {
          var o, l2;
          let C2 = u2[L];
          if (c === A2.FRAG_GAP && e.frag) {
            let _ = u2[L].details;
            if (_) {
              let I = Nt(e.frag, _.fragments, e.frag.start);
              if (I != null && I.gap)
                continue;
            }
          } else {
            if (R === W3.AUDIO_TRACK && C2.hasAudioGroup(S2) || R === W3.SUBTITLE_TRACK && C2.hasSubtitleGroup(S2))
              continue;
            if (p3 === N2.AUDIO && (o = i.audioGroups) != null && o.some((_) => C2.hasAudioGroup(_)) || p3 === N2.SUBTITLE && (l2 = i.subtitleGroups) != null && l2.some((_) => C2.hasSubtitleGroup(_)) || E4 && i.audioCodec === C2.audioCodec || !E4 && i.audioCodec !== C2.audioCodec || y3 && i.codecSet === C2.codecSet)
              continue;
          }
          h2 = L;
          break;
        }
      }
      if (h2 > -1 && s2.loadLevel !== h2)
        return e.levelRetry = true, this.playlistError = 0, { action: ae3.SendAlternateToPenaltyBox, flags: Te2.None, nextAutoLevel: h2 };
    }
    return { action: ae3.SendAlternateToPenaltyBox, flags: Te2.MoveAllAlternatesMatchingHost };
  }
  onErrorOut(e, t) {
    var s2;
    switch ((s2 = t.errorAction) == null ? void 0 : s2.action) {
      case ae3.DoNothing:
        break;
      case ae3.SendAlternateToPenaltyBox:
        this.sendAlternateToPenaltyBox(t), !t.errorAction.resolved && t.details !== A2.FRAG_GAP ? t.fatal = true : /MediaSource readyState: ended/.test(t.error.message) && (this.warn(`MediaSource ended after "${t.sourceBufferName}" sourceBuffer append error. Attempting to recover from media error.`), this.hls.recoverMediaError());
        break;
      case ae3.RetryRequest:
        break;
    }
    if (t.fatal) {
      this.hls.stopLoad();
      return;
    }
  }
  sendAlternateToPenaltyBox(e) {
    let t = this.hls, s2 = e.errorAction;
    if (!s2)
      return;
    let { flags: i, hdcpLevel: r2, nextAutoLevel: a2 } = s2;
    switch (i) {
      case Te2.None:
        this.switchLevel(e, a2);
        break;
      case Te2.MoveAllAlternatesMatchingHDCP:
        r2 && (t.maxHdcpLevel = ws[ws.indexOf(r2) - 1], s2.resolved = true), this.warn(`Restricting playback to HDCP-LEVEL of "${t.maxHdcpLevel}" or lower`);
        break;
    }
    s2.resolved || this.switchLevel(e, a2);
  }
  switchLevel(e, t) {
    t !== void 0 && e.errorAction && (this.warn(`switching to level ${t} after ${e.details}`), this.hls.nextAutoLevel = t, e.errorAction.resolved = true, this.hls.nextLoadLevel = this.hls.nextAutoLevel);
  }
};
var nt = class {
  constructor(e, t) {
    this.hls = void 0, this.timer = -1, this.requestScheduled = -1, this.canLoad = false, this.log = void 0, this.warn = void 0, this.log = v2.log.bind(v2, `${t}:`), this.warn = v2.warn.bind(v2, `${t}:`), this.hls = e;
  }
  destroy() {
    this.clearTimer(), this.hls = this.log = this.warn = null;
  }
  clearTimer() {
    this.timer !== -1 && (self.clearTimeout(this.timer), this.timer = -1);
  }
  startLoad() {
    this.canLoad = true, this.requestScheduled = -1, this.loadPlaylist();
  }
  stopLoad() {
    this.canLoad = false, this.clearTimer();
  }
  switchParams(e, t, s2) {
    let i = t?.renditionReports;
    if (i) {
      let r2 = -1;
      for (let a2 = 0; a2 < i.length; a2++) {
        let o = i[a2], l2;
        try {
          l2 = new self.URL(o.URI, t.url).href;
        } catch (c) {
          v2.warn(`Could not construct new URL for Rendition Report: ${c}`), l2 = o.URI || "";
        }
        if (l2 === e) {
          r2 = a2;
          break;
        } else
          l2 === e.substring(0, l2.length) && (r2 = a2);
      }
      if (r2 !== -1) {
        let a2 = i[r2], o = parseInt(a2["LAST-MSN"]) || t?.lastPartSn, l2 = parseInt(a2["LAST-PART"]) || t?.lastPartIndex;
        if (this.hls.config.lowLatencyMode) {
          let h2 = Math.min(t.age - t.partTarget, t.targetduration);
          l2 >= 0 && h2 > t.partTarget && (l2 += 1);
        }
        let c = s2 && cr(s2);
        return new Ft(o, l2 >= 0 ? l2 : void 0, c);
      }
    }
  }
  loadPlaylist(e) {
    this.requestScheduled === -1 && (this.requestScheduled = self.performance.now());
  }
  shouldLoadPlaylist(e) {
    return this.canLoad && !!e && !!e.url && (!e.details || e.details.live);
  }
  shouldReloadPlaylist(e) {
    return this.timer === -1 && this.requestScheduled === -1 && this.shouldLoadPlaylist(e);
  }
  playlistLoaded(e, t, s2) {
    let { details: i, stats: r2 } = t, a2 = self.performance.now(), o = r2.loading.first ? Math.max(0, a2 - r2.loading.first) : 0;
    if (i.advancedDateTime = Date.now() - o, i.live || s2 != null && s2.live) {
      if (i.reloaded(s2), s2 && this.log(`live playlist ${e} ${i.advanced ? "REFRESHED " + i.lastPartSn + "-" + i.lastPartIndex : i.updated ? "UPDATED" : "MISSED"}`), s2 && i.fragments.length > 0 && ja(s2, i), !this.canLoad || !i.live)
        return;
      let l2, c, h2;
      if (i.canBlockReload && i.endSN && i.advanced) {
        let T2 = this.hls.config.lowLatencyMode, E4 = i.lastPartSn, x3 = i.endSN, y3 = i.lastPartIndex, R = y3 !== -1, S2 = E4 === x3, b = T2 ? 0 : y3;
        R ? (c = S2 ? x3 + 1 : E4, h2 = S2 ? b : y3 + 1) : c = x3 + 1;
        let L = i.age, C2 = L + i.ageHeader, _ = Math.min(C2 - i.partTarget, i.targetduration * 1.5);
        if (_ > 0) {
          if (s2 && _ > s2.tuneInGoal)
            this.warn(`CDN Tune-in goal increased from: ${s2.tuneInGoal} to: ${_} with playlist age: ${i.age}`), _ = 0;
          else {
            let I = Math.floor(_ / i.targetduration);
            if (c += I, h2 !== void 0) {
              let w = Math.round(_ % i.targetduration / i.partTarget);
              h2 += w;
            }
            this.log(`CDN Tune-in age: ${i.ageHeader}s last advanced ${L.toFixed(2)}s goal: ${_} skip sn ${I} to part ${h2}`);
          }
          i.tuneInGoal = _;
        }
        if (l2 = this.getDeliveryDirectives(i, t.deliveryDirectives, c, h2), T2 || !S2) {
          this.loadPlaylist(l2);
          return;
        }
      } else
        (i.canBlockReload || i.canSkipUntil) && (l2 = this.getDeliveryDirectives(i, t.deliveryDirectives, c, h2));
      let u2 = this.hls.mainForwardBufferInfo, d3 = u2 ? u2.end - u2.len : 0, f3 = (i.edge - d3) * 1e3, g2 = Ja(i, f3);
      i.updated && a2 > this.requestScheduled + g2 && (this.requestScheduled = r2.loading.start), c !== void 0 && i.canBlockReload ? this.requestScheduled = r2.loading.first + g2 - (i.partTarget * 1e3 || 1e3) : this.requestScheduled === -1 || this.requestScheduled + g2 < a2 ? this.requestScheduled = a2 : this.requestScheduled - a2 <= 0 && (this.requestScheduled += g2);
      let p3 = this.requestScheduled - a2;
      p3 = Math.max(0, p3), this.log(`reload live playlist ${e} in ${Math.round(p3)} ms`), this.timer = self.setTimeout(() => this.loadPlaylist(l2), p3);
    } else
      this.clearTimer();
  }
  getDeliveryDirectives(e, t, s2, i) {
    let r2 = cr(e);
    return t != null && t.skip && e.deltaUpdateFailed && (s2 = t.msn, i = t.part, r2 = St.No), new Ft(s2, i, r2);
  }
  checkRetry(e) {
    let t = e.details, s2 = Mt(e), i = e.errorAction, { action: r2, retryCount: a2 = 0, retryConfig: o } = i || {}, l2 = !!i && !!o && (r2 === ae3.RetryRequest || !i.resolved && r2 === ae3.SendAlternateToPenaltyBox);
    if (l2) {
      var c;
      if (this.requestScheduled = -1, a2 >= o.maxNumRetry)
        return false;
      if (s2 && (c = e.context) != null && c.deliveryDirectives)
        this.warn(`Retrying playlist loading ${a2 + 1}/${o.maxNumRetry} after "${t}" without delivery-directives`), this.loadPlaylist();
      else {
        let h2 = Fi(o, a2);
        this.timer = self.setTimeout(() => this.loadPlaylist(), h2), this.warn(`Retrying playlist loading ${a2 + 1}/${o.maxNumRetry} after "${t}" in ${h2}ms`);
      }
      e.levelRetry = true, i.resolved = true;
    }
    return l2;
  }
};
var _e3 = class {
  constructor(e, t = 0, s2 = 0) {
    this.halfLife = void 0, this.alpha_ = void 0, this.estimate_ = void 0, this.totalWeight_ = void 0, this.halfLife = e, this.alpha_ = e ? Math.exp(Math.log(0.5) / e) : 0, this.estimate_ = t, this.totalWeight_ = s2;
  }
  sample(e, t) {
    let s2 = Math.pow(this.alpha_, e);
    this.estimate_ = t * (1 - s2) + s2 * this.estimate_, this.totalWeight_ += e;
  }
  getTotalWeight() {
    return this.totalWeight_;
  }
  getEstimate() {
    if (this.alpha_) {
      let e = 1 - Math.pow(this.alpha_, this.totalWeight_);
      if (e)
        return this.estimate_ / e;
    }
    return this.estimate_;
  }
};
var Fs = class {
  constructor(e, t, s2, i = 100) {
    this.defaultEstimate_ = void 0, this.minWeight_ = void 0, this.minDelayMs_ = void 0, this.slow_ = void 0, this.fast_ = void 0, this.defaultTTFB_ = void 0, this.ttfb_ = void 0, this.defaultEstimate_ = s2, this.minWeight_ = 1e-3, this.minDelayMs_ = 50, this.slow_ = new _e3(e), this.fast_ = new _e3(t), this.defaultTTFB_ = i, this.ttfb_ = new _e3(e);
  }
  update(e, t) {
    let { slow_: s2, fast_: i, ttfb_: r2 } = this;
    s2.halfLife !== e && (this.slow_ = new _e3(e, s2.getEstimate(), s2.getTotalWeight())), i.halfLife !== t && (this.fast_ = new _e3(t, i.getEstimate(), i.getTotalWeight())), r2.halfLife !== e && (this.ttfb_ = new _e3(e, r2.getEstimate(), r2.getTotalWeight()));
  }
  sample(e, t) {
    e = Math.max(e, this.minDelayMs_);
    let s2 = 8 * t, i = e / 1e3, r2 = s2 / i;
    this.fast_.sample(i, r2), this.slow_.sample(i, r2);
  }
  sampleTTFB(e) {
    let t = e / 1e3, s2 = Math.sqrt(2) * Math.exp(-Math.pow(t, 2) / 2);
    this.ttfb_.sample(s2, Math.max(e, 5));
  }
  canEstimate() {
    return this.fast_.getTotalWeight() >= this.minWeight_;
  }
  getEstimate() {
    return this.canEstimate() ? Math.min(this.fast_.getEstimate(), this.slow_.getEstimate()) : this.defaultEstimate_;
  }
  getEstimateTTFB() {
    return this.ttfb_.getTotalWeight() >= this.minWeight_ ? this.ttfb_.getEstimate() : this.defaultTTFB_;
  }
  destroy() {
  }
};
var ln = { supported: true, configurations: [], decodingInfoResults: [{ supported: true, powerEfficient: true, smooth: true }] };
var gr = {};
function ro(n12, e, t, s2, i, r2) {
  let a2 = n12.audioCodec ? n12.audioGroups : null, o = r2?.audioCodec, l2 = r2?.channels, c = l2 ? parseInt(l2) : o ? 1 / 0 : 2, h2 = null;
  if (a2 != null && a2.length)
    try {
      a2.length === 1 && a2[0] ? h2 = e.groups[a2[0]].channels : h2 = a2.reduce((u2, d3) => {
        if (d3) {
          let f3 = e.groups[d3];
          if (!f3)
            throw new Error(`Audio track group ${d3} not found`);
          Object.keys(f3.channels).forEach((g2) => {
            u2[g2] = (u2[g2] || 0) + f3.channels[g2];
          });
        }
        return u2;
      }, { 2: 0 });
    } catch {
      return true;
    }
  return n12.videoCodec !== void 0 && (n12.width > 1920 && n12.height > 1088 || n12.height > 1920 && n12.width > 1088 || n12.frameRate > Math.max(s2, 30) || n12.videoRange !== "SDR" && n12.videoRange !== t || n12.bitrate > Math.max(i, 8e6)) || !!h2 && F(c) && Object.keys(h2).some((u2) => parseInt(u2) > c);
}
function no(n12, e, t) {
  let s2 = n12.videoCodec, i = n12.audioCodec;
  if (!s2 || !i || !t)
    return Promise.resolve(ln);
  let r2 = { width: n12.width, height: n12.height, bitrate: Math.ceil(Math.max(n12.bitrate * 0.9, n12.averageBitrate)), framerate: n12.frameRate || 30 }, a2 = n12.videoRange;
  a2 !== "SDR" && (r2.transferFunction = a2.toLowerCase());
  let o = s2.split(",").map((l2) => ({ type: "media-source", video: le3(le3({}, r2), {}, { contentType: rt(l2, "video") }) }));
  return i && n12.audioGroups && n12.audioGroups.forEach((l2) => {
    var c;
    l2 && ((c = e.groups[l2]) == null || c.tracks.forEach((h2) => {
      if (h2.groupId === l2) {
        let u2 = h2.channels || "", d3 = parseFloat(u2);
        F(d3) && d3 > 2 && o.push.apply(o, i.split(",").map((f3) => ({ type: "media-source", audio: { contentType: rt(f3, "audio"), channels: "" + d3 } })));
      }
    }));
  }), Promise.all(o.map((l2) => {
    let c = ao(l2);
    return gr[c] || (gr[c] = t.decodingInfo(l2));
  })).then((l2) => ({ supported: !l2.some((c) => !c.supported), configurations: o, decodingInfoResults: l2 })).catch((l2) => ({ supported: false, configurations: o, decodingInfoResults: [], error: l2 }));
}
function ao(n12) {
  let { audio: e, video: t } = n12, s2 = t || e;
  if (s2) {
    let i = s2.contentType.split('"')[1];
    if (t)
      return `r${t.height}x${t.width}f${Math.ceil(t.framerate)}${t.transferFunction || "sd"}_${i}_${Math.ceil(t.bitrate / 1e5)}`;
    if (e)
      return `c${e.channels}${e.spatialRendering ? "s" : "n"}_${i}`;
  }
  return "";
}
function oo() {
  if (typeof matchMedia == "function") {
    let n12 = matchMedia("(dynamic-range: high)"), e = matchMedia("bad query");
    if (n12.media !== e.media)
      return n12.matches === true;
  }
  return false;
}
function lo(n12, e) {
  let t = false, s2 = [];
  return n12 && (t = n12 !== "SDR", s2 = [n12]), e && (s2 = e.allowedVideoRanges || kt.slice(0), t = e.preferHDR !== void 0 ? e.preferHDR : oo(), t ? s2 = s2.filter((i) => i !== "SDR") : s2 = ["SDR"]), { preferHDR: t, allowedVideoRanges: s2 };
}
function co(n12, e, t, s2, i) {
  let r2 = Object.keys(n12), a2 = s2?.channels, o = s2?.audioCodec, l2 = a2 && parseInt(a2) === 2, c = true, h2 = false, u2 = 1 / 0, d3 = 1 / 0, f3 = 1 / 0, g2 = 0, p3 = [], { preferHDR: T2, allowedVideoRanges: E4 } = lo(e, i);
  for (let S2 = r2.length; S2--; ) {
    let b = n12[r2[S2]];
    c = b.channels[2] > 0, u2 = Math.min(u2, b.minHeight), d3 = Math.min(d3, b.minFramerate), f3 = Math.min(f3, b.minBitrate);
    let L = E4.filter((C2) => b.videoRanges[C2] > 0);
    L.length > 0 && (h2 = true, p3 = L);
  }
  u2 = F(u2) ? u2 : 0, d3 = F(d3) ? d3 : 0;
  let x3 = Math.max(1080, u2), y3 = Math.max(30, d3);
  return f3 = F(f3) ? f3 : t, t = Math.max(f3, t), h2 || (e = void 0, p3 = []), { codecSet: r2.reduce((S2, b) => {
    let L = n12[b];
    if (b === S2)
      return S2;
    if (L.minBitrate > t)
      return be3(b, `min bitrate of ${L.minBitrate} > current estimate of ${t}`), S2;
    if (!L.hasDefaultAudio)
      return be3(b, "no renditions with default or auto-select sound found"), S2;
    if (o && b.indexOf(o.substring(0, 4)) % 5 !== 0)
      return be3(b, `audio codec preference "${o}" not found`), S2;
    if (a2 && !l2) {
      if (!L.channels[a2])
        return be3(b, `no renditions with ${a2} channel sound found (channels options: ${Object.keys(L.channels)})`), S2;
    } else if ((!o || l2) && c && L.channels[2] === 0)
      return be3(b, "no renditions with stereo sound found"), S2;
    return L.minHeight > x3 ? (be3(b, `min resolution of ${L.minHeight} > maximum of ${x3}`), S2) : L.minFramerate > y3 ? (be3(b, `min framerate of ${L.minFramerate} > maximum of ${y3}`), S2) : p3.some((C2) => L.videoRanges[C2] > 0) ? L.maxScore < g2 ? (be3(b, `max score of ${L.maxScore} < selected max of ${g2}`), S2) : S2 && (_t(b) >= _t(S2) || L.fragmentError > n12[S2].fragmentError) ? S2 : (g2 = L.maxScore, b) : (be3(b, `no variants with VIDEO-RANGE of ${JSON.stringify(p3)} found`), S2);
  }, void 0), videoRanges: p3, preferHDR: T2, minFramerate: d3, minBitrate: f3 };
}
function be3(n12, e) {
  v2.log(`[abr] start candidates with "${n12}" ignored because ${e}`);
}
function ho(n12) {
  return n12.reduce((e, t) => {
    let s2 = e.groups[t.groupId];
    s2 || (s2 = e.groups[t.groupId] = { tracks: [], channels: { 2: 0 }, hasDefault: false, hasAutoSelect: false }), s2.tracks.push(t);
    let i = t.channels || "2";
    return s2.channels[i] = (s2.channels[i] || 0) + 1, s2.hasDefault = s2.hasDefault || t.default, s2.hasAutoSelect = s2.hasAutoSelect || t.autoselect, s2.hasDefault && (e.hasDefaultAudio = true), s2.hasAutoSelect && (e.hasAutoSelectAudio = true), e;
  }, { hasDefaultAudio: false, hasAutoSelectAudio: false, groups: {} });
}
function uo(n12, e, t, s2) {
  return n12.slice(t, s2 + 1).reduce((i, r2) => {
    if (!r2.codecSet)
      return i;
    let a2 = r2.audioGroups, o = i[r2.codecSet];
    o || (i[r2.codecSet] = o = { minBitrate: 1 / 0, minHeight: 1 / 0, minFramerate: 1 / 0, maxScore: 0, videoRanges: { SDR: 0 }, channels: { 2: 0 }, hasDefaultAudio: !a2, fragmentError: 0 }), o.minBitrate = Math.min(o.minBitrate, r2.bitrate);
    let l2 = Math.min(r2.height, r2.width);
    return o.minHeight = Math.min(o.minHeight, l2), o.minFramerate = Math.min(o.minFramerate, r2.frameRate), o.maxScore = Math.max(o.maxScore, r2.score), o.fragmentError += r2.fragmentError, o.videoRanges[r2.videoRange] = (o.videoRanges[r2.videoRange] || 0) + 1, a2 && a2.forEach((c) => {
      if (!c)
        return;
      let h2 = e.groups[c];
      h2 && (o.hasDefaultAudio = o.hasDefaultAudio || e.hasDefaultAudio ? h2.hasDefault : h2.hasAutoSelect || !e.hasDefaultAudio && !e.hasAutoSelectAudio, Object.keys(h2.channels).forEach((u2) => {
        o.channels[u2] = (o.channels[u2] || 0) + h2.channels[u2];
      }));
    }), i;
  }, {});
}
function Le2(n12, e, t) {
  if ("attrs" in n12) {
    let s2 = e.indexOf(n12);
    if (s2 !== -1)
      return s2;
  }
  for (let s2 = 0; s2 < e.length; s2++) {
    let i = e[s2];
    if (Ye(n12, i, t))
      return s2;
  }
  return -1;
}
function Ye(n12, e, t) {
  let { groupId: s2, name: i, lang: r2, assocLang: a2, characteristics: o, default: l2 } = n12, c = n12.forced;
  return (s2 === void 0 || e.groupId === s2) && (i === void 0 || e.name === i) && (r2 === void 0 || e.lang === r2) && (r2 === void 0 || e.assocLang === a2) && (l2 === void 0 || e.default === l2) && (c === void 0 || e.forced === c) && (o === void 0 || fo(o, e.characteristics)) && (t === void 0 || t(n12, e));
}
function fo(n12, e = "") {
  let t = n12.split(","), s2 = e.split(",");
  return t.length === s2.length && !t.some((i) => s2.indexOf(i) === -1);
}
function Be2(n12, e) {
  let { audioCodec: t, channels: s2 } = n12;
  return (t === void 0 || (e.audioCodec || "").substring(0, 4) === t.substring(0, 4)) && (s2 === void 0 || s2 === (e.channels || "2"));
}
function go(n12, e, t, s2, i) {
  let r2 = e[s2], o = e.reduce((d3, f3, g2) => {
    let p3 = f3.uri;
    return (d3[p3] || (d3[p3] = [])).push(g2), d3;
  }, {})[r2.uri];
  o.length > 1 && (s2 = Math.max.apply(Math, o));
  let l2 = r2.videoRange, c = r2.frameRate, h2 = r2.codecSet.substring(0, 4), u2 = mr(e, s2, (d3) => {
    if (d3.videoRange !== l2 || d3.frameRate !== c || d3.codecSet.substring(0, 4) !== h2)
      return false;
    let f3 = d3.audioGroups, g2 = t.filter((p3) => !f3 || f3.indexOf(p3.groupId) !== -1);
    return Le2(n12, g2, i) > -1;
  });
  return u2 > -1 ? u2 : mr(e, s2, (d3) => {
    let f3 = d3.audioGroups, g2 = t.filter((p3) => !f3 || f3.indexOf(p3.groupId) !== -1);
    return Le2(n12, g2, i) > -1;
  });
}
function mr(n12, e, t) {
  for (let s2 = e; s2; s2--)
    if (t(n12[s2]))
      return s2;
  for (let s2 = e + 1; s2 < n12.length; s2++)
    if (t(n12[s2]))
      return s2;
  return -1;
}
var Ms = class {
  constructor(e) {
    this.hls = void 0, this.lastLevelLoadSec = 0, this.lastLoadedFragLevel = -1, this.firstSelection = -1, this._nextAutoLevel = -1, this.nextAutoLevelKey = "", this.audioTracksByGroup = null, this.codecTiers = null, this.timer = -1, this.fragCurrent = null, this.partCurrent = null, this.bitrateTestDelay = 0, this.bwEstimator = void 0, this._abandonRulesCheck = () => {
      let { fragCurrent: t, partCurrent: s2, hls: i } = this, { autoLevelEnabled: r2, media: a2 } = i;
      if (!t || !a2)
        return;
      let o = performance.now(), l2 = s2 ? s2.stats : t.stats, c = s2 ? s2.duration : t.duration, h2 = o - l2.loading.start, u2 = i.minAutoLevel;
      if (l2.aborted || l2.loaded && l2.loaded === l2.total || t.level <= u2) {
        this.clearTimer(), this._nextAutoLevel = -1;
        return;
      }
      if (!r2 || a2.paused || !a2.playbackRate || !a2.readyState)
        return;
      let d3 = i.mainForwardBufferInfo;
      if (d3 === null)
        return;
      let f3 = this.bwEstimator.getEstimateTTFB(), g2 = Math.abs(a2.playbackRate);
      if (h2 <= Math.max(f3, 1e3 * (c / (g2 * 2))))
        return;
      let p3 = d3.len / g2, T2 = l2.loading.first ? l2.loading.first - l2.loading.start : -1, E4 = l2.loaded && T2 > -1, x3 = this.getBwEstimate(), y3 = i.levels, R = y3[t.level], S2 = l2.total || Math.max(l2.loaded, Math.round(c * R.averageBitrate / 8)), b = E4 ? h2 - T2 : h2;
      b < 1 && E4 && (b = Math.min(h2, l2.loaded * 8 / x3));
      let L = E4 ? l2.loaded * 1e3 / b : 0, C2 = L ? (S2 - l2.loaded) / L : S2 * 8 / x3 + f3 / 1e3;
      if (C2 <= p3)
        return;
      let _ = L ? L * 8 : x3, I = Number.POSITIVE_INFINITY, w;
      for (w = t.level - 1; w > u2; w--) {
        let P2 = y3[w].maxBitrate;
        if (I = this.getTimeToLoadFrag(f3 / 1e3, _, c * P2, !y3[w].details), I < p3)
          break;
      }
      if (I >= C2 || I > c * 10)
        return;
      i.nextLoadLevel = i.nextAutoLevel = w, E4 ? this.bwEstimator.sample(h2 - Math.min(f3, T2), l2.loaded) : this.bwEstimator.sampleTTFB(h2);
      let K = y3[w].maxBitrate;
      this.getBwEstimate() * this.hls.config.abrBandWidthUpFactor > K && this.resetEstimator(K), this.clearTimer(), v2.warn(`[abr] Fragment ${t.sn}${s2 ? " part " + s2.index : ""} of level ${t.level} is loading too slowly;
      Time to underbuffer: ${p3.toFixed(3)} s
      Estimated load time for current fragment: ${C2.toFixed(3)} s
      Estimated load time for down switch fragment: ${I.toFixed(3)} s
      TTFB estimate: ${T2 | 0} ms
      Current BW estimate: ${F(x3) ? x3 | 0 : "Unknown"} bps
      New BW estimate: ${this.getBwEstimate() | 0} bps
      Switching to level ${w} @ ${K | 0} bps`), i.trigger(m2.FRAG_LOAD_EMERGENCY_ABORTED, { frag: t, part: s2, stats: l2 });
    }, this.hls = e, this.bwEstimator = this.initEstimator(), this.registerListeners();
  }
  resetEstimator(e) {
    e && (v2.log(`setting initial bwe to ${e}`), this.hls.config.abrEwmaDefaultEstimate = e), this.firstSelection = -1, this.bwEstimator = this.initEstimator();
  }
  initEstimator() {
    let e = this.hls.config;
    return new Fs(e.abrEwmaSlowVoD, e.abrEwmaFastVoD, e.abrEwmaDefaultEstimate);
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.FRAG_LOADING, this.onFragLoading, this), e.on(m2.FRAG_LOADED, this.onFragLoaded, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.on(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.on(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.on(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.on(m2.MAX_AUTO_LEVEL_UPDATED, this.onMaxAutoLevelUpdated, this), e.on(m2.ERROR, this.onError, this);
  }
  unregisterListeners() {
    let { hls: e } = this;
    e && (e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.FRAG_LOADING, this.onFragLoading, this), e.off(m2.FRAG_LOADED, this.onFragLoaded, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.off(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.off(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.off(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.off(m2.MAX_AUTO_LEVEL_UPDATED, this.onMaxAutoLevelUpdated, this), e.off(m2.ERROR, this.onError, this));
  }
  destroy() {
    this.unregisterListeners(), this.clearTimer(), this.hls = this._abandonRulesCheck = null, this.fragCurrent = this.partCurrent = null;
  }
  onManifestLoading(e, t) {
    this.lastLoadedFragLevel = -1, this.firstSelection = -1, this.lastLevelLoadSec = 0, this.fragCurrent = this.partCurrent = null, this.onLevelsUpdated(), this.clearTimer();
  }
  onLevelsUpdated() {
    this.lastLoadedFragLevel > -1 && this.fragCurrent && (this.lastLoadedFragLevel = this.fragCurrent.level), this._nextAutoLevel = -1, this.onMaxAutoLevelUpdated(), this.codecTiers = null, this.audioTracksByGroup = null;
  }
  onMaxAutoLevelUpdated() {
    this.firstSelection = -1, this.nextAutoLevelKey = "";
  }
  onFragLoading(e, t) {
    let s2 = t.frag;
    if (!this.ignoreFragment(s2)) {
      if (!s2.bitrateTest) {
        var i;
        this.fragCurrent = s2, this.partCurrent = (i = t.part) != null ? i : null;
      }
      this.clearTimer(), this.timer = self.setInterval(this._abandonRulesCheck, 100);
    }
  }
  onLevelSwitching(e, t) {
    this.clearTimer();
  }
  onError(e, t) {
    if (!t.fatal)
      switch (t.details) {
        case A2.BUFFER_ADD_CODEC_ERROR:
        case A2.BUFFER_APPEND_ERROR:
          this.lastLoadedFragLevel = -1, this.firstSelection = -1;
          break;
        case A2.FRAG_LOAD_TIMEOUT: {
          let s2 = t.frag, { fragCurrent: i, partCurrent: r2 } = this;
          if (s2 && i && s2.sn === i.sn && s2.level === i.level) {
            let a2 = performance.now(), o = r2 ? r2.stats : s2.stats, l2 = a2 - o.loading.start, c = o.loading.first ? o.loading.first - o.loading.start : -1;
            if (o.loaded && c > -1) {
              let u2 = this.bwEstimator.getEstimateTTFB();
              this.bwEstimator.sample(l2 - Math.min(u2, c), o.loaded);
            } else
              this.bwEstimator.sampleTTFB(l2);
          }
          break;
        }
      }
  }
  getTimeToLoadFrag(e, t, s2, i) {
    let r2 = e + s2 / t, a2 = i ? this.lastLevelLoadSec : 0;
    return r2 + a2;
  }
  onLevelLoaded(e, t) {
    let s2 = this.hls.config, { loading: i } = t.stats, r2 = i.end - i.start;
    F(r2) && (this.lastLevelLoadSec = r2 / 1e3), t.details.live ? this.bwEstimator.update(s2.abrEwmaSlowLive, s2.abrEwmaFastLive) : this.bwEstimator.update(s2.abrEwmaSlowVoD, s2.abrEwmaFastVoD);
  }
  onFragLoaded(e, { frag: t, part: s2 }) {
    let i = s2 ? s2.stats : t.stats;
    if (t.type === N2.MAIN && this.bwEstimator.sampleTTFB(i.loading.first - i.loading.start), !this.ignoreFragment(t)) {
      if (this.clearTimer(), t.level === this._nextAutoLevel && (this._nextAutoLevel = -1), this.firstSelection = -1, this.hls.config.abrMaxWithRealBitrate) {
        let r2 = s2 ? s2.duration : t.duration, a2 = this.hls.levels[t.level], o = (a2.loaded ? a2.loaded.bytes : 0) + i.loaded, l2 = (a2.loaded ? a2.loaded.duration : 0) + r2;
        a2.loaded = { bytes: o, duration: l2 }, a2.realBitrate = Math.round(8 * o / l2);
      }
      if (t.bitrateTest) {
        let r2 = { stats: i, frag: t, part: s2, id: t.type };
        this.onFragBuffered(m2.FRAG_BUFFERED, r2), t.bitrateTest = false;
      } else
        this.lastLoadedFragLevel = t.level;
    }
  }
  onFragBuffered(e, t) {
    let { frag: s2, part: i } = t, r2 = i != null && i.stats.loaded ? i.stats : s2.stats;
    if (r2.aborted || this.ignoreFragment(s2))
      return;
    let a2 = r2.parsing.end - r2.loading.start - Math.min(r2.loading.first - r2.loading.start, this.bwEstimator.getEstimateTTFB());
    this.bwEstimator.sample(a2, r2.loaded), r2.bwEstimate = this.getBwEstimate(), s2.bitrateTest ? this.bitrateTestDelay = a2 / 1e3 : this.bitrateTestDelay = 0;
  }
  ignoreFragment(e) {
    return e.type !== N2.MAIN || e.sn === "initSegment";
  }
  clearTimer() {
    this.timer > -1 && (self.clearInterval(this.timer), this.timer = -1);
  }
  get firstAutoLevel() {
    let { maxAutoLevel: e, minAutoLevel: t } = this.hls, s2 = this.getBwEstimate(), i = this.hls.config.maxStarvationDelay, r2 = this.findBestLevel(s2, t, e, 0, i, 1, 1);
    if (r2 > -1)
      return r2;
    let a2 = this.hls.firstLevel, o = Math.min(Math.max(a2, t), e);
    return v2.warn(`[abr] Could not find best starting auto level. Defaulting to first in playlist ${a2} clamped to ${o}`), o;
  }
  get forcedAutoLevel() {
    return this.nextAutoLevelKey ? -1 : this._nextAutoLevel;
  }
  get nextAutoLevel() {
    let e = this.forcedAutoLevel, s2 = this.bwEstimator.canEstimate(), i = this.lastLoadedFragLevel > -1;
    if (e !== -1 && (!s2 || !i || this.nextAutoLevelKey === this.getAutoLevelKey()))
      return e;
    let r2 = s2 && i ? this.getNextABRAutoLevel() : this.firstAutoLevel;
    if (e !== -1) {
      let a2 = this.hls.levels;
      if (a2.length > Math.max(e, r2) && a2[e].loadError <= a2[r2].loadError)
        return e;
    }
    return this._nextAutoLevel = r2, this.nextAutoLevelKey = this.getAutoLevelKey(), r2;
  }
  getAutoLevelKey() {
    return `${this.getBwEstimate()}_${this.getStarvationDelay().toFixed(2)}`;
  }
  getNextABRAutoLevel() {
    let { fragCurrent: e, partCurrent: t, hls: s2 } = this, { maxAutoLevel: i, config: r2, minAutoLevel: a2 } = s2, o = t ? t.duration : e ? e.duration : 0, l2 = this.getBwEstimate(), c = this.getStarvationDelay(), h2 = r2.abrBandWidthFactor, u2 = r2.abrBandWidthUpFactor;
    if (c) {
      let T2 = this.findBestLevel(l2, a2, i, c, 0, h2, u2);
      if (T2 >= 0)
        return T2;
    }
    let d3 = o ? Math.min(o, r2.maxStarvationDelay) : r2.maxStarvationDelay;
    if (!c) {
      let T2 = this.bitrateTestDelay;
      T2 && (d3 = (o ? Math.min(o, r2.maxLoadingDelay) : r2.maxLoadingDelay) - T2, v2.info(`[abr] bitrate test took ${Math.round(1e3 * T2)}ms, set first fragment max fetchDuration to ${Math.round(1e3 * d3)} ms`), h2 = u2 = 1);
    }
    let f3 = this.findBestLevel(l2, a2, i, c, d3, h2, u2);
    if (v2.info(`[abr] ${c ? "rebuffering expected" : "buffer is empty"}, optimal quality level ${f3}`), f3 > -1)
      return f3;
    let g2 = s2.levels[a2], p3 = s2.levels[s2.loadLevel];
    return g2?.bitrate < p3?.bitrate ? a2 : s2.loadLevel;
  }
  getStarvationDelay() {
    let e = this.hls, t = e.media;
    if (!t)
      return 1 / 0;
    let s2 = t && t.playbackRate !== 0 ? Math.abs(t.playbackRate) : 1, i = e.mainForwardBufferInfo;
    return (i ? i.len : 0) / s2;
  }
  getBwEstimate() {
    return this.bwEstimator.canEstimate() ? this.bwEstimator.getEstimate() : this.hls.config.abrEwmaDefaultEstimate;
  }
  findBestLevel(e, t, s2, i, r2, a2, o) {
    var l2;
    let c = i + r2, h2 = this.lastLoadedFragLevel, u2 = h2 === -1 ? this.hls.firstLevel : h2, { fragCurrent: d3, partCurrent: f3 } = this, { levels: g2, allAudioTracks: p3, loadLevel: T2, config: E4 } = this.hls;
    if (g2.length === 1)
      return 0;
    let x3 = g2[u2], y3 = !!(x3 != null && (l2 = x3.details) != null && l2.live), R = T2 === -1 || h2 === -1, S2, b = "SDR", L = x3?.frameRate || 0, { audioPreference: C2, videoPreference: _ } = E4, I = this.audioTracksByGroup || (this.audioTracksByGroup = ho(p3));
    if (R) {
      if (this.firstSelection !== -1)
        return this.firstSelection;
      let $3 = this.codecTiers || (this.codecTiers = uo(g2, I, t, s2)), U3 = co($3, b, e, C2, _), { codecSet: Y3, videoRanges: X, minFramerate: M2, minBitrate: k, preferHDR: q } = U3;
      S2 = Y3, b = q ? X[X.length - 1] : X[0], L = M2, e = Math.max(e, k), v2.log(`[abr] picked start tier ${JSON.stringify(U3)}`);
    } else
      S2 = x3?.codecSet, b = x3?.videoRange;
    let w = f3 ? f3.duration : d3 ? d3.duration : 0, K = this.bwEstimator.getEstimateTTFB() / 1e3, P2 = [];
    for (let $3 = s2; $3 >= t; $3--) {
      var G2;
      let U3 = g2[$3], Y3 = $3 > u2;
      if (!U3)
        continue;
      if (E4.useMediaCapabilities && !U3.supportedResult && !U3.supportedPromise) {
        let ee2 = navigator.mediaCapabilities;
        typeof ee2?.decodingInfo == "function" && ro(U3, I, b, L, e, C2) ? (U3.supportedPromise = no(U3, I, ee2), U3.supportedPromise.then((re2) => {
          if (!this.hls)
            return;
          U3.supportedResult = re2;
          let ce2 = this.hls.levels, ge3 = ce2.indexOf(U3);
          re2.error ? v2.warn(`[abr] MediaCapabilities decodingInfo error: "${re2.error}" for level ${ge3} ${JSON.stringify(re2)}`) : re2.supported || (v2.warn(`[abr] Unsupported MediaCapabilities decodingInfo result for level ${ge3} ${JSON.stringify(re2)}`), ge3 > -1 && ce2.length > 1 && (v2.log(`[abr] Removing unsupported level ${ge3}`), this.hls.removeLevel(ge3)));
        })) : U3.supportedResult = ln;
      }
      if (S2 && U3.codecSet !== S2 || b && U3.videoRange !== b || Y3 && L > U3.frameRate || !Y3 && L > 0 && L < U3.frameRate || U3.supportedResult && !((G2 = U3.supportedResult.decodingInfoResults) != null && G2[0].smooth)) {
        P2.push($3);
        continue;
      }
      let X = U3.details, M2 = (f3 ? X?.partTarget : X?.averagetargetduration) || w, k;
      Y3 ? k = o * e : k = a2 * e;
      let q = w && i >= w * 2 && r2 === 0 ? g2[$3].averageBitrate : g2[$3].maxBitrate, V = this.getTimeToLoadFrag(K, k, q * M2, X === void 0);
      if (k >= q && ($3 === h2 || U3.loadError === 0 && U3.fragmentError === 0) && (V <= K || !F(V) || y3 && !this.bitrateTestDelay || V < c)) {
        let ee2 = this.forcedAutoLevel;
        return $3 !== T2 && (ee2 === -1 || ee2 !== T2) && (P2.length && v2.trace(`[abr] Skipped level(s) ${P2.join(",")} of ${s2} max with CODECS and VIDEO-RANGE:"${g2[P2[0]].codecs}" ${g2[P2[0]].videoRange}; not compatible with "${x3.codecs}" ${b}`), v2.info(`[abr] switch candidate:${u2}->${$3} adjustedbw(${Math.round(k)})-bitrate=${Math.round(k - q)} ttfb:${K.toFixed(1)} avgDuration:${M2.toFixed(1)} maxFetchDuration:${c.toFixed(1)} fetchDuration:${V.toFixed(1)} firstSelection:${R} codecSet:${S2} videoRange:${b} hls.loadLevel:${T2}`)), R && (this.firstSelection = $3), $3;
      }
    }
    return -1;
  }
  set nextAutoLevel(e) {
    let { maxAutoLevel: t, minAutoLevel: s2 } = this.hls, i = Math.min(Math.max(e, s2), t);
    this._nextAutoLevel !== i && (this.nextAutoLevelKey = "", this._nextAutoLevel = i);
  }
};
var Os = class {
  constructor() {
    this._boundTick = void 0, this._tickTimer = null, this._tickInterval = null, this._tickCallCount = 0, this._boundTick = this.tick.bind(this);
  }
  destroy() {
    this.onHandlerDestroying(), this.onHandlerDestroyed();
  }
  onHandlerDestroying() {
    this.clearNextTick(), this.clearInterval();
  }
  onHandlerDestroyed() {
  }
  hasInterval() {
    return !!this._tickInterval;
  }
  hasNextTick() {
    return !!this._tickTimer;
  }
  setInterval(e) {
    return this._tickInterval ? false : (this._tickCallCount = 0, this._tickInterval = self.setInterval(this._boundTick, e), true);
  }
  clearInterval() {
    return this._tickInterval ? (self.clearInterval(this._tickInterval), this._tickInterval = null, true) : false;
  }
  clearNextTick() {
    return this._tickTimer ? (self.clearTimeout(this._tickTimer), this._tickTimer = null, true) : false;
  }
  tick() {
    this._tickCallCount++, this._tickCallCount === 1 && (this.doTick(), this._tickCallCount > 1 && this.tickImmediate(), this._tickCallCount = 0);
  }
  tickImmediate() {
    this.clearNextTick(), this._tickTimer = self.setTimeout(this._boundTick, 0);
  }
  doTick() {
  }
};
var oe2 = { NOT_LOADED: "NOT_LOADED", APPENDING: "APPENDING", PARTIAL: "PARTIAL", OK: "OK" };
var Ns = class {
  constructor(e) {
    this.activePartLists = /* @__PURE__ */ Object.create(null), this.endListFragments = /* @__PURE__ */ Object.create(null), this.fragments = /* @__PURE__ */ Object.create(null), this.timeRanges = /* @__PURE__ */ Object.create(null), this.bufferPadding = 0.2, this.hls = void 0, this.hasGaps = false, this.hls = e, this._registerListeners();
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.BUFFER_APPENDED, this.onBufferAppended, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.on(m2.FRAG_LOADED, this.onFragLoaded, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.BUFFER_APPENDED, this.onBufferAppended, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.off(m2.FRAG_LOADED, this.onFragLoaded, this);
  }
  destroy() {
    this._unregisterListeners(), this.fragments = this.activePartLists = this.endListFragments = this.timeRanges = null;
  }
  getAppendedFrag(e, t) {
    let s2 = this.activePartLists[t];
    if (s2)
      for (let i = s2.length; i--; ) {
        let r2 = s2[i];
        if (!r2)
          break;
        let a2 = r2.end;
        if (r2.start <= e && a2 !== null && e <= a2)
          return r2;
      }
    return this.getBufferedFrag(e, t);
  }
  getBufferedFrag(e, t) {
    let { fragments: s2 } = this, i = Object.keys(s2);
    for (let r2 = i.length; r2--; ) {
      let a2 = s2[i[r2]];
      if (a2?.body.type === t && a2.buffered) {
        let o = a2.body;
        if (o.start <= e && e <= o.end)
          return o;
      }
    }
    return null;
  }
  detectEvictedFragments(e, t, s2, i) {
    this.timeRanges && (this.timeRanges[e] = t);
    let r2 = i?.fragment.sn || -1;
    Object.keys(this.fragments).forEach((a2) => {
      let o = this.fragments[a2];
      if (!o || r2 >= o.body.sn)
        return;
      if (!o.buffered && !o.loaded) {
        o.body.type === s2 && this.removeFragment(o.body);
        return;
      }
      let l2 = o.range[e];
      l2 && l2.time.some((c) => {
        let h2 = !this.isTimeBuffered(c.startPTS, c.endPTS, t);
        return h2 && this.removeFragment(o.body), h2;
      });
    });
  }
  detectPartialFragments(e) {
    let t = this.timeRanges, { frag: s2, part: i } = e;
    if (!t || s2.sn === "initSegment")
      return;
    let r2 = $e2(s2), a2 = this.fragments[r2];
    if (!a2 || a2.buffered && s2.gap)
      return;
    let o = !s2.relurl;
    Object.keys(t).forEach((l2) => {
      let c = s2.elementaryStreams[l2];
      if (!c)
        return;
      let h2 = t[l2], u2 = o || c.partial === true;
      a2.range[l2] = this.getBufferedTimes(s2, i, u2, h2);
    }), a2.loaded = null, Object.keys(a2.range).length ? (a2.buffered = true, (a2.body.endList = s2.endList || a2.body.endList) && (this.endListFragments[a2.body.type] = a2), gt(a2) || this.removeParts(s2.sn - 1, s2.type)) : this.removeFragment(a2.body);
  }
  removeParts(e, t) {
    let s2 = this.activePartLists[t];
    s2 && (this.activePartLists[t] = s2.filter((i) => i.fragment.sn >= e));
  }
  fragBuffered(e, t) {
    let s2 = $e2(e), i = this.fragments[s2];
    !i && t && (i = this.fragments[s2] = { body: e, appendedPTS: null, loaded: null, buffered: false, range: /* @__PURE__ */ Object.create(null) }, e.gap && (this.hasGaps = true)), i && (i.loaded = null, i.buffered = true);
  }
  getBufferedTimes(e, t, s2, i) {
    let r2 = { time: [], partial: s2 }, a2 = e.start, o = e.end, l2 = e.minEndPTS || o, c = e.maxStartPTS || a2;
    for (let h2 = 0; h2 < i.length; h2++) {
      let u2 = i.start(h2) - this.bufferPadding, d3 = i.end(h2) + this.bufferPadding;
      if (c >= u2 && l2 <= d3) {
        r2.time.push({ startPTS: Math.max(a2, i.start(h2)), endPTS: Math.min(o, i.end(h2)) });
        break;
      } else if (a2 < d3 && o > u2) {
        let f3 = Math.max(a2, i.start(h2)), g2 = Math.min(o, i.end(h2));
        g2 > f3 && (r2.partial = true, r2.time.push({ startPTS: f3, endPTS: g2 }));
      } else if (o <= u2)
        break;
    }
    return r2;
  }
  getPartialFragment(e) {
    let t = null, s2, i, r2, a2 = 0, { bufferPadding: o, fragments: l2 } = this;
    return Object.keys(l2).forEach((c) => {
      let h2 = l2[c];
      h2 && gt(h2) && (i = h2.body.start - o, r2 = h2.body.end + o, e >= i && e <= r2 && (s2 = Math.min(e - i, r2 - e), a2 <= s2 && (t = h2.body, a2 = s2)));
    }), t;
  }
  isEndListAppended(e) {
    let t = this.endListFragments[e];
    return t !== void 0 && (t.buffered || gt(t));
  }
  getState(e) {
    let t = $e2(e), s2 = this.fragments[t];
    return s2 ? s2.buffered ? gt(s2) ? oe2.PARTIAL : oe2.OK : oe2.APPENDING : oe2.NOT_LOADED;
  }
  isTimeBuffered(e, t, s2) {
    let i, r2;
    for (let a2 = 0; a2 < s2.length; a2++) {
      if (i = s2.start(a2) - this.bufferPadding, r2 = s2.end(a2) + this.bufferPadding, e >= i && t <= r2)
        return true;
      if (t <= i)
        return false;
    }
    return false;
  }
  onFragLoaded(e, t) {
    let { frag: s2, part: i } = t;
    if (s2.sn === "initSegment" || s2.bitrateTest)
      return;
    let r2 = i ? null : t, a2 = $e2(s2);
    this.fragments[a2] = { body: s2, appendedPTS: null, loaded: r2, buffered: false, range: /* @__PURE__ */ Object.create(null) };
  }
  onBufferAppended(e, t) {
    let { frag: s2, part: i, timeRanges: r2 } = t;
    if (s2.sn === "initSegment")
      return;
    let a2 = s2.type;
    if (i) {
      let o = this.activePartLists[a2];
      o || (this.activePartLists[a2] = o = []), o.push(i);
    }
    this.timeRanges = r2, Object.keys(r2).forEach((o) => {
      let l2 = r2[o];
      this.detectEvictedFragments(o, l2, a2, i);
    });
  }
  onFragBuffered(e, t) {
    this.detectPartialFragments(t);
  }
  hasFragment(e) {
    let t = $e2(e);
    return !!this.fragments[t];
  }
  hasParts(e) {
    var t;
    return !!((t = this.activePartLists[e]) != null && t.length);
  }
  removeFragmentsInRange(e, t, s2, i, r2) {
    i && !this.hasGaps || Object.keys(this.fragments).forEach((a2) => {
      let o = this.fragments[a2];
      if (!o)
        return;
      let l2 = o.body;
      l2.type !== s2 || i && !l2.gap || l2.start < t && l2.end > e && (o.buffered || r2) && this.removeFragment(l2);
    });
  }
  removeFragment(e) {
    let t = $e2(e);
    e.stats.loaded = 0, e.clearElementaryStreamInfo();
    let s2 = this.activePartLists[e.type];
    if (s2) {
      let i = e.sn;
      this.activePartLists[e.type] = s2.filter((r2) => r2.fragment.sn !== i);
    }
    delete this.fragments[t], e.endList && delete this.endListFragments[e.type];
  }
  removeAllFragments() {
    this.fragments = /* @__PURE__ */ Object.create(null), this.endListFragments = /* @__PURE__ */ Object.create(null), this.activePartLists = /* @__PURE__ */ Object.create(null), this.hasGaps = false;
  }
};
function gt(n12) {
  var e, t, s2;
  return n12.buffered && (n12.body.gap || ((e = n12.range.video) == null ? void 0 : e.partial) || ((t = n12.range.audio) == null ? void 0 : t.partial) || ((s2 = n12.range.audiovideo) == null ? void 0 : s2.partial));
}
function $e2(n12) {
  return `${n12.type}_${n12.level}_${n12.sn}`;
}
var mo = { length: 0, start: () => 0, end: () => 0 };
var Q = class n5 {
  static isBuffered(e, t) {
    try {
      if (e) {
        let s2 = n5.getBuffered(e);
        for (let i = 0; i < s2.length; i++)
          if (t >= s2.start(i) && t <= s2.end(i))
            return true;
      }
    } catch {
    }
    return false;
  }
  static bufferInfo(e, t, s2) {
    try {
      if (e) {
        let i = n5.getBuffered(e), r2 = [], a2;
        for (a2 = 0; a2 < i.length; a2++)
          r2.push({ start: i.start(a2), end: i.end(a2) });
        return this.bufferedInfo(r2, t, s2);
      }
    } catch {
    }
    return { len: 0, start: t, end: t, nextStart: void 0 };
  }
  static bufferedInfo(e, t, s2) {
    t = Math.max(0, t), e.sort(function(c, h2) {
      let u2 = c.start - h2.start;
      return u2 || h2.end - c.end;
    });
    let i = [];
    if (s2)
      for (let c = 0; c < e.length; c++) {
        let h2 = i.length;
        if (h2) {
          let u2 = i[h2 - 1].end;
          e[c].start - u2 < s2 ? e[c].end > u2 && (i[h2 - 1].end = e[c].end) : i.push(e[c]);
        } else
          i.push(e[c]);
      }
    else
      i = e;
    let r2 = 0, a2, o = t, l2 = t;
    for (let c = 0; c < i.length; c++) {
      let h2 = i[c].start, u2 = i[c].end;
      if (t + s2 >= h2 && t < u2)
        o = h2, l2 = u2, r2 = l2 - t;
      else if (t + s2 < h2) {
        a2 = h2;
        break;
      }
    }
    return { len: r2, start: o || 0, end: l2 || 0, nextStart: a2 };
  }
  static getBuffered(e) {
    try {
      return e.buffered;
    } catch (t) {
      return v2.log("failed to get media.buffered", t), mo;
    }
  }
};
var at = class {
  constructor(e, t, s2, i = 0, r2 = -1, a2 = false) {
    this.level = void 0, this.sn = void 0, this.part = void 0, this.id = void 0, this.size = void 0, this.partial = void 0, this.transmuxing = mt(), this.buffering = { audio: mt(), video: mt(), audiovideo: mt() }, this.level = e, this.sn = t, this.id = s2, this.size = i, this.part = r2, this.partial = a2;
  }
};
function mt() {
  return { start: 0, executeStart: 0, executeEnd: 0, end: 0 };
}
function vt(n12, e) {
  for (let s2 = 0, i = n12.length; s2 < i; s2++) {
    var t;
    if (((t = n12[s2]) == null ? void 0 : t.cc) === e)
      return n12[s2];
  }
  return null;
}
function po(n12, e, t) {
  return !!(e && (t.endCC > t.startCC || n12 && n12.cc < t.startCC));
}
function To(n12, e) {
  let t = n12.fragments, s2 = e.fragments;
  if (!s2.length || !t.length) {
    v2.log("No fragments to align");
    return;
  }
  let i = vt(t, s2[0].cc);
  if (!i || i && !i.startPTS) {
    v2.log("No frag in previous level to align on");
    return;
  }
  return i;
}
function pr(n12, e) {
  if (n12) {
    let t = n12.start + e;
    n12.start = n12.startPTS = t, n12.endPTS = t + n12.duration;
  }
}
function cn(n12, e) {
  let t = e.fragments;
  for (let s2 = 0, i = t.length; s2 < i; s2++)
    pr(t[s2], n12);
  e.fragmentHint && pr(e.fragmentHint, n12), e.alignedSliding = true;
}
function Eo(n12, e, t) {
  e && (yo(n12, t, e), !t.alignedSliding && e && Ut(t, e), !t.alignedSliding && e && !t.skippedSegments && rn(e, t));
}
function yo(n12, e, t) {
  if (po(n12, t, e)) {
    let s2 = To(t, e);
    s2 && F(s2.start) && (v2.log(`Adjusting PTS using last level due to CC increase within current level ${e.url}`), cn(s2.start, e));
  }
}
function Ut(n12, e) {
  if (!n12.hasProgramDateTime || !e.hasProgramDateTime)
    return;
  let t = n12.fragments, s2 = e.fragments;
  if (!t.length || !s2.length)
    return;
  let i, r2, a2 = Math.min(e.endCC, n12.endCC);
  e.startCC < a2 && n12.startCC < a2 && (i = vt(s2, a2), r2 = vt(t, a2)), (!i || !r2) && (i = s2[Math.floor(s2.length / 2)], r2 = vt(t, i.cc) || t[Math.floor(t.length / 2)]);
  let o = i.programDateTime, l2 = r2.programDateTime;
  if (!o || !l2)
    return;
  let c = (l2 - o) / 1e3 - (r2.start - i.start);
  cn(c, n12);
}
var Tr = Math.pow(2, 17);
var Us = class {
  constructor(e) {
    this.config = void 0, this.loader = null, this.partLoadTimeout = -1, this.config = e;
  }
  destroy() {
    this.loader && (this.loader.destroy(), this.loader = null);
  }
  abort() {
    this.loader && this.loader.abort();
  }
  load(e, t) {
    let s2 = e.url;
    if (!s2)
      return Promise.reject(new ye3({ type: B2.NETWORK_ERROR, details: A2.FRAG_LOAD_ERROR, fatal: false, frag: e, error: new Error(`Fragment does not have a ${s2 ? "part list" : "url"}`), networkDetails: null }));
    this.abort();
    let i = this.config, r2 = i.fLoader, a2 = i.loader;
    return new Promise((o, l2) => {
      if (this.loader && this.loader.destroy(), e.gap)
        if (e.tagList.some((f3) => f3[0] === "GAP")) {
          l2(yr(e));
          return;
        } else
          e.gap = false;
      let c = this.loader = e.loader = r2 ? new r2(i) : new a2(i), h2 = Er(e), u2 = fr(i.fragLoadPolicy.default), d3 = { loadPolicy: u2, timeout: u2.maxLoadTimeMs, maxRetry: 0, retryDelay: 0, maxRetryDelay: 0, highWaterMark: e.sn === "initSegment" ? 1 / 0 : Tr };
      e.stats = c.stats, c.load(h2, d3, { onSuccess: (f3, g2, p3, T2) => {
        this.resetLoader(e, c);
        let E4 = f3.data;
        p3.resetIV && e.decryptdata && (e.decryptdata.iv = new Uint8Array(E4.slice(0, 16)), E4 = E4.slice(16)), o({ frag: e, part: null, payload: E4, networkDetails: T2 });
      }, onError: (f3, g2, p3, T2) => {
        this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.FRAG_LOAD_ERROR, fatal: false, frag: e, response: le3({ url: s2, data: void 0 }, f3), error: new Error(`HTTP Error ${f3.code} ${f3.text}`), networkDetails: p3, stats: T2 }));
      }, onAbort: (f3, g2, p3) => {
        this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.INTERNAL_ABORTED, fatal: false, frag: e, error: new Error("Aborted"), networkDetails: p3, stats: f3 }));
      }, onTimeout: (f3, g2, p3) => {
        this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.FRAG_LOAD_TIMEOUT, fatal: false, frag: e, error: new Error(`Timeout after ${d3.timeout}ms`), networkDetails: p3, stats: f3 }));
      }, onProgress: (f3, g2, p3, T2) => {
        t && t({ frag: e, part: null, payload: p3, networkDetails: T2 });
      } });
    });
  }
  loadPart(e, t, s2) {
    this.abort();
    let i = this.config, r2 = i.fLoader, a2 = i.loader;
    return new Promise((o, l2) => {
      if (this.loader && this.loader.destroy(), e.gap || t.gap) {
        l2(yr(e, t));
        return;
      }
      let c = this.loader = e.loader = r2 ? new r2(i) : new a2(i), h2 = Er(e, t), u2 = fr(i.fragLoadPolicy.default), d3 = { loadPolicy: u2, timeout: u2.maxLoadTimeMs, maxRetry: 0, retryDelay: 0, maxRetryDelay: 0, highWaterMark: Tr };
      t.stats = c.stats, c.load(h2, d3, { onSuccess: (f3, g2, p3, T2) => {
        this.resetLoader(e, c), this.updateStatsFromPart(e, t);
        let E4 = { frag: e, part: t, payload: f3.data, networkDetails: T2 };
        s2(E4), o(E4);
      }, onError: (f3, g2, p3, T2) => {
        this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.FRAG_LOAD_ERROR, fatal: false, frag: e, part: t, response: le3({ url: h2.url, data: void 0 }, f3), error: new Error(`HTTP Error ${f3.code} ${f3.text}`), networkDetails: p3, stats: T2 }));
      }, onAbort: (f3, g2, p3) => {
        e.stats.aborted = t.stats.aborted, this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.INTERNAL_ABORTED, fatal: false, frag: e, part: t, error: new Error("Aborted"), networkDetails: p3, stats: f3 }));
      }, onTimeout: (f3, g2, p3) => {
        this.resetLoader(e, c), l2(new ye3({ type: B2.NETWORK_ERROR, details: A2.FRAG_LOAD_TIMEOUT, fatal: false, frag: e, part: t, error: new Error(`Timeout after ${d3.timeout}ms`), networkDetails: p3, stats: f3 }));
      } });
    });
  }
  updateStatsFromPart(e, t) {
    let s2 = e.stats, i = t.stats, r2 = i.total;
    if (s2.loaded += i.loaded, r2) {
      let l2 = Math.round(e.duration / t.duration), c = Math.min(Math.round(s2.loaded / r2), l2), u2 = (l2 - c) * Math.round(s2.loaded / c);
      s2.total = s2.loaded + u2;
    } else
      s2.total = Math.max(s2.loaded, s2.total);
    let a2 = s2.loading, o = i.loading;
    a2.start ? a2.first += o.first - o.start : (a2.start = o.start, a2.first = o.first), a2.end = o.end;
  }
  resetLoader(e, t) {
    e.loader = null, this.loader === t && (self.clearTimeout(this.partLoadTimeout), this.loader = null), t.destroy();
  }
};
function Er(n12, e = null) {
  let t = e || n12, s2 = { frag: n12, part: e, responseType: "arraybuffer", url: t.url, headers: {}, rangeStart: 0, rangeEnd: 0 }, i = t.byteRangeStartOffset, r2 = t.byteRangeEndOffset;
  if (F(i) && F(r2)) {
    var a2;
    let o = i, l2 = r2;
    if (n12.sn === "initSegment" && ((a2 = n12.decryptdata) == null ? void 0 : a2.method) === "AES-128") {
      let c = r2 - i;
      c % 16 && (l2 = r2 + (16 - c % 16)), i !== 0 && (s2.resetIV = true, o = i - 16);
    }
    s2.rangeStart = o, s2.rangeEnd = l2;
  }
  return s2;
}
function yr(n12, e) {
  let t = new Error(`GAP ${n12.gap ? "tag" : "attribute"} found`), s2 = { type: B2.MEDIA_ERROR, details: A2.FRAG_GAP, fatal: false, frag: n12, error: t, networkDetails: null };
  return e && (s2.part = e), (e || n12).stats.aborted = true, new ye3(s2);
}
var ye3 = class extends Error {
  constructor(e) {
    super(e.error.message), this.data = void 0, this.data = e;
  }
};
var Bs = class {
  constructor(e, t) {
    this.subtle = void 0, this.aesIV = void 0, this.subtle = e, this.aesIV = t;
  }
  decrypt(e, t) {
    return this.subtle.decrypt({ name: "AES-CBC", iv: this.aesIV }, t, e);
  }
};
var $s = class {
  constructor(e, t) {
    this.subtle = void 0, this.key = void 0, this.subtle = e, this.key = t;
  }
  expandKey() {
    return this.subtle.importKey("raw", this.key, { name: "AES-CBC" }, false, ["encrypt", "decrypt"]);
  }
};
function xo(n12) {
  let e = n12.byteLength, t = e && new DataView(n12.buffer).getUint8(e - 1);
  return t ? Ne2(n12, 0, e - t) : n12;
}
var Gs = class {
  constructor() {
    this.rcon = [0, 1, 2, 4, 8, 16, 32, 64, 128, 27, 54], this.subMix = [new Uint32Array(256), new Uint32Array(256), new Uint32Array(256), new Uint32Array(256)], this.invSubMix = [new Uint32Array(256), new Uint32Array(256), new Uint32Array(256), new Uint32Array(256)], this.sBox = new Uint32Array(256), this.invSBox = new Uint32Array(256), this.key = new Uint32Array(0), this.ksRows = 0, this.keySize = 0, this.keySchedule = void 0, this.invKeySchedule = void 0, this.initTable();
  }
  uint8ArrayToUint32Array_(e) {
    let t = new DataView(e), s2 = new Uint32Array(4);
    for (let i = 0; i < 4; i++)
      s2[i] = t.getUint32(i * 4);
    return s2;
  }
  initTable() {
    let e = this.sBox, t = this.invSBox, s2 = this.subMix, i = s2[0], r2 = s2[1], a2 = s2[2], o = s2[3], l2 = this.invSubMix, c = l2[0], h2 = l2[1], u2 = l2[2], d3 = l2[3], f3 = new Uint32Array(256), g2 = 0, p3 = 0, T2 = 0;
    for (T2 = 0; T2 < 256; T2++)
      T2 < 128 ? f3[T2] = T2 << 1 : f3[T2] = T2 << 1 ^ 283;
    for (T2 = 0; T2 < 256; T2++) {
      let E4 = p3 ^ p3 << 1 ^ p3 << 2 ^ p3 << 3 ^ p3 << 4;
      E4 = E4 >>> 8 ^ E4 & 255 ^ 99, e[g2] = E4, t[E4] = g2;
      let x3 = f3[g2], y3 = f3[x3], R = f3[y3], S2 = f3[E4] * 257 ^ E4 * 16843008;
      i[g2] = S2 << 24 | S2 >>> 8, r2[g2] = S2 << 16 | S2 >>> 16, a2[g2] = S2 << 8 | S2 >>> 24, o[g2] = S2, S2 = R * 16843009 ^ y3 * 65537 ^ x3 * 257 ^ g2 * 16843008, c[E4] = S2 << 24 | S2 >>> 8, h2[E4] = S2 << 16 | S2 >>> 16, u2[E4] = S2 << 8 | S2 >>> 24, d3[E4] = S2, g2 ? (g2 = x3 ^ f3[f3[f3[R ^ x3]]], p3 ^= f3[f3[p3]]) : g2 = p3 = 1;
    }
  }
  expandKey(e) {
    let t = this.uint8ArrayToUint32Array_(e), s2 = true, i = 0;
    for (; i < t.length && s2; )
      s2 = t[i] === this.key[i], i++;
    if (s2)
      return;
    this.key = t;
    let r2 = this.keySize = t.length;
    if (r2 !== 4 && r2 !== 6 && r2 !== 8)
      throw new Error("Invalid aes key size=" + r2);
    let a2 = this.ksRows = (r2 + 6 + 1) * 4, o, l2, c = this.keySchedule = new Uint32Array(a2), h2 = this.invKeySchedule = new Uint32Array(a2), u2 = this.sBox, d3 = this.rcon, f3 = this.invSubMix, g2 = f3[0], p3 = f3[1], T2 = f3[2], E4 = f3[3], x3, y3;
    for (o = 0; o < a2; o++) {
      if (o < r2) {
        x3 = c[o] = t[o];
        continue;
      }
      y3 = x3, o % r2 === 0 ? (y3 = y3 << 8 | y3 >>> 24, y3 = u2[y3 >>> 24] << 24 | u2[y3 >>> 16 & 255] << 16 | u2[y3 >>> 8 & 255] << 8 | u2[y3 & 255], y3 ^= d3[o / r2 | 0] << 24) : r2 > 6 && o % r2 === 4 && (y3 = u2[y3 >>> 24] << 24 | u2[y3 >>> 16 & 255] << 16 | u2[y3 >>> 8 & 255] << 8 | u2[y3 & 255]), c[o] = x3 = (c[o - r2] ^ y3) >>> 0;
    }
    for (l2 = 0; l2 < a2; l2++)
      o = a2 - l2, l2 & 3 ? y3 = c[o] : y3 = c[o - 4], l2 < 4 || o <= 4 ? h2[l2] = y3 : h2[l2] = g2[u2[y3 >>> 24]] ^ p3[u2[y3 >>> 16 & 255]] ^ T2[u2[y3 >>> 8 & 255]] ^ E4[u2[y3 & 255]], h2[l2] = h2[l2] >>> 0;
  }
  networkToHostOrderSwap(e) {
    return e << 24 | (e & 65280) << 8 | (e & 16711680) >> 8 | e >>> 24;
  }
  decrypt(e, t, s2) {
    let i = this.keySize + 6, r2 = this.invKeySchedule, a2 = this.invSBox, o = this.invSubMix, l2 = o[0], c = o[1], h2 = o[2], u2 = o[3], d3 = this.uint8ArrayToUint32Array_(s2), f3 = d3[0], g2 = d3[1], p3 = d3[2], T2 = d3[3], E4 = new Int32Array(e), x3 = new Int32Array(E4.length), y3, R, S2, b, L, C2, _, I, w, K, P2, G2, $3, U3, Y3 = this.networkToHostOrderSwap;
    for (; t < E4.length; ) {
      for (w = Y3(E4[t]), K = Y3(E4[t + 1]), P2 = Y3(E4[t + 2]), G2 = Y3(E4[t + 3]), L = w ^ r2[0], C2 = G2 ^ r2[1], _ = P2 ^ r2[2], I = K ^ r2[3], $3 = 4, U3 = 1; U3 < i; U3++)
        y3 = l2[L >>> 24] ^ c[C2 >> 16 & 255] ^ h2[_ >> 8 & 255] ^ u2[I & 255] ^ r2[$3], R = l2[C2 >>> 24] ^ c[_ >> 16 & 255] ^ h2[I >> 8 & 255] ^ u2[L & 255] ^ r2[$3 + 1], S2 = l2[_ >>> 24] ^ c[I >> 16 & 255] ^ h2[L >> 8 & 255] ^ u2[C2 & 255] ^ r2[$3 + 2], b = l2[I >>> 24] ^ c[L >> 16 & 255] ^ h2[C2 >> 8 & 255] ^ u2[_ & 255] ^ r2[$3 + 3], L = y3, C2 = R, _ = S2, I = b, $3 = $3 + 4;
      y3 = a2[L >>> 24] << 24 ^ a2[C2 >> 16 & 255] << 16 ^ a2[_ >> 8 & 255] << 8 ^ a2[I & 255] ^ r2[$3], R = a2[C2 >>> 24] << 24 ^ a2[_ >> 16 & 255] << 16 ^ a2[I >> 8 & 255] << 8 ^ a2[L & 255] ^ r2[$3 + 1], S2 = a2[_ >>> 24] << 24 ^ a2[I >> 16 & 255] << 16 ^ a2[L >> 8 & 255] << 8 ^ a2[C2 & 255] ^ r2[$3 + 2], b = a2[I >>> 24] << 24 ^ a2[L >> 16 & 255] << 16 ^ a2[C2 >> 8 & 255] << 8 ^ a2[_ & 255] ^ r2[$3 + 3], x3[t] = Y3(y3 ^ f3), x3[t + 1] = Y3(b ^ g2), x3[t + 2] = Y3(S2 ^ p3), x3[t + 3] = Y3(R ^ T2), f3 = w, g2 = K, p3 = P2, T2 = G2, t = t + 4;
    }
    return x3.buffer;
  }
};
var So = 16;
var ot = class {
  constructor(e, { removePKCS7Padding: t = true } = {}) {
    if (this.logEnabled = true, this.removePKCS7Padding = void 0, this.subtle = null, this.softwareDecrypter = null, this.key = null, this.fastAesKey = null, this.remainderData = null, this.currentIV = null, this.currentResult = null, this.useSoftware = void 0, this.useSoftware = e.enableSoftwareAES, this.removePKCS7Padding = t, t)
      try {
        let s2 = self.crypto;
        s2 && (this.subtle = s2.subtle || s2.webkitSubtle);
      } catch {
      }
    this.subtle === null && (this.useSoftware = true);
  }
  destroy() {
    this.subtle = null, this.softwareDecrypter = null, this.key = null, this.fastAesKey = null, this.remainderData = null, this.currentIV = null, this.currentResult = null;
  }
  isSync() {
    return this.useSoftware;
  }
  flush() {
    let { currentResult: e, remainderData: t } = this;
    if (!e || t)
      return this.reset(), null;
    let s2 = new Uint8Array(e);
    return this.reset(), this.removePKCS7Padding ? xo(s2) : s2;
  }
  reset() {
    this.currentResult = null, this.currentIV = null, this.remainderData = null, this.softwareDecrypter && (this.softwareDecrypter = null);
  }
  decrypt(e, t, s2) {
    return this.useSoftware ? new Promise((i, r2) => {
      this.softwareDecrypt(new Uint8Array(e), t, s2);
      let a2 = this.flush();
      a2 ? i(a2.buffer) : r2(new Error("[softwareDecrypt] Failed to decrypt data"));
    }) : this.webCryptoDecrypt(new Uint8Array(e), t, s2);
  }
  softwareDecrypt(e, t, s2) {
    let { currentIV: i, currentResult: r2, remainderData: a2 } = this;
    this.logOnce("JS AES decrypt"), a2 && (e = pe3(a2, e), this.remainderData = null);
    let o = this.getValidChunk(e);
    if (!o.length)
      return null;
    i && (s2 = i);
    let l2 = this.softwareDecrypter;
    l2 || (l2 = this.softwareDecrypter = new Gs()), l2.expandKey(t);
    let c = r2;
    return this.currentResult = l2.decrypt(o.buffer, 0, s2), this.currentIV = Ne2(o, -16).buffer, c || null;
  }
  webCryptoDecrypt(e, t, s2) {
    let i = this.subtle;
    return (this.key !== t || !this.fastAesKey) && (this.key = t, this.fastAesKey = new $s(i, t)), this.fastAesKey.expandKey().then((r2) => i ? (this.logOnce("WebCrypto AES decrypt"), new Bs(i, new Uint8Array(s2)).decrypt(e.buffer, r2)) : Promise.reject(new Error("web crypto not initialized"))).catch((r2) => (v2.warn(`[decrypter]: WebCrypto Error, disable WebCrypto API, ${r2.name}: ${r2.message}`), this.onWebCryptoError(e, t, s2)));
  }
  onWebCryptoError(e, t, s2) {
    this.useSoftware = true, this.logEnabled = true, this.softwareDecrypt(e, t, s2);
    let i = this.flush();
    if (i)
      return i.buffer;
    throw new Error("WebCrypto and softwareDecrypt: failed to decrypt data");
  }
  getValidChunk(e) {
    let t = e, s2 = e.length - e.length % So;
    return s2 !== e.length && (t = Ne2(e, 0, s2), this.remainderData = Ne2(e, s2)), t;
  }
  logOnce(e) {
    this.logEnabled && (v2.log(`[decrypter]: ${e}`), this.logEnabled = false);
  }
};
var vo = { toString: function(n12) {
  let e = "", t = n12.length;
  for (let s2 = 0; s2 < t; s2++)
    e += `[${n12.start(s2).toFixed(3)}-${n12.end(s2).toFixed(3)}]`;
  return e;
} };
var D = { STOPPED: "STOPPED", IDLE: "IDLE", KEY_LOADING: "KEY_LOADING", FRAG_LOADING: "FRAG_LOADING", FRAG_LOADING_WAITING_RETRY: "FRAG_LOADING_WAITING_RETRY", WAITING_TRACK: "WAITING_TRACK", PARSING: "PARSING", PARSED: "PARSED", ENDED: "ENDED", ERROR: "ERROR", WAITING_INIT_PTS: "WAITING_INIT_PTS", WAITING_LEVEL: "WAITING_LEVEL" };
var lt = class extends Os {
  constructor(e, t, s2, i, r2) {
    super(), this.hls = void 0, this.fragPrevious = null, this.fragCurrent = null, this.fragmentTracker = void 0, this.transmuxer = null, this._state = D.STOPPED, this.playlistType = void 0, this.media = null, this.mediaBuffer = null, this.config = void 0, this.bitrateTest = false, this.lastCurrentTime = 0, this.nextLoadPosition = 0, this.startPosition = 0, this.startTimeOffset = null, this.loadedmetadata = false, this.retryDate = 0, this.levels = null, this.fragmentLoader = void 0, this.keyLoader = void 0, this.levelLastLoaded = null, this.startFragRequested = false, this.decrypter = void 0, this.initPTS = [], this.onvseeking = null, this.onvended = null, this.logPrefix = "", this.log = void 0, this.warn = void 0, this.playlistType = r2, this.logPrefix = i, this.log = v2.log.bind(v2, `${i}:`), this.warn = v2.warn.bind(v2, `${i}:`), this.hls = e, this.fragmentLoader = new Us(e.config), this.keyLoader = s2, this.fragmentTracker = t, this.config = e.config, this.decrypter = new ot(e.config), e.on(m2.MANIFEST_LOADED, this.onManifestLoaded, this);
  }
  doTick() {
    this.onTickEnd();
  }
  onTickEnd() {
  }
  startLoad(e) {
  }
  stopLoad() {
    this.fragmentLoader.abort(), this.keyLoader.abort(this.playlistType);
    let e = this.fragCurrent;
    e != null && e.loader && (e.abortRequests(), this.fragmentTracker.removeFragment(e)), this.resetTransmuxer(), this.fragCurrent = null, this.fragPrevious = null, this.clearInterval(), this.clearNextTick(), this.state = D.STOPPED;
  }
  _streamEnded(e, t) {
    if (t.live || e.nextStart || !e.end || !this.media)
      return false;
    let s2 = t.partList;
    if (s2 != null && s2.length) {
      let r2 = s2[s2.length - 1];
      return Q.isBuffered(this.media, r2.start + r2.duration / 2);
    }
    let i = t.fragments[t.fragments.length - 1].type;
    return this.fragmentTracker.isEndListAppended(i);
  }
  getLevelDetails() {
    if (this.levels && this.levelLastLoaded !== null) {
      var e;
      return (e = this.levelLastLoaded) == null ? void 0 : e.details;
    }
  }
  onMediaAttached(e, t) {
    let s2 = this.media = this.mediaBuffer = t.media;
    this.onvseeking = this.onMediaSeeking.bind(this), this.onvended = this.onMediaEnded.bind(this), s2.addEventListener("seeking", this.onvseeking), s2.addEventListener("ended", this.onvended);
    let i = this.config;
    this.levels && i.autoStartLoad && this.state === D.STOPPED && this.startLoad(i.startPosition);
  }
  onMediaDetaching() {
    let e = this.media;
    e != null && e.ended && (this.log("MSE detaching and video ended, reset startPosition"), this.startPosition = this.lastCurrentTime = 0), e && this.onvseeking && this.onvended && (e.removeEventListener("seeking", this.onvseeking), e.removeEventListener("ended", this.onvended), this.onvseeking = this.onvended = null), this.keyLoader && this.keyLoader.detach(), this.media = this.mediaBuffer = null, this.loadedmetadata = false, this.fragmentTracker.removeAllFragments(), this.stopLoad();
  }
  onMediaSeeking() {
    let { config: e, fragCurrent: t, media: s2, mediaBuffer: i, state: r2 } = this, a2 = s2 ? s2.currentTime : 0, o = Q.bufferInfo(i || s2, a2, e.maxBufferHole);
    if (this.log(`media seeking to ${F(a2) ? a2.toFixed(3) : a2}, state: ${r2}`), this.state === D.ENDED)
      this.resetLoadingState();
    else if (t) {
      let l2 = e.maxFragLookUpTolerance, c = t.start - l2, h2 = t.start + t.duration + l2;
      if (!o.len || h2 < o.start || c > o.end) {
        let u2 = a2 > h2;
        (a2 < c || u2) && (u2 && t.loader && (this.log("seeking outside of buffer while fragment load in progress, cancel fragment load"), t.abortRequests(), this.resetLoadingState()), this.fragPrevious = null);
      }
    }
    s2 && (this.fragmentTracker.removeFragmentsInRange(a2, 1 / 0, this.playlistType, true), this.lastCurrentTime = a2), !this.loadedmetadata && !o.len && (this.nextLoadPosition = this.startPosition = a2), this.tickImmediate();
  }
  onMediaEnded() {
    this.startPosition = this.lastCurrentTime = 0;
  }
  onManifestLoaded(e, t) {
    this.startTimeOffset = t.startTimeOffset, this.initPTS = [];
  }
  onHandlerDestroying() {
    this.hls.off(m2.MANIFEST_LOADED, this.onManifestLoaded, this), this.stopLoad(), super.onHandlerDestroying(), this.hls = null;
  }
  onHandlerDestroyed() {
    this.state = D.STOPPED, this.fragmentLoader && this.fragmentLoader.destroy(), this.keyLoader && this.keyLoader.destroy(), this.decrypter && this.decrypter.destroy(), this.hls = this.log = this.warn = this.decrypter = this.keyLoader = this.fragmentLoader = this.fragmentTracker = null, super.onHandlerDestroyed();
  }
  loadFragment(e, t, s2) {
    this._loadFragForPlayback(e, t, s2);
  }
  _loadFragForPlayback(e, t, s2) {
    let i = (r2) => {
      if (this.fragContextChanged(e)) {
        this.warn(`Fragment ${e.sn}${r2.part ? " p: " + r2.part.index : ""} of level ${e.level} was dropped during download.`), this.fragmentTracker.removeFragment(e);
        return;
      }
      e.stats.chunkCount++, this._handleFragmentLoadProgress(r2);
    };
    this._doFragLoad(e, t, s2, i).then((r2) => {
      if (!r2)
        return;
      let a2 = this.state;
      if (this.fragContextChanged(e)) {
        (a2 === D.FRAG_LOADING || !this.fragCurrent && a2 === D.PARSING) && (this.fragmentTracker.removeFragment(e), this.state = D.IDLE);
        return;
      }
      "payload" in r2 && (this.log(`Loaded fragment ${e.sn} of level ${e.level}`), this.hls.trigger(m2.FRAG_LOADED, r2)), this._handleFragmentLoadComplete(r2);
    }).catch((r2) => {
      this.state === D.STOPPED || this.state === D.ERROR || (this.warn(r2), this.resetFragmentLoading(e));
    });
  }
  clearTrackerIfNeeded(e) {
    var t;
    let { fragmentTracker: s2 } = this;
    if (s2.getState(e) === oe2.APPENDING) {
      let r2 = e.type, a2 = this.getFwdBufferInfo(this.mediaBuffer, r2), o = Math.max(e.duration, a2 ? a2.len : this.config.maxBufferLength);
      this.reduceMaxBufferLength(o) && s2.removeFragment(e);
    } else
      ((t = this.mediaBuffer) == null ? void 0 : t.buffered.length) === 0 ? s2.removeAllFragments() : s2.hasParts(e.type) && (s2.detectPartialFragments({ frag: e, part: null, stats: e.stats, id: e.type }), s2.getState(e) === oe2.PARTIAL && s2.removeFragment(e));
  }
  checkLiveUpdate(e) {
    if (e.updated && !e.live) {
      let t = e.fragments[e.fragments.length - 1];
      this.fragmentTracker.detectPartialFragments({ frag: t, part: null, stats: t.stats, id: t.type });
    }
    e.fragments[0] || (e.deltaUpdateFailed = true);
  }
  flushMainBuffer(e, t, s2 = null) {
    if (!(e - t))
      return;
    let i = { startOffset: e, endOffset: t, type: s2 };
    this.hls.trigger(m2.BUFFER_FLUSHING, i);
  }
  _loadInitSegment(e, t) {
    this._doFragLoad(e, t).then((s2) => {
      if (!s2 || this.fragContextChanged(e) || !this.levels)
        throw new Error("init load aborted");
      return s2;
    }).then((s2) => {
      let { hls: i } = this, { payload: r2 } = s2, a2 = e.decryptdata;
      if (r2 && r2.byteLength > 0 && a2 != null && a2.key && a2.iv && a2.method === "AES-128") {
        let o = self.performance.now();
        return this.decrypter.decrypt(new Uint8Array(r2), a2.key.buffer, a2.iv.buffer).catch((l2) => {
          throw i.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_DECRYPT_ERROR, fatal: false, error: l2, reason: l2.message, frag: e }), l2;
        }).then((l2) => {
          let c = self.performance.now();
          return i.trigger(m2.FRAG_DECRYPTED, { frag: e, payload: l2, stats: { tstart: o, tdecrypt: c } }), s2.payload = l2, this.completeInitSegmentLoad(s2);
        });
      }
      return this.completeInitSegmentLoad(s2);
    }).catch((s2) => {
      this.state === D.STOPPED || this.state === D.ERROR || (this.warn(s2), this.resetFragmentLoading(e));
    });
  }
  completeInitSegmentLoad(e) {
    let { levels: t } = this;
    if (!t)
      throw new Error("init load aborted, missing levels");
    let s2 = e.frag.stats;
    this.state = D.IDLE, e.frag.data = new Uint8Array(e.payload), s2.parsing.start = s2.buffering.start = self.performance.now(), s2.parsing.end = s2.buffering.end = self.performance.now(), this.tick();
  }
  fragContextChanged(e) {
    let { fragCurrent: t } = this;
    return !e || !t || e.sn !== t.sn || e.level !== t.level;
  }
  fragBufferedComplete(e, t) {
    var s2, i, r2, a2;
    let o = this.mediaBuffer ? this.mediaBuffer : this.media;
    if (this.log(`Buffered ${e.type} sn: ${e.sn}${t ? " part: " + t.index : ""} of ${this.playlistType === N2.MAIN ? "level" : "track"} ${e.level} (frag:[${((s2 = e.startPTS) != null ? s2 : NaN).toFixed(3)}-${((i = e.endPTS) != null ? i : NaN).toFixed(3)}] > buffer:${o ? vo.toString(Q.getBuffered(o)) : "(detached)"})`), e.sn !== "initSegment") {
      var l2;
      if (e.type !== N2.SUBTITLE) {
        let h2 = e.elementaryStreams;
        if (!Object.keys(h2).some((u2) => !!h2[u2])) {
          this.state = D.IDLE;
          return;
        }
      }
      let c = (l2 = this.levels) == null ? void 0 : l2[e.level];
      c != null && c.fragmentError && (this.log(`Resetting level fragment error count of ${c.fragmentError} on frag buffered`), c.fragmentError = 0);
    }
    this.state = D.IDLE, o && (!this.loadedmetadata && e.type == N2.MAIN && o.buffered.length && ((r2 = this.fragCurrent) == null ? void 0 : r2.sn) === ((a2 = this.fragPrevious) == null ? void 0 : a2.sn) && (this.loadedmetadata = true, this.seekToStartPos()), this.tick());
  }
  seekToStartPos() {
  }
  _handleFragmentLoadComplete(e) {
    let { transmuxer: t } = this;
    if (!t)
      return;
    let { frag: s2, part: i, partsLoaded: r2 } = e, a2 = !r2 || r2.length === 0 || r2.some((l2) => !l2), o = new at(s2.level, s2.sn, s2.stats.chunkCount + 1, 0, i ? i.index : -1, !a2);
    t.flush(o);
  }
  _handleFragmentLoadProgress(e) {
  }
  _doFragLoad(e, t, s2 = null, i) {
    var r2;
    let a2 = t?.details;
    if (!this.levels || !a2)
      throw new Error(`frag load aborted, missing level${a2 ? "" : " detail"}s`);
    let o = null;
    if (e.encrypted && !((r2 = e.decryptdata) != null && r2.key) ? (this.log(`Loading key for ${e.sn} of [${a2.startSN}-${a2.endSN}], ${this.logPrefix === "[stream-controller]" ? "level" : "track"} ${e.level}`), this.state = D.KEY_LOADING, this.fragCurrent = e, o = this.keyLoader.load(e).then((h2) => {
      if (!this.fragContextChanged(h2.frag))
        return this.hls.trigger(m2.KEY_LOADED, h2), this.state === D.KEY_LOADING && (this.state = D.IDLE), h2;
    }), this.hls.trigger(m2.KEY_LOADING, { frag: e }), this.fragCurrent === null && (o = Promise.reject(new Error("frag load aborted, context changed in KEY_LOADING")))) : !e.encrypted && a2.encryptedFragments.length && this.keyLoader.loadClear(e, a2.encryptedFragments), s2 = Math.max(e.start, s2 || 0), this.config.lowLatencyMode && e.sn !== "initSegment") {
      let h2 = a2.partList;
      if (h2 && i) {
        s2 > e.end && a2.fragmentHint && (e = a2.fragmentHint);
        let u2 = this.getNextPart(h2, e, s2);
        if (u2 > -1) {
          let d3 = h2[u2];
          this.log(`Loading part sn: ${e.sn} p: ${d3.index} cc: ${e.cc} of playlist [${a2.startSN}-${a2.endSN}] parts [0-${u2}-${h2.length - 1}] ${this.logPrefix === "[stream-controller]" ? "level" : "track"}: ${e.level}, target: ${parseFloat(s2.toFixed(3))}`), this.nextLoadPosition = d3.start + d3.duration, this.state = D.FRAG_LOADING;
          let f3;
          return o ? f3 = o.then((g2) => !g2 || this.fragContextChanged(g2.frag) ? null : this.doFragPartsLoad(e, d3, t, i)).catch((g2) => this.handleFragLoadError(g2)) : f3 = this.doFragPartsLoad(e, d3, t, i).catch((g2) => this.handleFragLoadError(g2)), this.hls.trigger(m2.FRAG_LOADING, { frag: e, part: d3, targetBufferTime: s2 }), this.fragCurrent === null ? Promise.reject(new Error("frag load aborted, context changed in FRAG_LOADING parts")) : f3;
        } else if (!e.url || this.loadedEndOfParts(h2, s2))
          return Promise.resolve(null);
      }
    }
    this.log(`Loading fragment ${e.sn} cc: ${e.cc} ${a2 ? "of [" + a2.startSN + "-" + a2.endSN + "] " : ""}${this.logPrefix === "[stream-controller]" ? "level" : "track"}: ${e.level}, target: ${parseFloat(s2.toFixed(3))}`), F(e.sn) && !this.bitrateTest && (this.nextLoadPosition = e.start + e.duration), this.state = D.FRAG_LOADING;
    let l2 = this.config.progressive, c;
    return l2 && o ? c = o.then((h2) => !h2 || this.fragContextChanged(h2?.frag) ? null : this.fragmentLoader.load(e, i)).catch((h2) => this.handleFragLoadError(h2)) : c = Promise.all([this.fragmentLoader.load(e, l2 ? i : void 0), o]).then(([h2]) => (!l2 && h2 && i && i(h2), h2)).catch((h2) => this.handleFragLoadError(h2)), this.hls.trigger(m2.FRAG_LOADING, { frag: e, targetBufferTime: s2 }), this.fragCurrent === null ? Promise.reject(new Error("frag load aborted, context changed in FRAG_LOADING")) : c;
  }
  doFragPartsLoad(e, t, s2, i) {
    return new Promise((r2, a2) => {
      var o;
      let l2 = [], c = (o = s2.details) == null ? void 0 : o.partList, h2 = (u2) => {
        this.fragmentLoader.loadPart(e, u2, i).then((d3) => {
          l2[u2.index] = d3;
          let f3 = d3.part;
          this.hls.trigger(m2.FRAG_LOADED, d3);
          let g2 = ur(s2, e.sn, u2.index + 1) || nn(c, e.sn, u2.index + 1);
          if (g2)
            h2(g2);
          else
            return r2({ frag: e, part: f3, partsLoaded: l2 });
        }).catch(a2);
      };
      h2(t);
    });
  }
  handleFragLoadError(e) {
    if ("data" in e) {
      let t = e.data;
      e.data && t.details === A2.INTERNAL_ABORTED ? this.handleFragLoadAborted(t.frag, t.part) : this.hls.trigger(m2.ERROR, t);
    } else
      this.hls.trigger(m2.ERROR, { type: B2.OTHER_ERROR, details: A2.INTERNAL_EXCEPTION, err: e, error: e, fatal: true });
    return null;
  }
  _handleTransmuxerFlush(e) {
    let t = this.getCurrentContext(e);
    if (!t || this.state !== D.PARSING) {
      !this.fragCurrent && this.state !== D.STOPPED && this.state !== D.ERROR && (this.state = D.IDLE);
      return;
    }
    let { frag: s2, part: i, level: r2 } = t, a2 = self.performance.now();
    s2.stats.parsing.end = a2, i && (i.stats.parsing.end = a2), this.updateLevelTiming(s2, i, r2, e.partial);
  }
  getCurrentContext(e) {
    let { levels: t, fragCurrent: s2 } = this, { level: i, sn: r2, part: a2 } = e;
    if (!(t != null && t[i]))
      return this.warn(`Levels object was unset while buffering fragment ${r2} of level ${i}. The current chunk will not be buffered.`), null;
    let o = t[i], l2 = a2 > -1 ? ur(o, r2, a2) : null, c = l2 ? l2.fragment : Za(o, r2, s2);
    return c ? (s2 && s2 !== c && (c.stats = s2.stats), { frag: c, part: l2, level: o }) : null;
  }
  bufferFragmentData(e, t, s2, i, r2) {
    var a2;
    if (!e || this.state !== D.PARSING)
      return;
    let { data1: o, data2: l2 } = e, c = o;
    if (o && l2 && (c = pe3(o, l2)), !((a2 = c) != null && a2.length))
      return;
    let h2 = { type: e.type, frag: t, part: s2, chunkMeta: i, parent: t.type, data: c };
    if (this.hls.trigger(m2.BUFFER_APPENDING, h2), e.dropped && e.independent && !s2) {
      if (r2)
        return;
      this.flushBufferGap(t);
    }
  }
  flushBufferGap(e) {
    let t = this.media;
    if (!t)
      return;
    if (!Q.isBuffered(t, t.currentTime)) {
      this.flushMainBuffer(0, e.start);
      return;
    }
    let s2 = t.currentTime, i = Q.bufferInfo(t, s2, 0), r2 = e.duration, a2 = Math.min(this.config.maxFragLookUpTolerance * 2, r2 * 0.25), o = Math.max(Math.min(e.start - a2, i.end - a2), s2 + a2);
    e.start - o > a2 && this.flushMainBuffer(o, e.start);
  }
  getFwdBufferInfo(e, t) {
    let s2 = this.getLoadPosition();
    return F(s2) ? this.getFwdBufferInfoAtPos(e, s2, t) : null;
  }
  getFwdBufferInfoAtPos(e, t, s2) {
    let { config: { maxBufferHole: i } } = this, r2 = Q.bufferInfo(e, t, i);
    if (r2.len === 0 && r2.nextStart !== void 0) {
      let a2 = this.fragmentTracker.getBufferedFrag(t, s2);
      if (a2 && r2.nextStart < a2.end)
        return Q.bufferInfo(e, t, Math.max(r2.nextStart, i));
    }
    return r2;
  }
  getMaxBufferLength(e) {
    let { config: t } = this, s2;
    return e ? s2 = Math.max(8 * t.maxBufferSize / e, t.maxBufferLength) : s2 = t.maxBufferLength, Math.min(s2, t.maxMaxBufferLength);
  }
  reduceMaxBufferLength(e) {
    let t = this.config, s2 = e || t.maxBufferLength;
    return t.maxMaxBufferLength >= s2 ? (t.maxMaxBufferLength /= 2, this.warn(`Reduce max buffer length to ${t.maxMaxBufferLength}s`), true) : false;
  }
  getAppendedFrag(e, t = N2.MAIN) {
    let s2 = this.fragmentTracker.getAppendedFrag(e, N2.MAIN);
    return s2 && "fragment" in s2 ? s2.fragment : s2;
  }
  getNextFragment(e, t) {
    let s2 = t.fragments, i = s2.length;
    if (!i)
      return null;
    let { config: r2 } = this, a2 = s2[0].start, o;
    if (t.live) {
      let l2 = r2.initialLiveManifestSize;
      if (i < l2)
        return this.warn(`Not enough fragments to start playback (have: ${i}, need: ${l2})`), null;
      (!t.PTSKnown && !this.startFragRequested && this.startPosition === -1 || e < a2) && (o = this.getInitialLiveFragment(t, s2), this.startPosition = this.nextLoadPosition = o ? this.hls.liveSyncPosition || o.start : e);
    } else
      e <= a2 && (o = s2[0]);
    if (!o) {
      let l2 = r2.lowLatencyMode ? t.partEnd : t.fragmentEnd;
      o = this.getFragmentAtPosition(e, l2, t);
    }
    return this.mapToInitFragWhenRequired(o);
  }
  isLoopLoading(e, t) {
    let s2 = this.fragmentTracker.getState(e);
    return (s2 === oe2.OK || s2 === oe2.PARTIAL && !!e.gap) && this.nextLoadPosition > t;
  }
  getNextFragmentLoopLoading(e, t, s2, i, r2) {
    let a2 = e.gap, o = this.getNextFragment(this.nextLoadPosition, t);
    if (o === null)
      return o;
    if (e = o, a2 && e && !e.gap && s2.nextStart) {
      let l2 = this.getFwdBufferInfoAtPos(this.mediaBuffer ? this.mediaBuffer : this.media, s2.nextStart, i);
      if (l2 !== null && s2.len + l2.len >= r2)
        return this.log(`buffer full after gaps in "${i}" playlist starting at sn: ${e.sn}`), null;
    }
    return e;
  }
  mapToInitFragWhenRequired(e) {
    return e != null && e.initSegment && !(e != null && e.initSegment.data) && !this.bitrateTest ? e.initSegment : e;
  }
  getNextPart(e, t, s2) {
    let i = -1, r2 = false, a2 = true;
    for (let o = 0, l2 = e.length; o < l2; o++) {
      let c = e[o];
      if (a2 = a2 && !c.independent, i > -1 && s2 < c.start)
        break;
      let h2 = c.loaded;
      h2 ? i = -1 : (r2 || c.independent || a2) && c.fragment === t && (i = o), r2 = h2;
    }
    return i;
  }
  loadedEndOfParts(e, t) {
    let s2 = e[e.length - 1];
    return s2 && t > s2.start && s2.loaded;
  }
  getInitialLiveFragment(e, t) {
    let s2 = this.fragPrevious, i = null;
    if (s2) {
      if (e.hasProgramDateTime && (this.log(`Live playlist, switching playlist, load frag with same PDT: ${s2.programDateTime}`), i = to(t, s2.endProgramDateTime, this.config.maxFragLookUpTolerance)), !i) {
        let r2 = s2.sn + 1;
        if (r2 >= e.startSN && r2 <= e.endSN) {
          let a2 = t[r2 - e.startSN];
          s2.cc === a2.cc && (i = a2, this.log(`Live playlist, switching playlist, load frag with next SN: ${i.sn}`));
        }
        i || (i = io(t, s2.cc), i && this.log(`Live playlist, switching playlist, load frag with same CC: ${i.sn}`));
      }
    } else {
      let r2 = this.hls.liveSyncPosition;
      r2 !== null && (i = this.getFragmentAtPosition(r2, this.bitrateTest ? e.fragmentEnd : e.edge, e));
    }
    return i;
  }
  getFragmentAtPosition(e, t, s2) {
    let { config: i } = this, { fragPrevious: r2 } = this, { fragments: a2, endSN: o } = s2, { fragmentHint: l2 } = s2, c = i.maxFragLookUpTolerance, h2 = s2.partList, u2 = !!(i.lowLatencyMode && h2 != null && h2.length && l2);
    u2 && l2 && !this.bitrateTest && (a2 = a2.concat(l2), o = l2.sn);
    let d3;
    if (e < t) {
      let f3 = e > t - c ? 0 : c;
      d3 = Nt(r2, a2, e, f3);
    } else
      d3 = a2[a2.length - 1];
    if (d3) {
      let f3 = d3.sn - s2.startSN, g2 = this.fragmentTracker.getState(d3);
      if ((g2 === oe2.OK || g2 === oe2.PARTIAL && d3.gap) && (r2 = d3), r2 && d3.sn === r2.sn && (!u2 || h2[0].fragment.sn > d3.sn) && r2 && d3.level === r2.level) {
        let T2 = a2[f3 + 1];
        d3.sn < o && this.fragmentTracker.getState(T2) !== oe2.OK ? d3 = T2 : d3 = null;
      }
    }
    return d3;
  }
  synchronizeToLiveEdge(e) {
    let { config: t, media: s2 } = this;
    if (!s2)
      return;
    let i = this.hls.liveSyncPosition, r2 = s2.currentTime, a2 = e.fragments[0].start, o = e.edge, l2 = r2 >= a2 - t.maxFragLookUpTolerance && r2 <= o;
    if (i !== null && s2.duration > i && (r2 < i || !l2)) {
      let c = t.liveMaxLatencyDuration !== void 0 ? t.liveMaxLatencyDuration : t.liveMaxLatencyDurationCount * e.targetduration;
      (!l2 && s2.readyState < 4 || r2 < o - c) && (this.loadedmetadata || (this.nextLoadPosition = i), s2.readyState && (this.warn(`Playback: ${r2.toFixed(3)} is located too far from the end of live sliding playlist: ${o}, reset currentTime to : ${i.toFixed(3)}`), s2.currentTime = i));
    }
  }
  alignPlaylists(e, t, s2) {
    let i = e.fragments.length;
    if (!i)
      return this.warn("No fragments in live playlist"), 0;
    let r2 = e.fragments[0].start, a2 = !t, o = e.alignedSliding && F(r2);
    if (a2 || !o && !r2) {
      let { fragPrevious: l2 } = this;
      Eo(l2, s2, e);
      let c = e.fragments[0].start;
      return this.log(`Live playlist sliding: ${c.toFixed(2)} start-sn: ${t ? t.startSN : "na"}->${e.startSN} prev-sn: ${l2 ? l2.sn : "na"} fragments: ${i}`), c;
    }
    return r2;
  }
  waitForCdnTuneIn(e) {
    return e.live && e.canBlockReload && e.partTarget && e.tuneInGoal > Math.max(e.partHoldBack, e.partTarget * 3);
  }
  setStartPosition(e, t) {
    let s2 = this.startPosition;
    if (s2 < t && (s2 = -1), s2 === -1 || this.lastCurrentTime === -1) {
      let i = this.startTimeOffset !== null, r2 = i ? this.startTimeOffset : e.startTimeOffset;
      r2 !== null && F(r2) ? (s2 = t + r2, r2 < 0 && (s2 += e.totalduration), s2 = Math.min(Math.max(t, s2), t + e.totalduration), this.log(`Start time offset ${r2} found in ${i ? "multivariant" : "media"} playlist, adjust startPosition to ${s2}`), this.startPosition = s2) : e.live ? s2 = this.hls.liveSyncPosition || t : this.startPosition = s2 = 0, this.lastCurrentTime = s2;
    }
    this.nextLoadPosition = s2;
  }
  getLoadPosition() {
    let { media: e } = this, t = 0;
    return this.loadedmetadata && e ? t = e.currentTime : this.nextLoadPosition && (t = this.nextLoadPosition), t;
  }
  handleFragLoadAborted(e, t) {
    this.transmuxer && e.sn !== "initSegment" && e.stats.aborted && (this.warn(`Fragment ${e.sn}${t ? " part " + t.index : ""} of level ${e.level} was aborted`), this.resetFragmentLoading(e));
  }
  resetFragmentLoading(e) {
    (!this.fragCurrent || !this.fragContextChanged(e) && this.state !== D.FRAG_LOADING_WAITING_RETRY) && (this.state = D.IDLE);
  }
  onFragmentOrKeyLoadError(e, t) {
    if (t.chunkMeta && !t.frag) {
      let h2 = this.getCurrentContext(t.chunkMeta);
      h2 && (t.frag = h2.frag);
    }
    let s2 = t.frag;
    if (!s2 || s2.type !== e || !this.levels)
      return;
    if (this.fragContextChanged(s2)) {
      var i;
      this.warn(`Frag load error must match current frag to retry ${s2.url} > ${(i = this.fragCurrent) == null ? void 0 : i.url}`);
      return;
    }
    let r2 = t.details === A2.FRAG_GAP;
    r2 && this.fragmentTracker.fragBuffered(s2, true);
    let a2 = t.errorAction, { action: o, retryCount: l2 = 0, retryConfig: c } = a2 || {};
    if (a2 && o === ae3.RetryRequest && c) {
      this.resetStartWhenNotLoaded(this.levelLastLoaded);
      let h2 = Fi(c, l2);
      this.warn(`Fragment ${s2.sn} of ${e} ${s2.level} errored with ${t.details}, retrying loading ${l2 + 1}/${c.maxNumRetry} in ${h2}ms`), a2.resolved = true, this.retryDate = self.performance.now() + h2, this.state = D.FRAG_LOADING_WAITING_RETRY;
    } else if (c && a2)
      if (this.resetFragmentErrors(e), l2 < c.maxNumRetry)
        !r2 && o !== ae3.RemoveAlternatePermanently && (a2.resolved = true);
      else {
        v2.warn(`${t.details} reached or exceeded max retry (${l2})`);
        return;
      }
    else
      a2?.action === ae3.SendAlternateToPenaltyBox ? this.state = D.WAITING_LEVEL : this.state = D.ERROR;
    this.tickImmediate();
  }
  reduceLengthAndFlushBuffer(e) {
    if (this.state === D.PARSING || this.state === D.PARSED) {
      let t = e.parent, s2 = this.getFwdBufferInfo(this.mediaBuffer, t), i = s2 && s2.len > 0.5;
      i && this.reduceMaxBufferLength(s2.len);
      let r2 = !i;
      return r2 && this.warn(`Buffer full error while media.currentTime is not buffered, flush ${t} buffer`), e.frag && (this.fragmentTracker.removeFragment(e.frag), this.nextLoadPosition = e.frag.start), this.resetLoadingState(), r2;
    }
    return false;
  }
  resetFragmentErrors(e) {
    e === N2.AUDIO && (this.fragCurrent = null), this.loadedmetadata || (this.startFragRequested = false), this.state !== D.STOPPED && (this.state = D.IDLE);
  }
  afterBufferFlushed(e, t, s2) {
    if (!e)
      return;
    let i = Q.getBuffered(e);
    this.fragmentTracker.detectEvictedFragments(t, i, s2), this.state === D.ENDED && this.resetLoadingState();
  }
  resetLoadingState() {
    this.log("Reset loading state"), this.fragCurrent = null, this.fragPrevious = null, this.state = D.IDLE;
  }
  resetStartWhenNotLoaded(e) {
    if (!this.loadedmetadata) {
      this.startFragRequested = false;
      let t = e ? e.details : null;
      t != null && t.live ? (this.startPosition = -1, this.setStartPosition(t, 0), this.resetLoadingState()) : this.nextLoadPosition = this.startPosition;
    }
  }
  resetWhenMissingContext(e) {
    this.warn(`The loading context changed while buffering fragment ${e.sn} of level ${e.level}. This chunk will not be buffered.`), this.removeUnbufferedFrags(), this.resetStartWhenNotLoaded(this.levelLastLoaded), this.resetLoadingState();
  }
  removeUnbufferedFrags(e = 0) {
    this.fragmentTracker.removeFragmentsInRange(e, 1 / 0, this.playlistType, false, true);
  }
  updateLevelTiming(e, t, s2, i) {
    var r2;
    let a2 = s2.details;
    if (!a2) {
      this.warn("level.details undefined");
      return;
    }
    if (!Object.keys(e.elementaryStreams).reduce((l2, c) => {
      let h2 = e.elementaryStreams[c];
      if (h2) {
        let u2 = h2.endPTS - h2.startPTS;
        if (u2 <= 0)
          return this.warn(`Could not parse fragment ${e.sn} ${c} duration reliably (${u2})`), l2 || false;
        let d3 = i ? 0 : sn2(a2, e, h2.startPTS, h2.endPTS, h2.startDTS, h2.endDTS);
        return this.hls.trigger(m2.LEVEL_PTS_UPDATED, { details: a2, level: s2, drift: d3, type: c, frag: e, start: h2.startPTS, end: h2.endPTS }), true;
      }
      return l2;
    }, false) && ((r2 = this.transmuxer) == null ? void 0 : r2.error) === null) {
      let l2 = new Error(`Found no media in fragment ${e.sn} of level ${e.level} resetting transmuxer to fallback to playlist timing`);
      if (s2.fragmentError === 0 && (s2.fragmentError++, e.gap = true, this.fragmentTracker.removeFragment(e), this.fragmentTracker.fragBuffered(e, true)), this.warn(l2.message), this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, fatal: false, error: l2, frag: e, reason: `Found no media in msn ${e.sn} of level "${s2.url}"` }), !this.hls)
        return;
      this.resetTransmuxer();
    }
    this.state = D.PARSED, this.hls.trigger(m2.FRAG_PARSED, { frag: e, part: t });
  }
  resetTransmuxer() {
    this.transmuxer && (this.transmuxer.destroy(), this.transmuxer = null);
  }
  recoverWorkerError(e) {
    e.event === "demuxerWorker" && (this.fragmentTracker.removeAllFragments(), this.resetTransmuxer(), this.resetStartWhenNotLoaded(this.levelLastLoaded), this.resetLoadingState());
  }
  set state(e) {
    let t = this._state;
    t !== e && (this._state = e, this.log(`${t}->${e}`));
  }
  get state() {
    return this._state;
  }
};
var Bt = class {
  constructor() {
    this.chunks = [], this.dataLength = 0;
  }
  push(e) {
    this.chunks.push(e), this.dataLength += e.length;
  }
  flush() {
    let { chunks: e, dataLength: t } = this, s2;
    if (e.length)
      e.length === 1 ? s2 = e[0] : s2 = Ao(e, t);
    else
      return new Uint8Array(0);
    return this.reset(), s2;
  }
  reset() {
    this.chunks.length = 0, this.dataLength = 0;
  }
};
function Ao(n12, e) {
  let t = new Uint8Array(e), s2 = 0;
  for (let i = 0; i < n12.length; i++) {
    let r2 = n12[i];
    t.set(r2, s2), s2 += r2.length;
  }
  return t;
}
function Lo() {
  return typeof __HLS_WORKER_BUNDLE__ == "function";
}
function Ro() {
  let n12 = new self.Blob([`var exports={};var module={exports:exports};function define(f){f()};define.amd=true;(${__HLS_WORKER_BUNDLE__.toString()})(true);`], { type: "text/javascript" }), e = self.URL.createObjectURL(n12);
  return { worker: new self.Worker(e), objectURL: e };
}
function Io(n12) {
  let e = new self.URL(n12, self.location.href).href;
  return { worker: new self.Worker(e), scriptURL: e };
}
function Ae2(n12 = "", e = 9e4) {
  return { type: n12, id: -1, pid: -1, inputTimeScale: e, sequenceNumber: -1, samples: [], dropped: 0 };
}
var ct = class {
  constructor() {
    this._audioTrack = void 0, this._id3Track = void 0, this.frameIndex = 0, this.cachedData = null, this.basePTS = null, this.initPTS = null, this.lastPTS = null;
  }
  resetInitSegment(e, t, s2, i) {
    this._id3Track = { type: "id3", id: 3, pid: -1, inputTimeScale: 9e4, sequenceNumber: 0, samples: [], dropped: 0 };
  }
  resetTimeStamp(e) {
    this.initPTS = e, this.resetContiguity();
  }
  resetContiguity() {
    this.basePTS = null, this.lastPTS = null, this.frameIndex = 0;
  }
  canParse(e, t) {
    return false;
  }
  appendFrame(e, t, s2) {
  }
  demux(e, t) {
    this.cachedData && (e = pe3(this.cachedData, e), this.cachedData = null);
    let s2 = st(e, 0), i = s2 ? s2.length : 0, r2, a2 = this._audioTrack, o = this._id3Track, l2 = s2 ? ki(s2) : void 0, c = e.length;
    for ((this.basePTS === null || this.frameIndex === 0 && F(l2)) && (this.basePTS = bo(l2, t, this.initPTS), this.lastPTS = this.basePTS), this.lastPTS === null && (this.lastPTS = this.basePTS), s2 && s2.length > 0 && o.samples.push({ pts: this.lastPTS, dts: this.lastPTS, data: s2, type: xe3.audioId3, duration: Number.POSITIVE_INFINITY }); i < c; ) {
      if (this.canParse(e, i)) {
        let h2 = this.appendFrame(a2, e, i);
        h2 ? (this.frameIndex++, this.lastPTS = h2.sample.pts, i += h2.length, r2 = i) : i = c;
      } else
        oa(e, i) ? (s2 = st(e, i), o.samples.push({ pts: this.lastPTS, dts: this.lastPTS, data: s2, type: xe3.audioId3, duration: Number.POSITIVE_INFINITY }), i += s2.length, r2 = i) : i++;
      if (i === c && r2 !== c) {
        let h2 = Ne2(e, r2);
        this.cachedData ? this.cachedData = pe3(this.cachedData, h2) : this.cachedData = h2;
      }
    }
    return { audioTrack: a2, videoTrack: Ae2(), id3Track: o, textTrack: Ae2() };
  }
  demuxSampleAes(e, t, s2) {
    return Promise.reject(new Error(`[${this}] This demuxer does not support Sample-AES decryption`));
  }
  flush(e) {
    let t = this.cachedData;
    return t && (this.cachedData = null, this.demux(t, 0)), { audioTrack: this._audioTrack, videoTrack: Ae2(), id3Track: this._id3Track, textTrack: Ae2() };
  }
  destroy() {
  }
};
var bo = (n12, e, t) => {
  if (F(n12))
    return n12 * 90;
  let s2 = t ? t.baseTime * 9e4 / t.timescale : 0;
  return e * 9e4 + s2;
};
function Do(n12, e, t, s2) {
  let i, r2, a2, o, l2 = navigator.userAgent.toLowerCase(), c = s2, h2 = [96e3, 88200, 64e3, 48e3, 44100, 32e3, 24e3, 22050, 16e3, 12e3, 11025, 8e3, 7350];
  i = ((e[t + 2] & 192) >>> 6) + 1;
  let u2 = (e[t + 2] & 60) >>> 2;
  if (u2 > h2.length - 1) {
    let d3 = new Error(`invalid ADTS sampling index:${u2}`);
    n12.emit(m2.ERROR, m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, fatal: true, error: d3, reason: d3.message });
    return;
  }
  return a2 = (e[t + 2] & 1) << 2, a2 |= (e[t + 3] & 192) >>> 6, v2.log(`manifest codec:${s2}, ADTS type:${i}, samplingIndex:${u2}`), /firefox/i.test(l2) ? u2 >= 6 ? (i = 5, o = new Array(4), r2 = u2 - 3) : (i = 2, o = new Array(2), r2 = u2) : l2.indexOf("android") !== -1 ? (i = 2, o = new Array(2), r2 = u2) : (i = 5, o = new Array(4), s2 && (s2.indexOf("mp4a.40.29") !== -1 || s2.indexOf("mp4a.40.5") !== -1) || !s2 && u2 >= 6 ? r2 = u2 - 3 : ((s2 && s2.indexOf("mp4a.40.2") !== -1 && (u2 >= 6 && a2 === 1 || /vivaldi/i.test(l2)) || !s2 && a2 === 1) && (i = 2, o = new Array(2)), r2 = u2)), o[0] = i << 3, o[0] |= (u2 & 14) >> 1, o[1] |= (u2 & 1) << 7, o[1] |= a2 << 3, i === 5 && (o[1] |= (r2 & 14) >> 1, o[2] = (r2 & 1) << 7, o[2] |= 8, o[3] = 0), { config: o, samplerate: h2[u2], channelCount: a2, codec: "mp4a.40." + i, manifestCodec: c };
}
function hn(n12, e) {
  return n12[e] === 255 && (n12[e + 1] & 246) === 240;
}
function un(n12, e) {
  return n12[e + 1] & 1 ? 7 : 9;
}
function Mi(n12, e) {
  return (n12[e + 3] & 3) << 11 | n12[e + 4] << 3 | (n12[e + 5] & 224) >>> 5;
}
function Co(n12, e) {
  return e + 5 < n12.length;
}
function $t(n12, e) {
  return e + 1 < n12.length && hn(n12, e);
}
function wo(n12, e) {
  return Co(n12, e) && hn(n12, e) && Mi(n12, e) <= n12.length - e;
}
function _o(n12, e) {
  if ($t(n12, e)) {
    let t = un(n12, e);
    if (e + t >= n12.length)
      return false;
    let s2 = Mi(n12, e);
    if (s2 <= t)
      return false;
    let i = e + s2;
    return i === n12.length || $t(n12, i);
  }
  return false;
}
function dn(n12, e, t, s2, i) {
  if (!n12.samplerate) {
    let r2 = Do(e, t, s2, i);
    if (!r2)
      return;
    n12.config = r2.config, n12.samplerate = r2.samplerate, n12.channelCount = r2.channelCount, n12.codec = r2.codec, n12.manifestCodec = r2.manifestCodec, v2.log(`parsed codec:${n12.codec}, rate:${r2.samplerate}, channels:${r2.channelCount}`);
  }
}
function fn(n12) {
  return 1024 * 9e4 / n12;
}
function Po(n12, e) {
  let t = un(n12, e);
  if (e + t <= n12.length) {
    let s2 = Mi(n12, e) - t;
    if (s2 > 0)
      return { headerLength: t, frameLength: s2 };
  }
}
function gn(n12, e, t, s2, i) {
  let r2 = fn(n12.samplerate), a2 = s2 + i * r2, o = Po(e, t), l2;
  if (o) {
    let { frameLength: u2, headerLength: d3 } = o, f3 = d3 + u2, g2 = Math.max(0, t + f3 - e.length);
    g2 ? (l2 = new Uint8Array(f3 - d3), l2.set(e.subarray(t + d3, e.length), 0)) : l2 = e.subarray(t + d3, t + f3);
    let p3 = { unit: l2, pts: a2 };
    return g2 || n12.samples.push(p3), { sample: p3, length: f3, missing: g2 };
  }
  let c = e.length - t;
  return l2 = new Uint8Array(c), l2.set(e.subarray(t, e.length), 0), { sample: { unit: l2, pts: a2 }, length: c, missing: -1 };
}
var pt = null;
var ko = [32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 384, 416, 448, 32, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 384, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 32, 48, 56, 64, 80, 96, 112, 128, 144, 160, 176, 192, 224, 256, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160];
var Fo = [44100, 48e3, 32e3, 22050, 24e3, 16e3, 11025, 12e3, 8e3];
var Mo = [[0, 72, 144, 12], [0, 0, 0, 0], [0, 72, 144, 12], [0, 144, 144, 12]];
var Oo = [0, 1, 1, 4];
function mn(n12, e, t, s2, i) {
  if (t + 24 > e.length)
    return;
  let r2 = pn(e, t);
  if (r2 && t + r2.frameLength <= e.length) {
    let a2 = r2.samplesPerFrame * 9e4 / r2.sampleRate, o = s2 + i * a2, l2 = { unit: e.subarray(t, t + r2.frameLength), pts: o, dts: o };
    return n12.config = [], n12.channelCount = r2.channelCount, n12.samplerate = r2.sampleRate, n12.samples.push(l2), { sample: l2, length: r2.frameLength, missing: 0 };
  }
}
function pn(n12, e) {
  let t = n12[e + 1] >> 3 & 3, s2 = n12[e + 1] >> 1 & 3, i = n12[e + 2] >> 4 & 15, r2 = n12[e + 2] >> 2 & 3;
  if (t !== 1 && i !== 0 && i !== 15 && r2 !== 3) {
    let a2 = n12[e + 2] >> 1 & 1, o = n12[e + 3] >> 6, l2 = t === 3 ? 3 - s2 : s2 === 3 ? 3 : 4, c = ko[l2 * 14 + i - 1] * 1e3, u2 = Fo[(t === 3 ? 0 : t === 2 ? 1 : 2) * 3 + r2], d3 = o === 3 ? 1 : 2, f3 = Mo[t][s2], g2 = Oo[s2], p3 = f3 * 8 * g2, T2 = Math.floor(f3 * c / u2 + a2) * g2;
    if (pt === null) {
      let y3 = (navigator.userAgent || "").match(/Chrome\/(\d+)/i);
      pt = y3 ? parseInt(y3[1]) : 0;
    }
    return !!pt && pt <= 87 && s2 === 2 && c >= 224e3 && o === 0 && (n12[e + 3] = n12[e + 3] | 128), { sampleRate: u2, channelCount: d3, frameLength: T2, samplesPerFrame: p3 };
  }
}
function Oi(n12, e) {
  return n12[e] === 255 && (n12[e + 1] & 224) === 224 && (n12[e + 1] & 6) !== 0;
}
function Tn(n12, e) {
  return e + 1 < n12.length && Oi(n12, e);
}
function No(n12, e) {
  return Oi(n12, e) && 4 <= n12.length - e;
}
function En(n12, e) {
  if (e + 1 < n12.length && Oi(n12, e)) {
    let s2 = pn(n12, e), i = 4;
    s2 != null && s2.frameLength && (i = s2.frameLength);
    let r2 = e + i;
    return r2 === n12.length || Tn(n12, r2);
  }
  return false;
}
var Ks = class extends ct {
  constructor(e, t) {
    super(), this.observer = void 0, this.config = void 0, this.observer = e, this.config = t;
  }
  resetInitSegment(e, t, s2, i) {
    super.resetInitSegment(e, t, s2, i), this._audioTrack = { container: "audio/adts", type: "audio", id: 2, pid: -1, sequenceNumber: 0, segmentCodec: "aac", samples: [], manifestCodec: t, duration: i, inputTimeScale: 9e4, dropped: 0 };
  }
  static probe(e) {
    if (!e)
      return false;
    let t = st(e, 0), s2 = t?.length || 0;
    if (En(e, s2))
      return false;
    for (let i = e.length; s2 < i; s2++)
      if (_o(e, s2))
        return v2.log("ADTS sync word found !"), true;
    return false;
  }
  canParse(e, t) {
    return wo(e, t);
  }
  appendFrame(e, t, s2) {
    dn(e, this.observer, t, s2, e.manifestCodec);
    let i = gn(e, t, s2, this.basePTS, this.frameIndex);
    if (i && i.missing === 0)
      return i;
  }
};
var Uo = /\/emsg[-/]ID3/i;
var Hs = class {
  constructor(e, t) {
    this.remainderData = null, this.timeOffset = 0, this.config = void 0, this.videoTrack = void 0, this.audioTrack = void 0, this.id3Track = void 0, this.txtTrack = void 0, this.config = t;
  }
  resetTimeStamp() {
  }
  resetInitSegment(e, t, s2, i) {
    let r2 = this.videoTrack = Ae2("video", 1), a2 = this.audioTrack = Ae2("audio", 1), o = this.txtTrack = Ae2("text", 1);
    if (this.id3Track = Ae2("id3", 1), this.timeOffset = 0, !(e != null && e.byteLength))
      return;
    let l2 = jr(e);
    if (l2.video) {
      let { id: c, timescale: h2, codec: u2 } = l2.video;
      r2.id = c, r2.timescale = o.timescale = h2, r2.codec = u2;
    }
    if (l2.audio) {
      let { id: c, timescale: h2, codec: u2 } = l2.audio;
      a2.id = c, a2.timescale = h2, a2.codec = u2;
    }
    o.id = Wr.text, r2.sampleDuration = 0, r2.duration = a2.duration = i;
  }
  resetContiguity() {
    this.remainderData = null;
  }
  static probe(e) {
    return pa(e);
  }
  demux(e, t) {
    this.timeOffset = t;
    let s2 = e, i = this.videoTrack, r2 = this.txtTrack;
    if (this.config.progressive) {
      this.remainderData && (s2 = pe3(this.remainderData, e));
      let o = La(s2);
      this.remainderData = o.remainder, i.samples = o.valid || new Uint8Array();
    } else
      i.samples = s2;
    let a2 = this.extractID3Track(i, t);
    return r2.samples = qi(t, i), { videoTrack: i, audioTrack: this.audioTrack, id3Track: a2, textTrack: this.txtTrack };
  }
  flush() {
    let e = this.timeOffset, t = this.videoTrack, s2 = this.txtTrack;
    t.samples = this.remainderData || new Uint8Array(), this.remainderData = null;
    let i = this.extractID3Track(t, this.timeOffset);
    return s2.samples = qi(e, t), { videoTrack: t, audioTrack: Ae2(), id3Track: i, textTrack: Ae2() };
  }
  extractID3Track(e, t) {
    let s2 = this.id3Track;
    if (e.samples.length) {
      let i = H2(e.samples, ["emsg"]);
      i && i.forEach((r2) => {
        let a2 = ba(r2);
        if (Uo.test(a2.schemeIdUri)) {
          let o = F(a2.presentationTime) ? a2.presentationTime / a2.timeScale : t + a2.presentationTimeDelta / a2.timeScale, l2 = a2.eventDuration === 4294967295 ? Number.POSITIVE_INFINITY : a2.eventDuration / a2.timeScale;
          l2 <= 1e-3 && (l2 = Number.POSITIVE_INFINITY);
          let c = a2.payload;
          s2.samples.push({ data: c, len: c.byteLength, dts: o, pts: o, type: xe3.emsg, duration: l2 });
        }
      });
    }
    return s2;
  }
  demuxSampleAes(e, t, s2) {
    return Promise.reject(new Error("The MP4 demuxer does not support SAMPLE-AES decryption"));
  }
  destroy() {
  }
};
var yn = (n12, e) => {
  let t = 0, s2 = 5;
  e += s2;
  let i = new Uint32Array(1), r2 = new Uint32Array(1), a2 = new Uint8Array(1);
  for (; s2 > 0; ) {
    a2[0] = n12[e];
    let o = Math.min(s2, 8), l2 = 8 - o;
    r2[0] = 4278190080 >>> 24 + l2 << l2, i[0] = (a2[0] & r2[0]) >> l2, t = t ? t << o | i[0] : i[0], e += 1, s2 -= o;
  }
  return t;
};
var Vs = class extends ct {
  constructor(e) {
    super(), this.observer = void 0, this.observer = e;
  }
  resetInitSegment(e, t, s2, i) {
    super.resetInitSegment(e, t, s2, i), this._audioTrack = { container: "audio/ac-3", type: "audio", id: 2, pid: -1, sequenceNumber: 0, segmentCodec: "ac3", samples: [], manifestCodec: t, duration: i, inputTimeScale: 9e4, dropped: 0 };
  }
  canParse(e, t) {
    return t + 64 < e.length;
  }
  appendFrame(e, t, s2) {
    let i = xn(e, t, s2, this.basePTS, this.frameIndex);
    if (i !== -1)
      return { sample: e.samples[e.samples.length - 1], length: i, missing: 0 };
  }
  static probe(e) {
    if (!e)
      return false;
    let t = st(e, 0);
    if (!t)
      return false;
    let s2 = t.length;
    return e[s2] === 11 && e[s2 + 1] === 119 && ki(t) !== void 0 && yn(e, s2) < 16;
  }
};
function xn(n12, e, t, s2, i) {
  if (t + 8 > e.length || e[t] !== 11 || e[t + 1] !== 119)
    return -1;
  let r2 = e[t + 4] >> 6;
  if (r2 >= 3)
    return -1;
  let o = [48e3, 44100, 32e3][r2], l2 = e[t + 4] & 63, h2 = [64, 69, 96, 64, 70, 96, 80, 87, 120, 80, 88, 120, 96, 104, 144, 96, 105, 144, 112, 121, 168, 112, 122, 168, 128, 139, 192, 128, 140, 192, 160, 174, 240, 160, 175, 240, 192, 208, 288, 192, 209, 288, 224, 243, 336, 224, 244, 336, 256, 278, 384, 256, 279, 384, 320, 348, 480, 320, 349, 480, 384, 417, 576, 384, 418, 576, 448, 487, 672, 448, 488, 672, 512, 557, 768, 512, 558, 768, 640, 696, 960, 640, 697, 960, 768, 835, 1152, 768, 836, 1152, 896, 975, 1344, 896, 976, 1344, 1024, 1114, 1536, 1024, 1115, 1536, 1152, 1253, 1728, 1152, 1254, 1728, 1280, 1393, 1920, 1280, 1394, 1920][l2 * 3 + r2] * 2;
  if (t + h2 > e.length)
    return -1;
  let u2 = e[t + 6] >> 5, d3 = 0;
  u2 === 2 ? d3 += 2 : (u2 & 1 && u2 !== 1 && (d3 += 2), u2 & 4 && (d3 += 2));
  let f3 = (e[t + 6] << 8 | e[t + 7]) >> 12 - d3 & 1, p3 = [2, 1, 2, 3, 3, 4, 4, 5][u2] + f3, T2 = e[t + 5] >> 3, E4 = e[t + 5] & 7, x3 = new Uint8Array([r2 << 6 | T2 << 1 | E4 >> 2, (E4 & 3) << 6 | u2 << 3 | f3 << 2 | l2 >> 4, l2 << 4 & 224]), y3 = 1536 / o * 9e4, R = s2 + i * y3, S2 = e.subarray(t, t + h2);
  return n12.config = x3, n12.channelCount = p3, n12.samplerate = o, n12.samples.push({ unit: S2, pts: R }), h2;
}
var Ws = class {
  constructor() {
    this.VideoSample = null;
  }
  createVideoSample(e, t, s2, i) {
    return { key: e, frame: false, pts: t, dts: s2, units: [], debug: i, length: 0 };
  }
  getLastNalUnit(e) {
    var t;
    let s2 = this.VideoSample, i;
    if ((!s2 || s2.units.length === 0) && (s2 = e[e.length - 1]), (t = s2) != null && t.units) {
      let r2 = s2.units;
      i = r2[r2.length - 1];
    }
    return i;
  }
  pushAccessUnit(e, t) {
    if (e.units.length && e.frame) {
      if (e.pts === void 0) {
        let s2 = t.samples, i = s2.length;
        if (i) {
          let r2 = s2[i - 1];
          e.pts = r2.pts, e.dts = r2.dts;
        } else {
          t.dropped++;
          return;
        }
      }
      t.samples.push(e);
    }
    e.debug.length && v2.log(e.pts + "/" + e.dts + ":" + e.debug);
  }
};
var Gt = class {
  constructor(e) {
    this.data = void 0, this.bytesAvailable = void 0, this.word = void 0, this.bitsAvailable = void 0, this.data = e, this.bytesAvailable = e.byteLength, this.word = 0, this.bitsAvailable = 0;
  }
  loadWord() {
    let e = this.data, t = this.bytesAvailable, s2 = e.byteLength - t, i = new Uint8Array(4), r2 = Math.min(4, t);
    if (r2 === 0)
      throw new Error("no bytes available");
    i.set(e.subarray(s2, s2 + r2)), this.word = new DataView(i.buffer).getUint32(0), this.bitsAvailable = r2 * 8, this.bytesAvailable -= r2;
  }
  skipBits(e) {
    let t;
    e = Math.min(e, this.bytesAvailable * 8 + this.bitsAvailable), this.bitsAvailable > e ? (this.word <<= e, this.bitsAvailable -= e) : (e -= this.bitsAvailable, t = e >> 3, e -= t << 3, this.bytesAvailable -= t, this.loadWord(), this.word <<= e, this.bitsAvailable -= e);
  }
  readBits(e) {
    let t = Math.min(this.bitsAvailable, e), s2 = this.word >>> 32 - t;
    if (e > 32 && v2.error("Cannot read more than 32 bits at a time"), this.bitsAvailable -= t, this.bitsAvailable > 0)
      this.word <<= t;
    else if (this.bytesAvailable > 0)
      this.loadWord();
    else
      throw new Error("no bits available");
    return t = e - t, t > 0 && this.bitsAvailable ? s2 << t | this.readBits(t) : s2;
  }
  skipLZ() {
    let e;
    for (e = 0; e < this.bitsAvailable; ++e)
      if (this.word & 2147483648 >>> e)
        return this.word <<= e, this.bitsAvailable -= e, e;
    return this.loadWord(), e + this.skipLZ();
  }
  skipUEG() {
    this.skipBits(1 + this.skipLZ());
  }
  skipEG() {
    this.skipBits(1 + this.skipLZ());
  }
  readUEG() {
    let e = this.skipLZ();
    return this.readBits(e + 1) - 1;
  }
  readEG() {
    let e = this.readUEG();
    return 1 & e ? 1 + e >>> 1 : -1 * (e >>> 1);
  }
  readBoolean() {
    return this.readBits(1) === 1;
  }
  readUByte() {
    return this.readBits(8);
  }
  readUShort() {
    return this.readBits(16);
  }
  readUInt() {
    return this.readBits(32);
  }
  skipScalingList(e) {
    let t = 8, s2 = 8, i;
    for (let r2 = 0; r2 < e; r2++)
      s2 !== 0 && (i = this.readEG(), s2 = (t + i + 256) % 256), t = s2 === 0 ? t : s2;
  }
  readSPS() {
    let e = 0, t = 0, s2 = 0, i = 0, r2, a2, o, l2 = this.readUByte.bind(this), c = this.readBits.bind(this), h2 = this.readUEG.bind(this), u2 = this.readBoolean.bind(this), d3 = this.skipBits.bind(this), f3 = this.skipEG.bind(this), g2 = this.skipUEG.bind(this), p3 = this.skipScalingList.bind(this);
    l2();
    let T2 = l2();
    if (c(5), d3(3), l2(), g2(), T2 === 100 || T2 === 110 || T2 === 122 || T2 === 244 || T2 === 44 || T2 === 83 || T2 === 86 || T2 === 118 || T2 === 128) {
      let b = h2();
      if (b === 3 && d3(1), g2(), g2(), d3(1), u2())
        for (a2 = b !== 3 ? 8 : 12, o = 0; o < a2; o++)
          u2() && (o < 6 ? p3(16) : p3(64));
    }
    g2();
    let E4 = h2();
    if (E4 === 0)
      h2();
    else if (E4 === 1)
      for (d3(1), f3(), f3(), r2 = h2(), o = 0; o < r2; o++)
        f3();
    g2(), d3(1);
    let x3 = h2(), y3 = h2(), R = c(1);
    R === 0 && d3(1), d3(1), u2() && (e = h2(), t = h2(), s2 = h2(), i = h2());
    let S2 = [1, 1];
    if (u2() && u2())
      switch (l2()) {
        case 1:
          S2 = [1, 1];
          break;
        case 2:
          S2 = [12, 11];
          break;
        case 3:
          S2 = [10, 11];
          break;
        case 4:
          S2 = [16, 11];
          break;
        case 5:
          S2 = [40, 33];
          break;
        case 6:
          S2 = [24, 11];
          break;
        case 7:
          S2 = [20, 11];
          break;
        case 8:
          S2 = [32, 11];
          break;
        case 9:
          S2 = [80, 33];
          break;
        case 10:
          S2 = [18, 11];
          break;
        case 11:
          S2 = [15, 11];
          break;
        case 12:
          S2 = [64, 33];
          break;
        case 13:
          S2 = [160, 99];
          break;
        case 14:
          S2 = [4, 3];
          break;
        case 15:
          S2 = [3, 2];
          break;
        case 16:
          S2 = [2, 1];
          break;
        case 255: {
          S2 = [l2() << 8 | l2(), l2() << 8 | l2()];
          break;
        }
      }
    return { width: Math.ceil((x3 + 1) * 16 - e * 2 - t * 2), height: (2 - R) * (y3 + 1) * 16 - (R ? 2 : 4) * (s2 + i), pixelRatio: S2 };
  }
  readSliceType() {
    return this.readUByte(), this.readUEG(), this.readUEG();
  }
};
var Ys = class extends Ws {
  parseAVCPES(e, t, s2, i, r2) {
    let a2 = this.parseAVCNALu(e, s2.data), o = this.VideoSample, l2, c = false;
    s2.data = null, o && a2.length && !e.audFound && (this.pushAccessUnit(o, e), o = this.VideoSample = this.createVideoSample(false, s2.pts, s2.dts, "")), a2.forEach((h2) => {
      var u2;
      switch (h2.type) {
        case 1: {
          let p3 = false;
          l2 = true;
          let T2 = h2.data;
          if (c && T2.length > 4) {
            let E4 = new Gt(T2).readSliceType();
            (E4 === 2 || E4 === 4 || E4 === 7 || E4 === 9) && (p3 = true);
          }
          if (p3) {
            var d3;
            (d3 = o) != null && d3.frame && !o.key && (this.pushAccessUnit(o, e), o = this.VideoSample = null);
          }
          o || (o = this.VideoSample = this.createVideoSample(true, s2.pts, s2.dts, "")), o.frame = true, o.key = p3;
          break;
        }
        case 5:
          l2 = true, (u2 = o) != null && u2.frame && !o.key && (this.pushAccessUnit(o, e), o = this.VideoSample = null), o || (o = this.VideoSample = this.createVideoSample(true, s2.pts, s2.dts, "")), o.key = true, o.frame = true;
          break;
        case 6: {
          l2 = true, Xr(h2.data, 1, s2.pts, t.samples);
          break;
        }
        case 7: {
          var f3, g2;
          l2 = true, c = true;
          let p3 = h2.data, E4 = new Gt(p3).readSPS();
          if (!e.sps || e.width !== E4.width || e.height !== E4.height || ((f3 = e.pixelRatio) == null ? void 0 : f3[0]) !== E4.pixelRatio[0] || ((g2 = e.pixelRatio) == null ? void 0 : g2[1]) !== E4.pixelRatio[1]) {
            e.width = E4.width, e.height = E4.height, e.pixelRatio = E4.pixelRatio, e.sps = [p3], e.duration = r2;
            let x3 = p3.subarray(1, 4), y3 = "avc1.";
            for (let R = 0; R < 3; R++) {
              let S2 = x3[R].toString(16);
              S2.length < 2 && (S2 = "0" + S2), y3 += S2;
            }
            e.codec = y3;
          }
          break;
        }
        case 8:
          l2 = true, e.pps = [h2.data];
          break;
        case 9:
          l2 = true, e.audFound = true, o && this.pushAccessUnit(o, e), o = this.VideoSample = this.createVideoSample(false, s2.pts, s2.dts, "");
          break;
        case 12:
          l2 = true;
          break;
        default:
          l2 = false, o && (o.debug += "unknown NAL " + h2.type + " ");
          break;
      }
      o && l2 && o.units.push(h2);
    }), i && o && (this.pushAccessUnit(o, e), this.VideoSample = null);
  }
  parseAVCNALu(e, t) {
    let s2 = t.byteLength, i = e.naluState || 0, r2 = i, a2 = [], o = 0, l2, c, h2, u2 = -1, d3 = 0;
    for (i === -1 && (u2 = 0, d3 = t[0] & 31, i = 0, o = 1); o < s2; ) {
      if (l2 = t[o++], !i) {
        i = l2 ? 0 : 1;
        continue;
      }
      if (i === 1) {
        i = l2 ? 0 : 2;
        continue;
      }
      if (!l2)
        i = 3;
      else if (l2 === 1) {
        if (c = o - i - 1, u2 >= 0) {
          let f3 = { data: t.subarray(u2, c), type: d3 };
          a2.push(f3);
        } else {
          let f3 = this.getLastNalUnit(e.samples);
          f3 && (r2 && o <= 4 - r2 && f3.state && (f3.data = f3.data.subarray(0, f3.data.byteLength - r2)), c > 0 && (f3.data = pe3(f3.data, t.subarray(0, c)), f3.state = 0));
        }
        o < s2 ? (h2 = t[o] & 31, u2 = o, d3 = h2, i = 0) : i = -1;
      } else
        i = 0;
    }
    if (u2 >= 0 && i >= 0) {
      let f3 = { data: t.subarray(u2, s2), type: d3, state: i };
      a2.push(f3);
    }
    if (a2.length === 0) {
      let f3 = this.getLastNalUnit(e.samples);
      f3 && (f3.data = pe3(f3.data, t));
    }
    return e.naluState = i, a2;
  }
};
var qs = class {
  constructor(e, t, s2) {
    this.keyData = void 0, this.decrypter = void 0, this.keyData = s2, this.decrypter = new ot(t, { removePKCS7Padding: false });
  }
  decryptBuffer(e) {
    return this.decrypter.decrypt(e, this.keyData.key.buffer, this.keyData.iv.buffer);
  }
  decryptAacSample(e, t, s2) {
    let i = e[t].unit;
    if (i.length <= 16)
      return;
    let r2 = i.subarray(16, i.length - i.length % 16), a2 = r2.buffer.slice(r2.byteOffset, r2.byteOffset + r2.length);
    this.decryptBuffer(a2).then((o) => {
      let l2 = new Uint8Array(o);
      i.set(l2, 16), this.decrypter.isSync() || this.decryptAacSamples(e, t + 1, s2);
    });
  }
  decryptAacSamples(e, t, s2) {
    for (; ; t++) {
      if (t >= e.length) {
        s2();
        return;
      }
      if (!(e[t].unit.length < 32) && (this.decryptAacSample(e, t, s2), !this.decrypter.isSync()))
        return;
    }
  }
  getAvcEncryptedData(e) {
    let t = Math.floor((e.length - 48) / 160) * 16 + 16, s2 = new Int8Array(t), i = 0;
    for (let r2 = 32; r2 < e.length - 16; r2 += 160, i += 16)
      s2.set(e.subarray(r2, r2 + 16), i);
    return s2;
  }
  getAvcDecryptedUnit(e, t) {
    let s2 = new Uint8Array(t), i = 0;
    for (let r2 = 32; r2 < e.length - 16; r2 += 160, i += 16)
      e.set(s2.subarray(i, i + 16), r2);
    return e;
  }
  decryptAvcSample(e, t, s2, i, r2) {
    let a2 = Qr(r2.data), o = this.getAvcEncryptedData(a2);
    this.decryptBuffer(o.buffer).then((l2) => {
      r2.data = this.getAvcDecryptedUnit(a2, l2), this.decrypter.isSync() || this.decryptAvcSamples(e, t, s2 + 1, i);
    });
  }
  decryptAvcSamples(e, t, s2, i) {
    if (e instanceof Uint8Array)
      throw new Error("Cannot decrypt samples of type Uint8Array");
    for (; ; t++, s2 = 0) {
      if (t >= e.length) {
        i();
        return;
      }
      let r2 = e[t].units;
      for (; !(s2 >= r2.length); s2++) {
        let a2 = r2[s2];
        if (!(a2.data.length <= 48 || a2.type !== 1 && a2.type !== 5) && (this.decryptAvcSample(e, t, s2, i, a2), !this.decrypter.isSync()))
          return;
      }
    }
  }
};
var ne2 = 188;
var js = class n6 {
  constructor(e, t, s2) {
    this.observer = void 0, this.config = void 0, this.typeSupported = void 0, this.sampleAes = null, this.pmtParsed = false, this.audioCodec = void 0, this.videoCodec = void 0, this._duration = 0, this._pmtId = -1, this._videoTrack = void 0, this._audioTrack = void 0, this._id3Track = void 0, this._txtTrack = void 0, this.aacOverFlow = null, this.remainderData = null, this.videoParser = void 0, this.observer = e, this.config = t, this.typeSupported = s2, this.videoParser = new Ys();
  }
  static probe(e) {
    let t = n6.syncOffset(e);
    return t > 0 && v2.warn(`MPEG2-TS detected but first sync word found @ offset ${t}`), t !== -1;
  }
  static syncOffset(e) {
    let t = e.length, s2 = Math.min(ne2 * 5, t - ne2) + 1, i = 0;
    for (; i < s2; ) {
      let r2 = false, a2 = -1, o = 0;
      for (let l2 = i; l2 < t; l2 += ne2)
        if (e[l2] === 71 && (t - l2 === ne2 || e[l2 + ne2] === 71)) {
          if (o++, a2 === -1 && (a2 = l2, a2 !== 0 && (s2 = Math.min(a2 + ne2 * 99, e.length - ne2) + 1)), r2 || (r2 = zs(e, l2) === 0), r2 && o > 1 && (a2 === 0 && o > 2 || l2 + ne2 > s2))
            return a2;
        } else {
          if (o)
            return -1;
          break;
        }
      i++;
    }
    return -1;
  }
  static createTrack(e, t) {
    return { container: e === "video" || e === "audio" ? "video/mp2t" : void 0, type: e, id: Wr[e], pid: -1, inputTimeScale: 9e4, sequenceNumber: 0, samples: [], dropped: 0, duration: e === "audio" ? t : void 0 };
  }
  resetInitSegment(e, t, s2, i) {
    this.pmtParsed = false, this._pmtId = -1, this._videoTrack = n6.createTrack("video"), this._audioTrack = n6.createTrack("audio", i), this._id3Track = n6.createTrack("id3"), this._txtTrack = n6.createTrack("text"), this._audioTrack.segmentCodec = "aac", this.aacOverFlow = null, this.remainderData = null, this.audioCodec = t, this.videoCodec = s2, this._duration = i;
  }
  resetTimeStamp() {
  }
  resetContiguity() {
    let { _audioTrack: e, _videoTrack: t, _id3Track: s2 } = this;
    e && (e.pesData = null), t && (t.pesData = null), s2 && (s2.pesData = null), this.aacOverFlow = null, this.remainderData = null;
  }
  demux(e, t, s2 = false, i = false) {
    s2 || (this.sampleAes = null);
    let r2, a2 = this._videoTrack, o = this._audioTrack, l2 = this._id3Track, c = this._txtTrack, h2 = a2.pid, u2 = a2.pesData, d3 = o.pid, f3 = l2.pid, g2 = o.pesData, p3 = l2.pesData, T2 = null, E4 = this.pmtParsed, x3 = this._pmtId, y3 = e.length;
    if (this.remainderData && (e = pe3(this.remainderData, e), y3 = e.length, this.remainderData = null), y3 < ne2 && !i)
      return this.remainderData = e, { audioTrack: o, videoTrack: a2, id3Track: l2, textTrack: c };
    let R = Math.max(0, n6.syncOffset(e));
    y3 -= (y3 - R) % ne2, y3 < e.byteLength && !i && (this.remainderData = new Uint8Array(e.buffer, y3, e.buffer.byteLength - y3));
    let S2 = 0;
    for (let L = R; L < y3; L += ne2)
      if (e[L] === 71) {
        let C2 = !!(e[L + 1] & 64), _ = zs(e, L), I = (e[L + 3] & 48) >> 4, w;
        if (I > 1) {
          if (w = L + 5 + e[L + 4], w === L + ne2)
            continue;
        } else
          w = L + 4;
        switch (_) {
          case h2:
            C2 && (u2 && (r2 = Ge(u2)) && this.videoParser.parseAVCPES(a2, c, r2, false, this._duration), u2 = { data: [], size: 0 }), u2 && (u2.data.push(e.subarray(w, L + ne2)), u2.size += L + ne2 - w);
            break;
          case d3:
            if (C2) {
              if (g2 && (r2 = Ge(g2)))
                switch (o.segmentCodec) {
                  case "aac":
                    this.parseAACPES(o, r2);
                    break;
                  case "mp3":
                    this.parseMPEGPES(o, r2);
                    break;
                  case "ac3":
                    this.parseAC3PES(o, r2);
                    break;
                }
              g2 = { data: [], size: 0 };
            }
            g2 && (g2.data.push(e.subarray(w, L + ne2)), g2.size += L + ne2 - w);
            break;
          case f3:
            C2 && (p3 && (r2 = Ge(p3)) && this.parseID3PES(l2, r2), p3 = { data: [], size: 0 }), p3 && (p3.data.push(e.subarray(w, L + ne2)), p3.size += L + ne2 - w);
            break;
          case 0:
            C2 && (w += e[w] + 1), x3 = this._pmtId = Bo(e, w);
            break;
          case x3: {
            C2 && (w += e[w] + 1);
            let K = $o(e, w, this.typeSupported, s2);
            h2 = K.videoPid, h2 > 0 && (a2.pid = h2, a2.segmentCodec = K.segmentVideoCodec), d3 = K.audioPid, d3 > 0 && (o.pid = d3, o.segmentCodec = K.segmentAudioCodec), f3 = K.id3Pid, f3 > 0 && (l2.pid = f3), T2 !== null && !E4 && (v2.warn(`MPEG-TS PMT found at ${L} after unknown PID '${T2}'. Backtracking to sync byte @${R} to parse all TS packets.`), T2 = null, L = R - 188), E4 = this.pmtParsed = true;
            break;
          }
          case 17:
          case 8191:
            break;
          default:
            T2 = _;
            break;
        }
      } else
        S2++;
    if (S2 > 0) {
      let L = new Error(`Found ${S2} TS packet/s that do not start with 0x47`);
      this.observer.emit(m2.ERROR, m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, fatal: false, error: L, reason: L.message });
    }
    a2.pesData = u2, o.pesData = g2, l2.pesData = p3;
    let b = { audioTrack: o, videoTrack: a2, id3Track: l2, textTrack: c };
    return i && this.extractRemainingSamples(b), b;
  }
  flush() {
    let { remainderData: e } = this;
    this.remainderData = null;
    let t;
    return e ? t = this.demux(e, -1, false, true) : t = { videoTrack: this._videoTrack, audioTrack: this._audioTrack, id3Track: this._id3Track, textTrack: this._txtTrack }, this.extractRemainingSamples(t), this.sampleAes ? this.decrypt(t, this.sampleAes) : t;
  }
  extractRemainingSamples(e) {
    let { audioTrack: t, videoTrack: s2, id3Track: i, textTrack: r2 } = e, a2 = s2.pesData, o = t.pesData, l2 = i.pesData, c;
    if (a2 && (c = Ge(a2)) ? (this.videoParser.parseAVCPES(s2, r2, c, true, this._duration), s2.pesData = null) : s2.pesData = a2, o && (c = Ge(o))) {
      switch (t.segmentCodec) {
        case "aac":
          this.parseAACPES(t, c);
          break;
        case "mp3":
          this.parseMPEGPES(t, c);
          break;
        case "ac3":
          this.parseAC3PES(t, c);
          break;
      }
      t.pesData = null;
    } else
      o != null && o.size && v2.log("last AAC PES packet truncated,might overlap between fragments"), t.pesData = o;
    l2 && (c = Ge(l2)) ? (this.parseID3PES(i, c), i.pesData = null) : i.pesData = l2;
  }
  demuxSampleAes(e, t, s2) {
    let i = this.demux(e, s2, true, !this.config.progressive), r2 = this.sampleAes = new qs(this.observer, this.config, t);
    return this.decrypt(i, r2);
  }
  decrypt(e, t) {
    return new Promise((s2) => {
      let { audioTrack: i, videoTrack: r2 } = e;
      i.samples && i.segmentCodec === "aac" ? t.decryptAacSamples(i.samples, 0, () => {
        r2.samples ? t.decryptAvcSamples(r2.samples, 0, 0, () => {
          s2(e);
        }) : s2(e);
      }) : r2.samples && t.decryptAvcSamples(r2.samples, 0, 0, () => {
        s2(e);
      });
    });
  }
  destroy() {
    this._duration = 0;
  }
  parseAACPES(e, t) {
    let s2 = 0, i = this.aacOverFlow, r2 = t.data;
    if (i) {
      this.aacOverFlow = null;
      let u2 = i.missing, d3 = i.sample.unit.byteLength;
      if (u2 === -1)
        r2 = pe3(i.sample.unit, r2);
      else {
        let f3 = d3 - u2;
        i.sample.unit.set(r2.subarray(0, u2), f3), e.samples.push(i.sample), s2 = i.missing;
      }
    }
    let a2, o;
    for (a2 = s2, o = r2.length; a2 < o - 1 && !$t(r2, a2); a2++)
      ;
    if (a2 !== s2) {
      let u2, d3 = a2 < o - 1;
      d3 ? u2 = `AAC PES did not start with ADTS header,offset:${a2}` : u2 = "No ADTS header found in AAC PES";
      let f3 = new Error(u2);
      if (v2.warn(`parsing error: ${u2}`), this.observer.emit(m2.ERROR, m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, fatal: false, levelRetry: d3, error: f3, reason: u2 }), !d3)
        return;
    }
    dn(e, this.observer, r2, a2, this.audioCodec);
    let l2;
    if (t.pts !== void 0)
      l2 = t.pts;
    else if (i) {
      let u2 = fn(e.samplerate);
      l2 = i.sample.pts + u2;
    } else {
      v2.warn("[tsdemuxer]: AAC PES unknown PTS");
      return;
    }
    let c = 0, h2;
    for (; a2 < o; )
      if (h2 = gn(e, r2, a2, l2, c), a2 += h2.length, h2.missing) {
        this.aacOverFlow = h2;
        break;
      } else
        for (c++; a2 < o - 1 && !$t(r2, a2); a2++)
          ;
  }
  parseMPEGPES(e, t) {
    let s2 = t.data, i = s2.length, r2 = 0, a2 = 0, o = t.pts;
    if (o === void 0) {
      v2.warn("[tsdemuxer]: MPEG PES unknown PTS");
      return;
    }
    for (; a2 < i; )
      if (Tn(s2, a2)) {
        let l2 = mn(e, s2, a2, o, r2);
        if (l2)
          a2 += l2.length, r2++;
        else
          break;
      } else
        a2++;
  }
  parseAC3PES(e, t) {
    {
      let s2 = t.data, i = t.pts;
      if (i === void 0) {
        v2.warn("[tsdemuxer]: AC3 PES unknown PTS");
        return;
      }
      let r2 = s2.length, a2 = 0, o = 0, l2;
      for (; o < r2 && (l2 = xn(e, s2, o, i, a2++)) > 0; )
        o += l2;
    }
  }
  parseID3PES(e, t) {
    if (t.pts === void 0) {
      v2.warn("[tsdemuxer]: ID3 PES unknown PTS");
      return;
    }
    let s2 = te2({}, t, { type: this._videoTrack ? xe3.emsg : xe3.audioId3, duration: Number.POSITIVE_INFINITY });
    e.samples.push(s2);
  }
};
function zs(n12, e) {
  return ((n12[e + 1] & 31) << 8) + n12[e + 2];
}
function Bo(n12, e) {
  return (n12[e + 10] & 31) << 8 | n12[e + 11];
}
function $o(n12, e, t, s2) {
  let i = { audioPid: -1, videoPid: -1, id3Pid: -1, segmentVideoCodec: "avc", segmentAudioCodec: "aac" }, r2 = (n12[e + 1] & 15) << 8 | n12[e + 2], a2 = e + 3 + r2 - 4, o = (n12[e + 10] & 15) << 8 | n12[e + 11];
  for (e += 12 + o; e < a2; ) {
    let l2 = zs(n12, e), c = (n12[e + 3] & 15) << 8 | n12[e + 4];
    switch (n12[e]) {
      case 207:
        if (!s2) {
          hs("ADTS AAC");
          break;
        }
      case 15:
        i.audioPid === -1 && (i.audioPid = l2);
        break;
      case 21:
        i.id3Pid === -1 && (i.id3Pid = l2);
        break;
      case 219:
        if (!s2) {
          hs("H.264");
          break;
        }
      case 27:
        i.videoPid === -1 && (i.videoPid = l2, i.segmentVideoCodec = "avc");
        break;
      case 3:
      case 4:
        !t.mpeg && !t.mp3 ? v2.log("MPEG audio found, not supported in this browser") : i.audioPid === -1 && (i.audioPid = l2, i.segmentAudioCodec = "mp3");
        break;
      case 193:
        if (!s2) {
          hs("AC-3");
          break;
        }
      case 129:
        t.ac3 ? i.audioPid === -1 && (i.audioPid = l2, i.segmentAudioCodec = "ac3") : v2.log("AC-3 audio found, not supported in this browser");
        break;
      case 6:
        if (i.audioPid === -1 && c > 0) {
          let h2 = e + 5, u2 = c;
          for (; u2 > 2; ) {
            switch (n12[h2]) {
              case 106:
                t.ac3 !== true ? v2.log("AC-3 audio found, not supported in this browser for now") : (i.audioPid = l2, i.segmentAudioCodec = "ac3");
                break;
            }
            let f3 = n12[h2 + 1] + 2;
            h2 += f3, u2 -= f3;
          }
        }
        break;
      case 194:
      case 135:
        v2.warn("Unsupported EC-3 in M2TS found");
        break;
      case 36:
        v2.warn("Unsupported HEVC in M2TS found");
        break;
    }
    e += c + 5;
  }
  return i;
}
function hs(n12) {
  v2.log(`${n12} with AES-128-CBC encryption found in unencrypted stream`);
}
function Ge(n12) {
  let e = 0, t, s2, i, r2, a2, o = n12.data;
  if (!n12 || n12.size === 0)
    return null;
  for (; o[0].length < 19 && o.length > 1; )
    o[0] = pe3(o[0], o[1]), o.splice(1, 1);
  if (t = o[0], (t[0] << 16) + (t[1] << 8) + t[2] === 1) {
    if (s2 = (t[4] << 8) + t[5], s2 && s2 > n12.size - 6)
      return null;
    let c = t[7];
    c & 192 && (r2 = (t[9] & 14) * 536870912 + (t[10] & 255) * 4194304 + (t[11] & 254) * 16384 + (t[12] & 255) * 128 + (t[13] & 254) / 2, c & 64 ? (a2 = (t[14] & 14) * 536870912 + (t[15] & 255) * 4194304 + (t[16] & 254) * 16384 + (t[17] & 255) * 128 + (t[18] & 254) / 2, r2 - a2 > 60 * 9e4 && (v2.warn(`${Math.round((r2 - a2) / 9e4)}s delta between PTS and DTS, align them`), r2 = a2)) : a2 = r2), i = t[8];
    let h2 = i + 9;
    if (n12.size <= h2)
      return null;
    n12.size -= h2;
    let u2 = new Uint8Array(n12.size);
    for (let d3 = 0, f3 = o.length; d3 < f3; d3++) {
      t = o[d3];
      let g2 = t.byteLength;
      if (h2)
        if (h2 > g2) {
          h2 -= g2;
          continue;
        } else
          t = t.subarray(h2), g2 -= h2, h2 = 0;
      u2.set(t, e), e += g2;
    }
    return s2 && (s2 -= i + 3), { data: u2, pts: r2, dts: a2, len: s2 };
  }
  return null;
}
var Xs = class extends ct {
  resetInitSegment(e, t, s2, i) {
    super.resetInitSegment(e, t, s2, i), this._audioTrack = { container: "audio/mpeg", type: "audio", id: 2, pid: -1, sequenceNumber: 0, segmentCodec: "mp3", samples: [], manifestCodec: t, duration: i, inputTimeScale: 9e4, dropped: 0 };
  }
  static probe(e) {
    if (!e)
      return false;
    let t = st(e, 0), s2 = t?.length || 0;
    if (t && e[s2] === 11 && e[s2 + 1] === 119 && ki(t) !== void 0 && yn(e, s2) <= 16)
      return false;
    for (let i = e.length; s2 < i; s2++)
      if (En(e, s2))
        return v2.log("MPEG Audio sync word found !"), true;
    return false;
  }
  canParse(e, t) {
    return No(e, t);
  }
  appendFrame(e, t, s2) {
    if (this.basePTS !== null)
      return mn(e, t, s2, this.basePTS, this.frameIndex);
  }
};
var Kt = class {
  static getSilentFrame(e, t) {
    switch (e) {
      case "mp4a.40.2":
        if (t === 1)
          return new Uint8Array([0, 200, 0, 128, 35, 128]);
        if (t === 2)
          return new Uint8Array([33, 0, 73, 144, 2, 25, 0, 35, 128]);
        if (t === 3)
          return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 142]);
        if (t === 4)
          return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 128, 44, 128, 8, 2, 56]);
        if (t === 5)
          return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 130, 48, 4, 153, 0, 33, 144, 2, 56]);
        if (t === 6)
          return new Uint8Array([0, 200, 0, 128, 32, 132, 1, 38, 64, 8, 100, 0, 130, 48, 4, 153, 0, 33, 144, 2, 0, 178, 0, 32, 8, 224]);
        break;
      default:
        if (t === 1)
          return new Uint8Array([1, 64, 34, 128, 163, 78, 230, 128, 186, 8, 0, 0, 0, 28, 6, 241, 193, 10, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 94]);
        if (t === 2)
          return new Uint8Array([1, 64, 34, 128, 163, 94, 230, 128, 186, 8, 0, 0, 0, 0, 149, 0, 6, 241, 161, 10, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 94]);
        if (t === 3)
          return new Uint8Array([1, 64, 34, 128, 163, 94, 230, 128, 186, 8, 0, 0, 0, 0, 149, 0, 6, 241, 161, 10, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 90, 94]);
        break;
    }
  }
};
var we3 = Math.pow(2, 32) - 1;
var ie2 = class n7 {
  static init() {
    n7.types = { avc1: [], avcC: [], btrt: [], dinf: [], dref: [], esds: [], ftyp: [], hdlr: [], mdat: [], mdhd: [], mdia: [], mfhd: [], minf: [], moof: [], moov: [], mp4a: [], ".mp3": [], dac3: [], "ac-3": [], mvex: [], mvhd: [], pasp: [], sdtp: [], stbl: [], stco: [], stsc: [], stsd: [], stsz: [], stts: [], tfdt: [], tfhd: [], traf: [], trak: [], trun: [], trex: [], tkhd: [], vmhd: [], smhd: [] };
    let e;
    for (e in n7.types)
      n7.types.hasOwnProperty(e) && (n7.types[e] = [e.charCodeAt(0), e.charCodeAt(1), e.charCodeAt(2), e.charCodeAt(3)]);
    let t = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 118, 105, 100, 101, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86, 105, 100, 101, 111, 72, 97, 110, 100, 108, 101, 114, 0]), s2 = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 115, 111, 117, 110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 83, 111, 117, 110, 100, 72, 97, 110, 100, 108, 101, 114, 0]);
    n7.HDLR_TYPES = { video: t, audio: s2 };
    let i = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 12, 117, 114, 108, 32, 0, 0, 0, 1]), r2 = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0]);
    n7.STTS = n7.STSC = n7.STCO = r2, n7.STSZ = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]), n7.VMHD = new Uint8Array([0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0]), n7.SMHD = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 0]), n7.STSD = new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1]);
    let a2 = new Uint8Array([105, 115, 111, 109]), o = new Uint8Array([97, 118, 99, 49]), l2 = new Uint8Array([0, 0, 0, 1]);
    n7.FTYP = n7.box(n7.types.ftyp, a2, l2, a2, o), n7.DINF = n7.box(n7.types.dinf, n7.box(n7.types.dref, i));
  }
  static box(e, ...t) {
    let s2 = 8, i = t.length, r2 = i;
    for (; i--; )
      s2 += t[i].byteLength;
    let a2 = new Uint8Array(s2);
    for (a2[0] = s2 >> 24 & 255, a2[1] = s2 >> 16 & 255, a2[2] = s2 >> 8 & 255, a2[3] = s2 & 255, a2.set(e, 4), i = 0, s2 = 8; i < r2; i++)
      a2.set(t[i], s2), s2 += t[i].byteLength;
    return a2;
  }
  static hdlr(e) {
    return n7.box(n7.types.hdlr, n7.HDLR_TYPES[e]);
  }
  static mdat(e) {
    return n7.box(n7.types.mdat, e);
  }
  static mdhd(e, t) {
    t *= e;
    let s2 = Math.floor(t / (we3 + 1)), i = Math.floor(t % (we3 + 1));
    return n7.box(n7.types.mdhd, new Uint8Array([1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, e >> 24 & 255, e >> 16 & 255, e >> 8 & 255, e & 255, s2 >> 24, s2 >> 16 & 255, s2 >> 8 & 255, s2 & 255, i >> 24, i >> 16 & 255, i >> 8 & 255, i & 255, 85, 196, 0, 0]));
  }
  static mdia(e) {
    return n7.box(n7.types.mdia, n7.mdhd(e.timescale, e.duration), n7.hdlr(e.type), n7.minf(e));
  }
  static mfhd(e) {
    return n7.box(n7.types.mfhd, new Uint8Array([0, 0, 0, 0, e >> 24, e >> 16 & 255, e >> 8 & 255, e & 255]));
  }
  static minf(e) {
    return e.type === "audio" ? n7.box(n7.types.minf, n7.box(n7.types.smhd, n7.SMHD), n7.DINF, n7.stbl(e)) : n7.box(n7.types.minf, n7.box(n7.types.vmhd, n7.VMHD), n7.DINF, n7.stbl(e));
  }
  static moof(e, t, s2) {
    return n7.box(n7.types.moof, n7.mfhd(e), n7.traf(s2, t));
  }
  static moov(e) {
    let t = e.length, s2 = [];
    for (; t--; )
      s2[t] = n7.trak(e[t]);
    return n7.box.apply(null, [n7.types.moov, n7.mvhd(e[0].timescale, e[0].duration)].concat(s2).concat(n7.mvex(e)));
  }
  static mvex(e) {
    let t = e.length, s2 = [];
    for (; t--; )
      s2[t] = n7.trex(e[t]);
    return n7.box.apply(null, [n7.types.mvex, ...s2]);
  }
  static mvhd(e, t) {
    t *= e;
    let s2 = Math.floor(t / (we3 + 1)), i = Math.floor(t % (we3 + 1)), r2 = new Uint8Array([1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, e >> 24 & 255, e >> 16 & 255, e >> 8 & 255, e & 255, s2 >> 24, s2 >> 16 & 255, s2 >> 8 & 255, s2 & 255, i >> 24, i >> 16 & 255, i >> 8 & 255, i & 255, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 64, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 255, 255, 255, 255]);
    return n7.box(n7.types.mvhd, r2);
  }
  static sdtp(e) {
    let t = e.samples || [], s2 = new Uint8Array(4 + t.length), i, r2;
    for (i = 0; i < t.length; i++)
      r2 = t[i].flags, s2[i + 4] = r2.dependsOn << 4 | r2.isDependedOn << 2 | r2.hasRedundancy;
    return n7.box(n7.types.sdtp, s2);
  }
  static stbl(e) {
    return n7.box(n7.types.stbl, n7.stsd(e), n7.box(n7.types.stts, n7.STTS), n7.box(n7.types.stsc, n7.STSC), n7.box(n7.types.stsz, n7.STSZ), n7.box(n7.types.stco, n7.STCO));
  }
  static avc1(e) {
    let t = [], s2 = [], i, r2, a2;
    for (i = 0; i < e.sps.length; i++)
      r2 = e.sps[i], a2 = r2.byteLength, t.push(a2 >>> 8 & 255), t.push(a2 & 255), t = t.concat(Array.prototype.slice.call(r2));
    for (i = 0; i < e.pps.length; i++)
      r2 = e.pps[i], a2 = r2.byteLength, s2.push(a2 >>> 8 & 255), s2.push(a2 & 255), s2 = s2.concat(Array.prototype.slice.call(r2));
    let o = n7.box(n7.types.avcC, new Uint8Array([1, t[3], t[4], t[5], 255, 224 | e.sps.length].concat(t).concat([e.pps.length]).concat(s2))), l2 = e.width, c = e.height, h2 = e.pixelRatio[0], u2 = e.pixelRatio[1];
    return n7.box(n7.types.avc1, new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, l2 >> 8 & 255, l2 & 255, c >> 8 & 255, c & 255, 0, 72, 0, 0, 0, 72, 0, 0, 0, 0, 0, 0, 0, 1, 18, 100, 97, 105, 108, 121, 109, 111, 116, 105, 111, 110, 47, 104, 108, 115, 46, 106, 115, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 24, 17, 17]), o, n7.box(n7.types.btrt, new Uint8Array([0, 28, 156, 128, 0, 45, 198, 192, 0, 45, 198, 192])), n7.box(n7.types.pasp, new Uint8Array([h2 >> 24, h2 >> 16 & 255, h2 >> 8 & 255, h2 & 255, u2 >> 24, u2 >> 16 & 255, u2 >> 8 & 255, u2 & 255])));
  }
  static esds(e) {
    let t = e.config.length;
    return new Uint8Array([0, 0, 0, 0, 3, 23 + t, 0, 1, 0, 4, 15 + t, 64, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5].concat([t]).concat(e.config).concat([6, 1, 2]));
  }
  static audioStsd(e) {
    let t = e.samplerate;
    return new Uint8Array([0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, e.channelCount, 0, 16, 0, 0, 0, 0, t >> 8 & 255, t & 255, 0, 0]);
  }
  static mp4a(e) {
    return n7.box(n7.types.mp4a, n7.audioStsd(e), n7.box(n7.types.esds, n7.esds(e)));
  }
  static mp3(e) {
    return n7.box(n7.types[".mp3"], n7.audioStsd(e));
  }
  static ac3(e) {
    return n7.box(n7.types["ac-3"], n7.audioStsd(e), n7.box(n7.types.dac3, e.config));
  }
  static stsd(e) {
    return e.type === "audio" ? e.segmentCodec === "mp3" && e.codec === "mp3" ? n7.box(n7.types.stsd, n7.STSD, n7.mp3(e)) : e.segmentCodec === "ac3" ? n7.box(n7.types.stsd, n7.STSD, n7.ac3(e)) : n7.box(n7.types.stsd, n7.STSD, n7.mp4a(e)) : n7.box(n7.types.stsd, n7.STSD, n7.avc1(e));
  }
  static tkhd(e) {
    let t = e.id, s2 = e.duration * e.timescale, i = e.width, r2 = e.height, a2 = Math.floor(s2 / (we3 + 1)), o = Math.floor(s2 % (we3 + 1));
    return n7.box(n7.types.tkhd, new Uint8Array([1, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 3, t >> 24 & 255, t >> 16 & 255, t >> 8 & 255, t & 255, 0, 0, 0, 0, a2 >> 24, a2 >> 16 & 255, a2 >> 8 & 255, a2 & 255, o >> 24, o >> 16 & 255, o >> 8 & 255, o & 255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 64, 0, 0, 0, i >> 8 & 255, i & 255, 0, 0, r2 >> 8 & 255, r2 & 255, 0, 0]));
  }
  static traf(e, t) {
    let s2 = n7.sdtp(e), i = e.id, r2 = Math.floor(t / (we3 + 1)), a2 = Math.floor(t % (we3 + 1));
    return n7.box(n7.types.traf, n7.box(n7.types.tfhd, new Uint8Array([0, 0, 0, 0, i >> 24, i >> 16 & 255, i >> 8 & 255, i & 255])), n7.box(n7.types.tfdt, new Uint8Array([1, 0, 0, 0, r2 >> 24, r2 >> 16 & 255, r2 >> 8 & 255, r2 & 255, a2 >> 24, a2 >> 16 & 255, a2 >> 8 & 255, a2 & 255])), n7.trun(e, s2.length + 16 + 20 + 8 + 16 + 8 + 8), s2);
  }
  static trak(e) {
    return e.duration = e.duration || 4294967295, n7.box(n7.types.trak, n7.tkhd(e), n7.mdia(e));
  }
  static trex(e) {
    let t = e.id;
    return n7.box(n7.types.trex, new Uint8Array([0, 0, 0, 0, t >> 24, t >> 16 & 255, t >> 8 & 255, t & 255, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 1]));
  }
  static trun(e, t) {
    let s2 = e.samples || [], i = s2.length, r2 = 12 + 16 * i, a2 = new Uint8Array(r2), o, l2, c, h2, u2, d3;
    for (t += 8 + r2, a2.set([e.type === "video" ? 1 : 0, 0, 15, 1, i >>> 24 & 255, i >>> 16 & 255, i >>> 8 & 255, i & 255, t >>> 24 & 255, t >>> 16 & 255, t >>> 8 & 255, t & 255], 0), o = 0; o < i; o++)
      l2 = s2[o], c = l2.duration, h2 = l2.size, u2 = l2.flags, d3 = l2.cts, a2.set([c >>> 24 & 255, c >>> 16 & 255, c >>> 8 & 255, c & 255, h2 >>> 24 & 255, h2 >>> 16 & 255, h2 >>> 8 & 255, h2 & 255, u2.isLeading << 2 | u2.dependsOn, u2.isDependedOn << 6 | u2.hasRedundancy << 4 | u2.paddingValue << 1 | u2.isNonSync, u2.degradPrio & 61440, u2.degradPrio & 15, d3 >>> 24 & 255, d3 >>> 16 & 255, d3 >>> 8 & 255, d3 & 255], 12 + 16 * o);
    return n7.box(n7.types.trun, a2);
  }
  static initSegment(e) {
    n7.types || n7.init();
    let t = n7.moov(e);
    return pe3(n7.FTYP, t);
  }
};
ie2.types = void 0;
ie2.HDLR_TYPES = void 0;
ie2.STTS = void 0;
ie2.STSC = void 0;
ie2.STCO = void 0;
ie2.STSZ = void 0;
ie2.VMHD = void 0;
ie2.SMHD = void 0;
ie2.STSD = void 0;
ie2.FTYP = void 0;
ie2.DINF = void 0;
var Sn = 9e4;
function Ni(n12, e, t = 1, s2 = false) {
  let i = n12 * e * t;
  return s2 ? Math.round(i) : i;
}
function Go(n12, e, t = 1, s2 = false) {
  return Ni(n12, e, 1 / t, s2);
}
function Je(n12, e = false) {
  return Ni(n12, 1e3, 1 / Sn, e);
}
function Ko(n12, e = 1) {
  return Ni(n12, Sn, 1 / e);
}
var Ho = 10 * 1e3;
var xr = 1024;
var Vo = 1152;
var Wo = 1536;
var Ke = null;
var us = null;
var qe2 = class {
  constructor(e, t, s2, i = "") {
    if (this.observer = void 0, this.config = void 0, this.typeSupported = void 0, this.ISGenerated = false, this._initPTS = null, this._initDTS = null, this.nextAvcDts = null, this.nextAudioPts = null, this.videoSampleDuration = null, this.isAudioContiguous = false, this.isVideoContiguous = false, this.videoTrackConfig = void 0, this.observer = e, this.config = t, this.typeSupported = s2, this.ISGenerated = false, Ke === null) {
      let a2 = (navigator.userAgent || "").match(/Chrome\/(\d+)/i);
      Ke = a2 ? parseInt(a2[1]) : 0;
    }
    if (us === null) {
      let r2 = navigator.userAgent.match(/Safari\/(\d+)/i);
      us = r2 ? parseInt(r2[1]) : 0;
    }
  }
  destroy() {
    this.config = this.videoTrackConfig = this._initPTS = this._initDTS = null;
  }
  resetTimeStamp(e) {
    v2.log("[mp4-remuxer]: initPTS & initDTS reset"), this._initPTS = this._initDTS = e;
  }
  resetNextTimestamp() {
    v2.log("[mp4-remuxer]: reset next timestamp"), this.isVideoContiguous = false, this.isAudioContiguous = false;
  }
  resetInitSegment() {
    v2.log("[mp4-remuxer]: ISGenerated flag reset"), this.ISGenerated = false, this.videoTrackConfig = void 0;
  }
  getVideoStartPts(e) {
    let t = false, s2 = e.reduce((i, r2) => {
      let a2 = r2.pts - i;
      return a2 < -4294967296 ? (t = true, me3(i, r2.pts)) : a2 > 0 ? i : r2.pts;
    }, e[0].pts);
    return t && v2.debug("PTS rollover detected"), s2;
  }
  remux(e, t, s2, i, r2, a2, o, l2) {
    let c, h2, u2, d3, f3, g2, p3 = r2, T2 = r2, E4 = e.pid > -1, x3 = t.pid > -1, y3 = t.samples.length, R = e.samples.length > 0, S2 = o && y3 > 0 || y3 > 1;
    if ((!E4 || R) && (!x3 || S2) || this.ISGenerated || o) {
      if (this.ISGenerated) {
        var L, C2, _, I;
        let G2 = this.videoTrackConfig;
        G2 && (t.width !== G2.width || t.height !== G2.height || ((L = t.pixelRatio) == null ? void 0 : L[0]) !== ((C2 = G2.pixelRatio) == null ? void 0 : C2[0]) || ((_ = t.pixelRatio) == null ? void 0 : _[1]) !== ((I = G2.pixelRatio) == null ? void 0 : I[1])) && this.resetInitSegment();
      } else
        u2 = this.generateIS(e, t, r2, a2);
      let w = this.isVideoContiguous, K = -1, P2;
      if (S2 && (K = Yo(t.samples), !w && this.config.forceKeyFrameOnDiscontinuity))
        if (g2 = true, K > 0) {
          v2.warn(`[mp4-remuxer]: Dropped ${K} out of ${y3} video samples due to a missing keyframe`);
          let G2 = this.getVideoStartPts(t.samples);
          t.samples = t.samples.slice(K), t.dropped += K, T2 += (t.samples[0].pts - G2) / t.inputTimeScale, P2 = T2;
        } else
          K === -1 && (v2.warn(`[mp4-remuxer]: No keyframe found out of ${y3} video samples`), g2 = false);
      if (this.ISGenerated) {
        if (R && S2) {
          let G2 = this.getVideoStartPts(t.samples), U3 = (me3(e.samples[0].pts, G2) - G2) / t.inputTimeScale;
          p3 += Math.max(0, U3), T2 += Math.max(0, -U3);
        }
        if (R) {
          if (e.samplerate || (v2.warn("[mp4-remuxer]: regenerate InitSegment as audio detected"), u2 = this.generateIS(e, t, r2, a2)), h2 = this.remuxAudio(e, p3, this.isAudioContiguous, a2, x3 || S2 || l2 === N2.AUDIO ? T2 : void 0), S2) {
            let G2 = h2 ? h2.endPTS - h2.startPTS : 0;
            t.inputTimeScale || (v2.warn("[mp4-remuxer]: regenerate InitSegment as video detected"), u2 = this.generateIS(e, t, r2, a2)), c = this.remuxVideo(t, T2, w, G2);
          }
        } else
          S2 && (c = this.remuxVideo(t, T2, w, 0));
        c && (c.firstKeyFrame = K, c.independent = K !== -1, c.firstKeyFramePTS = P2);
      }
    }
    return this.ISGenerated && this._initPTS && this._initDTS && (s2.samples.length && (f3 = vn(s2, r2, this._initPTS, this._initDTS)), i.samples.length && (d3 = An(i, r2, this._initPTS))), { audio: h2, video: c, initSegment: u2, independent: g2, text: d3, id3: f3 };
  }
  generateIS(e, t, s2, i) {
    let r2 = e.samples, a2 = t.samples, o = this.typeSupported, l2 = {}, c = this._initPTS, h2 = !c || i, u2 = "audio/mp4", d3, f3, g2;
    if (h2 && (d3 = f3 = 1 / 0), e.config && r2.length) {
      switch (e.timescale = e.samplerate, e.segmentCodec) {
        case "mp3":
          o.mpeg ? (u2 = "audio/mpeg", e.codec = "") : o.mp3 && (e.codec = "mp3");
          break;
        case "ac3":
          e.codec = "ac-3";
          break;
      }
      l2.audio = { id: "audio", container: u2, codec: e.codec, initSegment: e.segmentCodec === "mp3" && o.mpeg ? new Uint8Array(0) : ie2.initSegment([e]), metadata: { channelCount: e.channelCount } }, h2 && (g2 = e.inputTimeScale, !c || g2 !== c.timescale ? d3 = f3 = r2[0].pts - Math.round(g2 * s2) : h2 = false);
    }
    if (t.sps && t.pps && a2.length) {
      if (t.timescale = t.inputTimeScale, l2.video = { id: "main", container: "video/mp4", codec: t.codec, initSegment: ie2.initSegment([t]), metadata: { width: t.width, height: t.height } }, h2)
        if (g2 = t.inputTimeScale, !c || g2 !== c.timescale) {
          let p3 = this.getVideoStartPts(a2), T2 = Math.round(g2 * s2);
          f3 = Math.min(f3, me3(a2[0].dts, p3) - T2), d3 = Math.min(d3, p3 - T2);
        } else
          h2 = false;
      this.videoTrackConfig = { width: t.width, height: t.height, pixelRatio: t.pixelRatio };
    }
    if (Object.keys(l2).length)
      return this.ISGenerated = true, h2 ? (this._initPTS = { baseTime: d3, timescale: g2 }, this._initDTS = { baseTime: f3, timescale: g2 }) : d3 = g2 = void 0, { tracks: l2, initPTS: d3, timescale: g2 };
  }
  remuxVideo(e, t, s2, i) {
    let r2 = e.inputTimeScale, a2 = e.samples, o = [], l2 = a2.length, c = this._initPTS, h2 = this.nextAvcDts, u2 = 8, d3 = this.videoSampleDuration, f3, g2, p3 = Number.POSITIVE_INFINITY, T2 = Number.NEGATIVE_INFINITY, E4 = false;
    if (!s2 || h2 === null) {
      let M2 = t * r2, k = a2[0].pts - me3(a2[0].dts, a2[0].pts);
      Ke && h2 !== null && Math.abs(M2 - k - h2) < 15e3 ? s2 = true : h2 = M2 - k;
    }
    let x3 = c.baseTime * r2 / c.timescale;
    for (let M2 = 0; M2 < l2; M2++) {
      let k = a2[M2];
      k.pts = me3(k.pts - x3, h2), k.dts = me3(k.dts - x3, h2), k.dts < a2[M2 > 0 ? M2 - 1 : M2].dts && (E4 = true);
    }
    E4 && a2.sort(function(M2, k) {
      let q = M2.dts - k.dts, V = M2.pts - k.pts;
      return q || V;
    }), f3 = a2[0].dts, g2 = a2[a2.length - 1].dts;
    let y3 = g2 - f3, R = y3 ? Math.round(y3 / (l2 - 1)) : d3 || e.inputTimeScale / 30;
    if (s2) {
      let M2 = f3 - h2, k = M2 > R, q = M2 < -1;
      if ((k || q) && (k ? v2.warn(`AVC: ${Je(M2, true)} ms (${M2}dts) hole between fragments detected at ${t.toFixed(3)}`) : v2.warn(`AVC: ${Je(-M2, true)} ms (${M2}dts) overlapping between fragments detected at ${t.toFixed(3)}`), !q || h2 >= a2[0].pts || Ke)) {
        f3 = h2;
        let V = a2[0].pts - M2;
        if (k)
          a2[0].dts = f3, a2[0].pts = V;
        else
          for (let j = 0; j < a2.length && !(a2[j].dts > V); j++)
            a2[j].dts -= M2, a2[j].pts -= M2;
        v2.log(`Video: Initial PTS/DTS adjusted: ${Je(V, true)}/${Je(f3, true)}, delta: ${Je(M2, true)} ms`);
      }
    }
    f3 = Math.max(0, f3);
    let S2 = 0, b = 0, L = f3;
    for (let M2 = 0; M2 < l2; M2++) {
      let k = a2[M2], q = k.units, V = q.length, j = 0;
      for (let ee2 = 0; ee2 < V; ee2++)
        j += q[ee2].data.length;
      b += j, S2 += V, k.length = j, k.dts < L ? (k.dts = L, L += R / 4 | 0 || 1) : L = k.dts, p3 = Math.min(k.pts, p3), T2 = Math.max(k.pts, T2);
    }
    g2 = a2[l2 - 1].dts;
    let C2 = b + 4 * S2 + 8, _;
    try {
      _ = new Uint8Array(C2);
    } catch (M2) {
      this.observer.emit(m2.ERROR, m2.ERROR, { type: B2.MUX_ERROR, details: A2.REMUX_ALLOC_ERROR, fatal: false, error: M2, bytes: C2, reason: `fail allocating video mdat ${C2}` });
      return;
    }
    let I = new DataView(_.buffer);
    I.setUint32(0, C2), _.set(ie2.types.mdat, 4);
    let w = false, K = Number.POSITIVE_INFINITY, P2 = Number.POSITIVE_INFINITY, G2 = Number.NEGATIVE_INFINITY, $3 = Number.NEGATIVE_INFINITY;
    for (let M2 = 0; M2 < l2; M2++) {
      let k = a2[M2], q = k.units, V = 0;
      for (let re2 = 0, ce2 = q.length; re2 < ce2; re2++) {
        let ge3 = q[re2], Qe = ge3.data, es = ge3.data.byteLength;
        I.setUint32(u2, es), u2 += 4, _.set(Qe, u2), u2 += es, V += 4 + es;
      }
      let j;
      if (M2 < l2 - 1)
        d3 = a2[M2 + 1].dts - k.dts, j = a2[M2 + 1].pts - k.pts;
      else {
        let re2 = this.config, ce2 = M2 > 0 ? k.dts - a2[M2 - 1].dts : R;
        if (j = M2 > 0 ? k.pts - a2[M2 - 1].pts : R, re2.stretchShortVideoTrack && this.nextAudioPts !== null) {
          let ge3 = Math.floor(re2.maxBufferHole * r2), Qe = (i ? p3 + i * r2 : this.nextAudioPts) - k.pts;
          Qe > ge3 ? (d3 = Qe - ce2, d3 < 0 ? d3 = ce2 : w = true, v2.log(`[mp4-remuxer]: It is approximately ${Qe / 90} ms to the next segment; using duration ${d3 / 90} ms for the last video frame.`)) : d3 = ce2;
        } else
          d3 = ce2;
      }
      let ee2 = Math.round(k.pts - k.dts);
      K = Math.min(K, d3), G2 = Math.max(G2, d3), P2 = Math.min(P2, j), $3 = Math.max($3, j), o.push(new Ht(k.key, d3, V, ee2));
    }
    if (o.length) {
      if (Ke) {
        if (Ke < 70) {
          let M2 = o[0].flags;
          M2.dependsOn = 2, M2.isNonSync = 0;
        }
      } else if (us && $3 - P2 < G2 - K && R / G2 < 0.025 && o[0].cts === 0) {
        v2.warn("Found irregular gaps in sample duration. Using PTS instead of DTS to determine MP4 sample duration.");
        let M2 = f3;
        for (let k = 0, q = o.length; k < q; k++) {
          let V = M2 + o[k].duration, j = M2 + o[k].cts;
          if (k < q - 1) {
            let ee2 = V + o[k + 1].cts;
            o[k].duration = ee2 - j;
          } else
            o[k].duration = k ? o[k - 1].duration : R;
          o[k].cts = 0, M2 = V;
        }
      }
    }
    d3 = w || !d3 ? R : d3, this.nextAvcDts = h2 = g2 + d3, this.videoSampleDuration = d3, this.isVideoContiguous = true;
    let X = { data1: ie2.moof(e.sequenceNumber++, f3, te2({}, e, { samples: o })), data2: _, startPTS: p3 / r2, endPTS: (T2 + d3) / r2, startDTS: f3 / r2, endDTS: h2 / r2, type: "video", hasAudio: false, hasVideo: true, nb: o.length, dropped: e.dropped };
    return e.samples = [], e.dropped = 0, X;
  }
  getSamplesPerFrame(e) {
    switch (e.segmentCodec) {
      case "mp3":
        return Vo;
      case "ac3":
        return Wo;
      default:
        return xr;
    }
  }
  remuxAudio(e, t, s2, i, r2) {
    let a2 = e.inputTimeScale, o = e.samplerate ? e.samplerate : a2, l2 = a2 / o, c = this.getSamplesPerFrame(e), h2 = c * l2, u2 = this._initPTS, d3 = e.segmentCodec === "mp3" && this.typeSupported.mpeg, f3 = [], g2 = r2 !== void 0, p3 = e.samples, T2 = d3 ? 0 : 8, E4 = this.nextAudioPts || -1, x3 = t * a2, y3 = u2.baseTime * a2 / u2.timescale;
    if (this.isAudioContiguous = s2 = s2 || p3.length && E4 > 0 && (i && Math.abs(x3 - E4) < 9e3 || Math.abs(me3(p3[0].pts - y3, x3) - E4) < 20 * h2), p3.forEach(function(U3) {
      U3.pts = me3(U3.pts - y3, x3);
    }), !s2 || E4 < 0) {
      if (p3 = p3.filter((U3) => U3.pts >= 0), !p3.length)
        return;
      r2 === 0 ? E4 = 0 : i && !g2 ? E4 = Math.max(0, x3) : E4 = p3[0].pts;
    }
    if (e.segmentCodec === "aac") {
      let U3 = this.config.maxAudioFramesDrift;
      for (let Y3 = 0, X = E4; Y3 < p3.length; Y3++) {
        let M2 = p3[Y3], k = M2.pts, q = k - X, V = Math.abs(1e3 * q / a2);
        if (q <= -U3 * h2 && g2)
          Y3 === 0 && (v2.warn(`Audio frame @ ${(k / a2).toFixed(3)}s overlaps nextAudioPts by ${Math.round(1e3 * q / a2)} ms.`), this.nextAudioPts = E4 = X = k);
        else if (q >= U3 * h2 && V < Ho && g2) {
          let j = Math.round(q / h2);
          X = k - j * h2, X < 0 && (j--, X += h2), Y3 === 0 && (this.nextAudioPts = E4 = X), v2.warn(`[mp4-remuxer]: Injecting ${j} audio frame @ ${(X / a2).toFixed(3)}s due to ${Math.round(1e3 * q / a2)} ms gap.`);
          for (let ee2 = 0; ee2 < j; ee2++) {
            let re2 = Math.max(X, 0), ce2 = Kt.getSilentFrame(e.manifestCodec || e.codec, e.channelCount);
            ce2 || (v2.log("[mp4-remuxer]: Unable to get silent frame for given audio codec; duplicating last frame instead."), ce2 = M2.unit.subarray()), p3.splice(Y3, 0, { unit: ce2, pts: re2 }), X += h2, Y3++;
          }
        }
        M2.pts = X, X += h2;
      }
    }
    let R = null, S2 = null, b, L = 0, C2 = p3.length;
    for (; C2--; )
      L += p3[C2].unit.byteLength;
    for (let U3 = 0, Y3 = p3.length; U3 < Y3; U3++) {
      let X = p3[U3], M2 = X.unit, k = X.pts;
      if (S2 !== null) {
        let V = f3[U3 - 1];
        V.duration = Math.round((k - S2) / l2);
      } else if (s2 && e.segmentCodec === "aac" && (k = E4), R = k, L > 0) {
        L += T2;
        try {
          b = new Uint8Array(L);
        } catch (V) {
          this.observer.emit(m2.ERROR, m2.ERROR, { type: B2.MUX_ERROR, details: A2.REMUX_ALLOC_ERROR, fatal: false, error: V, bytes: L, reason: `fail allocating audio mdat ${L}` });
          return;
        }
        d3 || (new DataView(b.buffer).setUint32(0, L), b.set(ie2.types.mdat, 4));
      } else
        return;
      b.set(M2, T2);
      let q = M2.byteLength;
      T2 += q, f3.push(new Ht(true, c, q, 0)), S2 = k;
    }
    let _ = f3.length;
    if (!_)
      return;
    let I = f3[f3.length - 1];
    this.nextAudioPts = E4 = S2 + l2 * I.duration;
    let w = d3 ? new Uint8Array(0) : ie2.moof(e.sequenceNumber++, R / l2, te2({}, e, { samples: f3 }));
    e.samples = [];
    let K = R / a2, P2 = E4 / a2, $3 = { data1: w, data2: b, startPTS: K, endPTS: P2, startDTS: K, endDTS: P2, type: "audio", hasAudio: true, hasVideo: false, nb: _ };
    return this.isAudioContiguous = true, $3;
  }
  remuxEmptyAudio(e, t, s2, i) {
    let r2 = e.inputTimeScale, a2 = e.samplerate ? e.samplerate : r2, o = r2 / a2, l2 = this.nextAudioPts, c = this._initDTS, h2 = c.baseTime * 9e4 / c.timescale, u2 = (l2 !== null ? l2 : i.startDTS * r2) + h2, d3 = i.endDTS * r2 + h2, f3 = o * xr, g2 = Math.ceil((d3 - u2) / f3), p3 = Kt.getSilentFrame(e.manifestCodec || e.codec, e.channelCount);
    if (v2.warn("[mp4-remuxer]: remux empty Audio"), !p3) {
      v2.trace("[mp4-remuxer]: Unable to remuxEmptyAudio since we were unable to get a silent frame for given audio codec");
      return;
    }
    let T2 = [];
    for (let E4 = 0; E4 < g2; E4++) {
      let x3 = u2 + E4 * f3;
      T2.push({ unit: p3, pts: x3, dts: x3 });
    }
    return e.samples = T2, this.remuxAudio(e, t, s2, false);
  }
};
function me3(n12, e) {
  let t;
  if (e === null)
    return n12;
  for (e < n12 ? t = -8589934592 : t = 8589934592; Math.abs(n12 - e) > 4294967296; )
    n12 += t;
  return n12;
}
function Yo(n12) {
  for (let e = 0; e < n12.length; e++)
    if (n12[e].key)
      return e;
  return -1;
}
function vn(n12, e, t, s2) {
  let i = n12.samples.length;
  if (!i)
    return;
  let r2 = n12.inputTimeScale;
  for (let o = 0; o < i; o++) {
    let l2 = n12.samples[o];
    l2.pts = me3(l2.pts - t.baseTime * r2 / t.timescale, e * r2) / r2, l2.dts = me3(l2.dts - s2.baseTime * r2 / s2.timescale, e * r2) / r2;
  }
  let a2 = n12.samples;
  return n12.samples = [], { samples: a2 };
}
function An(n12, e, t) {
  let s2 = n12.samples.length;
  if (!s2)
    return;
  let i = n12.inputTimeScale;
  for (let a2 = 0; a2 < s2; a2++) {
    let o = n12.samples[a2];
    o.pts = me3(o.pts - t.baseTime * i / t.timescale, e * i) / i;
  }
  n12.samples.sort((a2, o) => a2.pts - o.pts);
  let r2 = n12.samples;
  return n12.samples = [], { samples: r2 };
}
var Ht = class {
  constructor(e, t, s2, i) {
    this.size = void 0, this.duration = void 0, this.cts = void 0, this.flags = void 0, this.duration = t, this.size = s2, this.cts = i, this.flags = { isLeading: 0, isDependedOn: 0, hasRedundancy: 0, degradPrio: 0, dependsOn: e ? 2 : 1, isNonSync: e ? 0 : 1 };
  }
};
var Qs = class {
  constructor() {
    this.emitInitSegment = false, this.audioCodec = void 0, this.videoCodec = void 0, this.initData = void 0, this.initPTS = null, this.initTracks = void 0, this.lastEndTime = null;
  }
  destroy() {
  }
  resetTimeStamp(e) {
    this.initPTS = e, this.lastEndTime = null;
  }
  resetNextTimestamp() {
    this.lastEndTime = null;
  }
  resetInitSegment(e, t, s2, i) {
    this.audioCodec = t, this.videoCodec = s2, this.generateInitSegment(ya2(e, i)), this.emitInitSegment = true;
  }
  generateInitSegment(e) {
    let { audioCodec: t, videoCodec: s2 } = this;
    if (!(e != null && e.byteLength)) {
      this.initTracks = void 0, this.initData = void 0;
      return;
    }
    let i = this.initData = jr(e);
    i.audio && (t = Sr(i.audio, z2.AUDIO)), i.video && (s2 = Sr(i.video, z2.VIDEO));
    let r2 = {};
    i.audio && i.video ? r2.audiovideo = { container: "video/mp4", codec: t + "," + s2, initSegment: e, id: "main" } : i.audio ? r2.audio = { container: "audio/mp4", codec: t, initSegment: e, id: "audio" } : i.video ? r2.video = { container: "video/mp4", codec: s2, initSegment: e, id: "main" } : v2.warn("[passthrough-remuxer.ts]: initSegment does not contain moov or trak boxes."), this.initTracks = r2;
  }
  remux(e, t, s2, i, r2, a2) {
    var o, l2;
    let { initPTS: c, lastEndTime: h2 } = this, u2 = { audio: void 0, video: void 0, text: i, id3: s2, initSegment: void 0 };
    F(h2) || (h2 = this.lastEndTime = r2 || 0);
    let d3 = t.samples;
    if (!(d3 != null && d3.length))
      return u2;
    let f3 = { initPTS: void 0, timescale: 1 }, g2 = this.initData;
    if ((o = g2) != null && o.length || (this.generateInitSegment(d3), g2 = this.initData), !((l2 = g2) != null && l2.length))
      return v2.warn("[passthrough-remuxer.ts]: Failed to generate initSegment."), u2;
    this.emitInitSegment && (f3.tracks = this.initTracks, this.emitInitSegment = false);
    let p3 = Sa2(d3, g2), T2 = xa2(g2, d3), E4 = T2 === null ? r2 : T2;
    (qo(c, E4, r2, p3) || f3.timescale !== c.timescale && a2) && (f3.initPTS = E4 - r2, c && c.timescale === 1 && v2.warn(`Adjusting initPTS by ${f3.initPTS - c.baseTime}`), this.initPTS = c = { baseTime: f3.initPTS, timescale: 1 });
    let x3 = e ? E4 - c.baseTime / c.timescale : h2, y3 = x3 + p3;
    Aa(g2, d3, c.baseTime / c.timescale), p3 > 0 ? this.lastEndTime = y3 : (v2.warn("Duration parsed from mp4 should be greater than zero"), this.resetNextTimestamp());
    let R = !!g2.audio, S2 = !!g2.video, b = "";
    R && (b += "audio"), S2 && (b += "video");
    let L = { data1: d3, startPTS: x3, startDTS: x3, endPTS: y3, endDTS: y3, type: b, hasAudio: R, hasVideo: S2, nb: 1, dropped: 0 };
    return u2.audio = L.type === "audio" ? L : void 0, u2.video = L.type !== "audio" ? L : void 0, u2.initSegment = f3, u2.id3 = vn(s2, r2, c, c), i.samples.length && (u2.text = An(i, r2, c)), u2;
  }
};
function qo(n12, e, t, s2) {
  if (n12 === null)
    return true;
  let i = Math.max(s2, 1), r2 = e - n12.baseTime / n12.timescale;
  return Math.abs(r2 - t) > i;
}
function Sr(n12, e) {
  let t = n12?.codec;
  if (t && t.length > 4)
    return t;
  if (e === z2.AUDIO) {
    if (t === "ec-3" || t === "ac-3" || t === "alac")
      return t;
    if (t === "fLaC" || t === "Opus")
      return Pt(t, false);
    let s2 = "mp4a.40.5";
    return v2.info(`Parsed audio codec "${t}" or audio object type not handled. Using "${s2}"`), s2;
  }
  return v2.warn(`Unhandled video codec "${t}"`), t === "hvc1" || t === "hev1" ? "hvc1.1.6.L120.90" : t === "av01" ? "av01.0.04M.08" : "avc1.42e01e";
}
var Ce3;
try {
  Ce3 = self.performance.now.bind(self.performance);
} catch {
  v2.debug("Unable to use Performance API on this environment"), Ce3 = ze2?.Date.now;
}
var At = [{ demux: Hs, remux: Qs }, { demux: js, remux: qe2 }, { demux: Ks, remux: qe2 }, { demux: Xs, remux: qe2 }];
At.splice(2, 0, { demux: Vs, remux: qe2 });
var Vt = class {
  constructor(e, t, s2, i, r2) {
    this.async = false, this.observer = void 0, this.typeSupported = void 0, this.config = void 0, this.vendor = void 0, this.id = void 0, this.demuxer = void 0, this.remuxer = void 0, this.decrypter = void 0, this.probe = void 0, this.decryptionPromise = null, this.transmuxConfig = void 0, this.currentTransmuxState = void 0, this.observer = e, this.typeSupported = t, this.config = s2, this.vendor = i, this.id = r2;
  }
  configure(e) {
    this.transmuxConfig = e, this.decrypter && this.decrypter.reset();
  }
  push(e, t, s2, i) {
    let r2 = s2.transmuxing;
    r2.executeStart = Ce3();
    let a2 = new Uint8Array(e), { currentTransmuxState: o, transmuxConfig: l2 } = this;
    i && (this.currentTransmuxState = i);
    let { contiguous: c, discontinuity: h2, trackSwitch: u2, accurateTimeOffset: d3, timeOffset: f3, initSegmentChange: g2 } = i || o, { audioCodec: p3, videoCodec: T2, defaultInitPts: E4, duration: x3, initSegmentData: y3 } = l2, R = jo(a2, t);
    if (R && R.method === "AES-128") {
      let C2 = this.getDecrypter();
      if (C2.isSync()) {
        let _ = C2.softwareDecrypt(a2, R.key.buffer, R.iv.buffer);
        if (s2.part > -1 && (_ = C2.flush()), !_)
          return r2.executeEnd = Ce3(), ds(s2);
        a2 = new Uint8Array(_);
      } else
        return this.decryptionPromise = C2.webCryptoDecrypt(a2, R.key.buffer, R.iv.buffer).then((_) => {
          let I = this.push(_, null, s2);
          return this.decryptionPromise = null, I;
        }), this.decryptionPromise;
    }
    let S2 = this.needsProbing(h2, u2);
    if (S2) {
      let C2 = this.configureTransmuxer(a2);
      if (C2)
        return v2.warn(`[transmuxer] ${C2.message}`), this.observer.emit(m2.ERROR, m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, fatal: false, error: C2, reason: C2.message }), r2.executeEnd = Ce3(), ds(s2);
    }
    (h2 || u2 || g2 || S2) && this.resetInitSegment(y3, p3, T2, x3, t), (h2 || g2 || S2) && this.resetInitialTimestamp(E4), c || this.resetContiguity();
    let b = this.transmux(a2, R, f3, d3, s2), L = this.currentTransmuxState;
    return L.contiguous = true, L.discontinuity = false, L.trackSwitch = false, r2.executeEnd = Ce3(), b;
  }
  flush(e) {
    let t = e.transmuxing;
    t.executeStart = Ce3();
    let { decrypter: s2, currentTransmuxState: i, decryptionPromise: r2 } = this;
    if (r2)
      return r2.then(() => this.flush(e));
    let a2 = [], { timeOffset: o } = i;
    if (s2) {
      let u2 = s2.flush();
      u2 && a2.push(this.push(u2, null, e));
    }
    let { demuxer: l2, remuxer: c } = this;
    if (!l2 || !c)
      return t.executeEnd = Ce3(), [ds(e)];
    let h2 = l2.flush(o);
    return Lt(h2) ? h2.then((u2) => (this.flushRemux(a2, u2, e), a2)) : (this.flushRemux(a2, h2, e), a2);
  }
  flushRemux(e, t, s2) {
    let { audioTrack: i, videoTrack: r2, id3Track: a2, textTrack: o } = t, { accurateTimeOffset: l2, timeOffset: c } = this.currentTransmuxState;
    v2.log(`[transmuxer.ts]: Flushed fragment ${s2.sn}${s2.part > -1 ? " p: " + s2.part : ""} of level ${s2.level}`);
    let h2 = this.remuxer.remux(i, r2, a2, o, c, l2, true, this.id);
    e.push({ remuxResult: h2, chunkMeta: s2 }), s2.transmuxing.executeEnd = Ce3();
  }
  resetInitialTimestamp(e) {
    let { demuxer: t, remuxer: s2 } = this;
    !t || !s2 || (t.resetTimeStamp(e), s2.resetTimeStamp(e));
  }
  resetContiguity() {
    let { demuxer: e, remuxer: t } = this;
    !e || !t || (e.resetContiguity(), t.resetNextTimestamp());
  }
  resetInitSegment(e, t, s2, i, r2) {
    let { demuxer: a2, remuxer: o } = this;
    !a2 || !o || (a2.resetInitSegment(e, t, s2, i), o.resetInitSegment(e, t, s2, r2));
  }
  destroy() {
    this.demuxer && (this.demuxer.destroy(), this.demuxer = void 0), this.remuxer && (this.remuxer.destroy(), this.remuxer = void 0);
  }
  transmux(e, t, s2, i, r2) {
    let a2;
    return t && t.method === "SAMPLE-AES" ? a2 = this.transmuxSampleAes(e, t, s2, i, r2) : a2 = this.transmuxUnencrypted(e, s2, i, r2), a2;
  }
  transmuxUnencrypted(e, t, s2, i) {
    let { audioTrack: r2, videoTrack: a2, id3Track: o, textTrack: l2 } = this.demuxer.demux(e, t, false, !this.config.progressive);
    return { remuxResult: this.remuxer.remux(r2, a2, o, l2, t, s2, false, this.id), chunkMeta: i };
  }
  transmuxSampleAes(e, t, s2, i, r2) {
    return this.demuxer.demuxSampleAes(e, t, s2).then((a2) => ({ remuxResult: this.remuxer.remux(a2.audioTrack, a2.videoTrack, a2.id3Track, a2.textTrack, s2, i, false, this.id), chunkMeta: r2 }));
  }
  configureTransmuxer(e) {
    let { config: t, observer: s2, typeSupported: i, vendor: r2 } = this, a2;
    for (let d3 = 0, f3 = At.length; d3 < f3; d3++) {
      var o;
      if ((o = At[d3].demux) != null && o.probe(e)) {
        a2 = At[d3];
        break;
      }
    }
    if (!a2)
      return new Error("Failed to find demuxer by probing fragment data");
    let l2 = this.demuxer, c = this.remuxer, h2 = a2.remux, u2 = a2.demux;
    (!c || !(c instanceof h2)) && (this.remuxer = new h2(s2, t, i, r2)), (!l2 || !(l2 instanceof u2)) && (this.demuxer = new u2(s2, t, i), this.probe = u2.probe);
  }
  needsProbing(e, t) {
    return !this.demuxer || !this.remuxer || e || t;
  }
  getDecrypter() {
    let e = this.decrypter;
    return e || (e = this.decrypter = new ot(this.config)), e;
  }
};
function jo(n12, e) {
  let t = null;
  return n12.byteLength > 0 && e?.key != null && e.iv !== null && e.method != null && (t = e), t;
}
var ds = (n12) => ({ remuxResult: {}, chunkMeta: n12 });
function Lt(n12) {
  return "then" in n12 && n12.then instanceof Function;
}
var Js = class {
  constructor(e, t, s2, i, r2) {
    this.audioCodec = void 0, this.videoCodec = void 0, this.initSegmentData = void 0, this.duration = void 0, this.defaultInitPts = void 0, this.audioCodec = e, this.videoCodec = t, this.initSegmentData = s2, this.duration = i, this.defaultInitPts = r2 || null;
  }
};
var Zs = class {
  constructor(e, t, s2, i, r2, a2) {
    this.discontinuity = void 0, this.contiguous = void 0, this.accurateTimeOffset = void 0, this.trackSwitch = void 0, this.timeOffset = void 0, this.initSegmentChange = void 0, this.discontinuity = e, this.contiguous = t, this.accurateTimeOffset = s2, this.trackSwitch = i, this.timeOffset = r2, this.initSegmentChange = a2;
  }
};
var Ln = { exports: {} };
(function(n12) {
  var e = Object.prototype.hasOwnProperty, t = "~";
  function s2() {
  }
  Object.create && (s2.prototype = /* @__PURE__ */ Object.create(null), new s2().__proto__ || (t = false));
  function i(l2, c, h2) {
    this.fn = l2, this.context = c, this.once = h2 || false;
  }
  function r2(l2, c, h2, u2, d3) {
    if (typeof h2 != "function")
      throw new TypeError("The listener must be a function");
    var f3 = new i(h2, u2 || l2, d3), g2 = t ? t + c : c;
    return l2._events[g2] ? l2._events[g2].fn ? l2._events[g2] = [l2._events[g2], f3] : l2._events[g2].push(f3) : (l2._events[g2] = f3, l2._eventsCount++), l2;
  }
  function a2(l2, c) {
    --l2._eventsCount === 0 ? l2._events = new s2() : delete l2._events[c];
  }
  function o() {
    this._events = new s2(), this._eventsCount = 0;
  }
  o.prototype.eventNames = function() {
    var c = [], h2, u2;
    if (this._eventsCount === 0)
      return c;
    for (u2 in h2 = this._events)
      e.call(h2, u2) && c.push(t ? u2.slice(1) : u2);
    return Object.getOwnPropertySymbols ? c.concat(Object.getOwnPropertySymbols(h2)) : c;
  }, o.prototype.listeners = function(c) {
    var h2 = t ? t + c : c, u2 = this._events[h2];
    if (!u2)
      return [];
    if (u2.fn)
      return [u2.fn];
    for (var d3 = 0, f3 = u2.length, g2 = new Array(f3); d3 < f3; d3++)
      g2[d3] = u2[d3].fn;
    return g2;
  }, o.prototype.listenerCount = function(c) {
    var h2 = t ? t + c : c, u2 = this._events[h2];
    return u2 ? u2.fn ? 1 : u2.length : 0;
  }, o.prototype.emit = function(c, h2, u2, d3, f3, g2) {
    var p3 = t ? t + c : c;
    if (!this._events[p3])
      return false;
    var T2 = this._events[p3], E4 = arguments.length, x3, y3;
    if (T2.fn) {
      switch (T2.once && this.removeListener(c, T2.fn, void 0, true), E4) {
        case 1:
          return T2.fn.call(T2.context), true;
        case 2:
          return T2.fn.call(T2.context, h2), true;
        case 3:
          return T2.fn.call(T2.context, h2, u2), true;
        case 4:
          return T2.fn.call(T2.context, h2, u2, d3), true;
        case 5:
          return T2.fn.call(T2.context, h2, u2, d3, f3), true;
        case 6:
          return T2.fn.call(T2.context, h2, u2, d3, f3, g2), true;
      }
      for (y3 = 1, x3 = new Array(E4 - 1); y3 < E4; y3++)
        x3[y3 - 1] = arguments[y3];
      T2.fn.apply(T2.context, x3);
    } else {
      var R = T2.length, S2;
      for (y3 = 0; y3 < R; y3++)
        switch (T2[y3].once && this.removeListener(c, T2[y3].fn, void 0, true), E4) {
          case 1:
            T2[y3].fn.call(T2[y3].context);
            break;
          case 2:
            T2[y3].fn.call(T2[y3].context, h2);
            break;
          case 3:
            T2[y3].fn.call(T2[y3].context, h2, u2);
            break;
          case 4:
            T2[y3].fn.call(T2[y3].context, h2, u2, d3);
            break;
          default:
            if (!x3)
              for (S2 = 1, x3 = new Array(E4 - 1); S2 < E4; S2++)
                x3[S2 - 1] = arguments[S2];
            T2[y3].fn.apply(T2[y3].context, x3);
        }
    }
    return true;
  }, o.prototype.on = function(c, h2, u2) {
    return r2(this, c, h2, u2, false);
  }, o.prototype.once = function(c, h2, u2) {
    return r2(this, c, h2, u2, true);
  }, o.prototype.removeListener = function(c, h2, u2, d3) {
    var f3 = t ? t + c : c;
    if (!this._events[f3])
      return this;
    if (!h2)
      return a2(this, f3), this;
    var g2 = this._events[f3];
    if (g2.fn)
      g2.fn === h2 && (!d3 || g2.once) && (!u2 || g2.context === u2) && a2(this, f3);
    else {
      for (var p3 = 0, T2 = [], E4 = g2.length; p3 < E4; p3++)
        (g2[p3].fn !== h2 || d3 && !g2[p3].once || u2 && g2[p3].context !== u2) && T2.push(g2[p3]);
      T2.length ? this._events[f3] = T2.length === 1 ? T2[0] : T2 : a2(this, f3);
    }
    return this;
  }, o.prototype.removeAllListeners = function(c) {
    var h2;
    return c ? (h2 = t ? t + c : c, this._events[h2] && a2(this, h2)) : (this._events = new s2(), this._eventsCount = 0), this;
  }, o.prototype.off = o.prototype.removeListener, o.prototype.addListener = o.prototype.on, o.prefixed = t, o.EventEmitter = o, n12.exports = o;
})(Ln);
var zo = Ln.exports;
var Ui = Kn(zo);
var Wt = class {
  constructor(e, t, s2, i) {
    this.error = null, this.hls = void 0, this.id = void 0, this.observer = void 0, this.frag = null, this.part = null, this.useWorker = void 0, this.workerContext = null, this.onwmsg = void 0, this.transmuxer = null, this.onTransmuxComplete = void 0, this.onFlush = void 0;
    let r2 = e.config;
    this.hls = e, this.id = t, this.useWorker = !!r2.enableWorker, this.onTransmuxComplete = s2, this.onFlush = i;
    let a2 = (h2, u2) => {
      u2 = u2 || {}, u2.frag = this.frag, u2.id = this.id, h2 === m2.ERROR && (this.error = u2.error), this.hls.trigger(h2, u2);
    };
    this.observer = new Ui(), this.observer.on(m2.FRAG_DECRYPTED, a2), this.observer.on(m2.ERROR, a2);
    let o = Ue2(r2.preferManagedMediaSource) || { isTypeSupported: () => false }, l2 = { mpeg: o.isTypeSupported("audio/mpeg"), mp3: o.isTypeSupported('audio/mp4; codecs="mp3"'), ac3: o.isTypeSupported('audio/mp4; codecs="ac-3"') }, c = navigator.vendor;
    if (this.useWorker && typeof Worker < "u" && (r2.workerPath || Lo())) {
      try {
        r2.workerPath ? (v2.log(`loading Web Worker ${r2.workerPath} for "${t}"`), this.workerContext = Io(r2.workerPath)) : (v2.log(`injecting Web Worker for "${t}"`), this.workerContext = Ro()), this.onwmsg = (d3) => this.onWorkerMessage(d3);
        let { worker: u2 } = this.workerContext;
        u2.addEventListener("message", this.onwmsg), u2.onerror = (d3) => {
          let f3 = new Error(`${d3.message}  (${d3.filename}:${d3.lineno})`);
          r2.enableWorker = false, v2.warn(`Error in "${t}" Web Worker, fallback to inline`), this.hls.trigger(m2.ERROR, { type: B2.OTHER_ERROR, details: A2.INTERNAL_EXCEPTION, fatal: false, event: "demuxerWorker", error: f3 });
        }, u2.postMessage({ cmd: "init", typeSupported: l2, vendor: c, id: t, config: JSON.stringify(r2) });
      } catch (u2) {
        v2.warn(`Error setting up "${t}" Web Worker, fallback to inline`, u2), this.resetWorker(), this.error = null, this.transmuxer = new Vt(this.observer, l2, r2, c, t);
      }
      return;
    }
    this.transmuxer = new Vt(this.observer, l2, r2, c, t);
  }
  resetWorker() {
    if (this.workerContext) {
      let { worker: e, objectURL: t } = this.workerContext;
      t && self.URL.revokeObjectURL(t), e.removeEventListener("message", this.onwmsg), e.onerror = null, e.terminate(), this.workerContext = null;
    }
  }
  destroy() {
    if (this.workerContext)
      this.resetWorker(), this.onwmsg = void 0;
    else {
      let t = this.transmuxer;
      t && (t.destroy(), this.transmuxer = null);
    }
    let e = this.observer;
    e && e.removeAllListeners(), this.frag = null, this.observer = null, this.hls = null;
  }
  push(e, t, s2, i, r2, a2, o, l2, c, h2) {
    var u2, d3;
    c.transmuxing.start = self.performance.now();
    let { transmuxer: f3 } = this, g2 = a2 ? a2.start : r2.start, p3 = r2.decryptdata, T2 = this.frag, E4 = !(T2 && r2.cc === T2.cc), x3 = !(T2 && c.level === T2.level), y3 = T2 ? c.sn - T2.sn : -1, R = this.part ? c.part - this.part.index : -1, S2 = y3 === 0 && c.id > 1 && c.id === T2?.stats.chunkCount, b = !x3 && (y3 === 1 || y3 === 0 && (R === 1 || S2 && R <= 0)), L = self.performance.now();
    (x3 || y3 || r2.stats.parsing.start === 0) && (r2.stats.parsing.start = L), a2 && (R || !b) && (a2.stats.parsing.start = L);
    let C2 = !(T2 && ((u2 = r2.initSegment) == null ? void 0 : u2.url) === ((d3 = T2.initSegment) == null ? void 0 : d3.url)), _ = new Zs(E4, b, l2, x3, g2, C2);
    if (!b || E4 || C2) {
      v2.log(`[transmuxer-interface, ${r2.type}]: Starting new transmux session for sn: ${c.sn} p: ${c.part} level: ${c.level} id: ${c.id}
        discontinuity: ${E4}
        trackSwitch: ${x3}
        contiguous: ${b}
        accurateTimeOffset: ${l2}
        timeOffset: ${g2}
        initSegmentChange: ${C2}`);
      let I = new Js(s2, i, t, o, h2);
      this.configureTransmuxer(I);
    }
    if (this.frag = r2, this.part = a2, this.workerContext)
      this.workerContext.worker.postMessage({ cmd: "demux", data: e, decryptdata: p3, chunkMeta: c, state: _ }, e instanceof ArrayBuffer ? [e] : []);
    else if (f3) {
      let I = f3.push(e, p3, c, _);
      Lt(I) ? (f3.async = true, I.then((w) => {
        this.handleTransmuxComplete(w);
      }).catch((w) => {
        this.transmuxerError(w, c, "transmuxer-interface push error");
      })) : (f3.async = false, this.handleTransmuxComplete(I));
    }
  }
  flush(e) {
    e.transmuxing.start = self.performance.now();
    let { transmuxer: t } = this;
    if (this.workerContext)
      this.workerContext.worker.postMessage({ cmd: "flush", chunkMeta: e });
    else if (t) {
      let s2 = t.flush(e);
      Lt(s2) || t.async ? (Lt(s2) || (s2 = Promise.resolve(s2)), s2.then((r2) => {
        this.handleFlushResult(r2, e);
      }).catch((r2) => {
        this.transmuxerError(r2, e, "transmuxer-interface flush error");
      })) : this.handleFlushResult(s2, e);
    }
  }
  transmuxerError(e, t, s2) {
    this.hls && (this.error = e, this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_PARSING_ERROR, chunkMeta: t, fatal: false, error: e, err: e, reason: s2 }));
  }
  handleFlushResult(e, t) {
    e.forEach((s2) => {
      this.handleTransmuxComplete(s2);
    }), this.onFlush(t);
  }
  onWorkerMessage(e) {
    let t = e.data, s2 = this.hls;
    switch (t.event) {
      case "init": {
        var i;
        let r2 = (i = this.workerContext) == null ? void 0 : i.objectURL;
        r2 && self.URL.revokeObjectURL(r2);
        break;
      }
      case "transmuxComplete": {
        this.handleTransmuxComplete(t.data);
        break;
      }
      case "flush": {
        this.onFlush(t.data);
        break;
      }
      case "workerLog":
        v2[t.data.logType] && v2[t.data.logType](t.data.message);
        break;
      default: {
        t.data = t.data || {}, t.data.frag = this.frag, t.data.id = this.id, s2.trigger(t.event, t.data);
        break;
      }
    }
  }
  configureTransmuxer(e) {
    let { transmuxer: t } = this;
    this.workerContext ? this.workerContext.worker.postMessage({ cmd: "configure", config: e }) : t && t.configure(e);
  }
  handleTransmuxComplete(e) {
    e.chunkMeta.transmuxing.end = self.performance.now(), this.onTransmuxComplete(e);
  }
};
function Rn(n12, e) {
  if (n12.length !== e.length)
    return false;
  for (let t = 0; t < n12.length; t++)
    if (!Xe(n12[t].attrs, e[t].attrs))
      return false;
  return true;
}
function Xe(n12, e, t) {
  let s2 = n12["STABLE-RENDITION-ID"];
  return s2 && !t ? s2 === e["STABLE-RENDITION-ID"] : !(t || ["LANGUAGE", "NAME", "CHARACTERISTICS", "AUTOSELECT", "DEFAULT", "FORCED", "ASSOC-LANGUAGE"]).some((i) => n12[i] !== e[i]);
}
function ei(n12, e) {
  return e.label.toLowerCase() === n12.name.toLowerCase() && (!e.language || e.language.toLowerCase() === (n12.lang || "").toLowerCase());
}
var vr = 100;
var ti = class extends lt {
  constructor(e, t, s2) {
    super(e, t, s2, "[audio-stream-controller]", N2.AUDIO), this.videoBuffer = null, this.videoTrackCC = -1, this.waitingVideoCC = -1, this.bufferedTrack = null, this.switchingTrack = null, this.trackId = -1, this.waitingData = null, this.mainDetails = null, this.flushing = false, this.bufferFlushed = false, this.cachedTrackLoadedData = null, this._registerListeners();
  }
  onHandlerDestroying() {
    this._unregisterListeners(), super.onHandlerDestroying(), this.mainDetails = null, this.bufferedTrack = null, this.switchingTrack = null;
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.on(m2.AUDIO_TRACKS_UPDATED, this.onAudioTracksUpdated, this), e.on(m2.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this), e.on(m2.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this), e.on(m2.ERROR, this.onError, this), e.on(m2.BUFFER_RESET, this.onBufferReset, this), e.on(m2.BUFFER_CREATED, this.onBufferCreated, this), e.on(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.on(m2.BUFFER_FLUSHED, this.onBufferFlushed, this), e.on(m2.INIT_PTS_FOUND, this.onInitPtsFound, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.off(m2.AUDIO_TRACKS_UPDATED, this.onAudioTracksUpdated, this), e.off(m2.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this), e.off(m2.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this), e.off(m2.ERROR, this.onError, this), e.off(m2.BUFFER_RESET, this.onBufferReset, this), e.off(m2.BUFFER_CREATED, this.onBufferCreated, this), e.off(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.off(m2.BUFFER_FLUSHED, this.onBufferFlushed, this), e.off(m2.INIT_PTS_FOUND, this.onInitPtsFound, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  onInitPtsFound(e, { frag: t, id: s2, initPTS: i, timescale: r2 }) {
    if (s2 === "main") {
      let a2 = t.cc;
      this.initPTS[t.cc] = { baseTime: i, timescale: r2 }, this.log(`InitPTS for cc: ${a2} found from main: ${i}`), this.videoTrackCC = a2, this.state === D.WAITING_INIT_PTS && this.tick();
    }
  }
  startLoad(e) {
    if (!this.levels) {
      this.startPosition = e, this.state = D.STOPPED;
      return;
    }
    let t = this.lastCurrentTime;
    this.stopLoad(), this.setInterval(vr), t > 0 && e === -1 ? (this.log(`Override startPosition with lastCurrentTime @${t.toFixed(3)}`), e = t, this.state = D.IDLE) : (this.loadedmetadata = false, this.state = D.WAITING_TRACK), this.nextLoadPosition = this.startPosition = this.lastCurrentTime = e, this.tick();
  }
  doTick() {
    switch (this.state) {
      case D.IDLE:
        this.doTickIdle();
        break;
      case D.WAITING_TRACK: {
        var e;
        let { levels: s2, trackId: i } = this, r2 = s2 == null || (e = s2[i]) == null ? void 0 : e.details;
        if (r2) {
          if (this.waitForCdnTuneIn(r2))
            break;
          this.state = D.WAITING_INIT_PTS;
        }
        break;
      }
      case D.FRAG_LOADING_WAITING_RETRY: {
        var t;
        let s2 = performance.now(), i = this.retryDate;
        if (!i || s2 >= i || (t = this.media) != null && t.seeking) {
          let { levels: r2, trackId: a2 } = this;
          this.log("RetryDate reached, switch back to IDLE state"), this.resetStartWhenNotLoaded(r2?.[a2] || null), this.state = D.IDLE;
        }
        break;
      }
      case D.WAITING_INIT_PTS: {
        let s2 = this.waitingData;
        if (s2) {
          let { frag: i, part: r2, cache: a2, complete: o } = s2;
          if (this.initPTS[i.cc] !== void 0) {
            this.waitingData = null, this.waitingVideoCC = -1, this.state = D.FRAG_LOADING;
            let l2 = a2.flush(), c = { frag: i, part: r2, payload: l2, networkDetails: null };
            this._handleFragmentLoadProgress(c), o && super._handleFragmentLoadComplete(c);
          } else if (this.videoTrackCC !== this.waitingVideoCC)
            this.log(`Waiting fragment cc (${i.cc}) cancelled because video is at cc ${this.videoTrackCC}`), this.clearWaitingFragment();
          else {
            let l2 = this.getLoadPosition(), c = Q.bufferInfo(this.mediaBuffer, l2, this.config.maxBufferHole);
            Ps(c.end, this.config.maxFragLookUpTolerance, i) < 0 && (this.log(`Waiting fragment cc (${i.cc}) @ ${i.start} cancelled because another fragment at ${c.end} is needed`), this.clearWaitingFragment());
          }
        } else
          this.state = D.IDLE;
      }
    }
    this.onTickEnd();
  }
  clearWaitingFragment() {
    let e = this.waitingData;
    e && (this.fragmentTracker.removeFragment(e.frag), this.waitingData = null, this.waitingVideoCC = -1, this.state = D.IDLE);
  }
  resetLoadingState() {
    this.clearWaitingFragment(), super.resetLoadingState();
  }
  onTickEnd() {
    let { media: e } = this;
    e != null && e.readyState && (this.lastCurrentTime = e.currentTime);
  }
  doTickIdle() {
    let { hls: e, levels: t, media: s2, trackId: i } = this, r2 = e.config;
    if (!s2 && (this.startFragRequested || !r2.startFragPrefetch) || !(t != null && t[i]))
      return;
    let a2 = t[i], o = a2.details;
    if (!o || o.live && this.levelLastLoaded !== a2 || this.waitForCdnTuneIn(o)) {
      this.state = D.WAITING_TRACK;
      return;
    }
    let l2 = this.mediaBuffer ? this.mediaBuffer : this.media;
    this.bufferFlushed && l2 && (this.bufferFlushed = false, this.afterBufferFlushed(l2, z2.AUDIO, N2.AUDIO));
    let c = this.getFwdBufferInfo(l2, N2.AUDIO);
    if (c === null)
      return;
    let { bufferedTrack: h2, switchingTrack: u2 } = this;
    if (!u2 && this._streamEnded(c, o)) {
      e.trigger(m2.BUFFER_EOS, { type: "audio" }), this.state = D.ENDED;
      return;
    }
    let d3 = this.getFwdBufferInfo(this.videoBuffer ? this.videoBuffer : this.media, N2.MAIN), f3 = c.len, g2 = this.getMaxBufferLength(d3?.len), p3 = o.fragments, T2 = p3[0].start, E4 = this.flushing ? this.getLoadPosition() : c.end;
    if (u2 && s2) {
      let S2 = this.getLoadPosition();
      h2 && !Xe(u2.attrs, h2.attrs) && (E4 = S2), o.PTSKnown && S2 < T2 && (c.end > T2 || c.nextStart) && (this.log("Alt audio track ahead of main track, seek to start of alt audio track"), s2.currentTime = T2 + 0.05);
    }
    if (f3 >= g2 && !u2 && E4 < p3[p3.length - 1].start)
      return;
    let x3 = this.getNextFragment(E4, o), y3 = false;
    if (x3 && this.isLoopLoading(x3, E4) && (y3 = !!x3.gap, x3 = this.getNextFragmentLoopLoading(x3, o, c, N2.MAIN, g2)), !x3) {
      this.bufferFlushed = true;
      return;
    }
    let R = d3 && x3.start > d3.end + o.targetduration;
    if (R || !(d3 != null && d3.len) && c.len) {
      let S2 = this.getAppendedFrag(x3.start, N2.MAIN);
      if (S2 === null || (y3 || (y3 = !!S2.gap || !!R && d3.len === 0), R && !y3 || y3 && c.nextStart && c.nextStart < S2.end))
        return;
    }
    this.loadFragment(x3, a2, E4);
  }
  getMaxBufferLength(e) {
    let t = super.getMaxBufferLength();
    return e ? Math.min(Math.max(t, e), this.config.maxMaxBufferLength) : t;
  }
  onMediaDetaching() {
    this.videoBuffer = null, this.bufferFlushed = this.flushing = false, super.onMediaDetaching();
  }
  onAudioTracksUpdated(e, { audioTracks: t }) {
    this.resetTransmuxer(), this.levels = t.map((s2) => new Pe3(s2));
  }
  onAudioTrackSwitching(e, t) {
    let s2 = !!t.url;
    this.trackId = t.id;
    let { fragCurrent: i } = this;
    i && (i.abortRequests(), this.removeUnbufferedFrags(i.start)), this.resetLoadingState(), s2 ? this.setInterval(vr) : this.resetTransmuxer(), s2 ? (this.switchingTrack = t, this.state = D.IDLE, this.flushAudioIfNeeded(t)) : (this.switchingTrack = null, this.bufferedTrack = t, this.state = D.STOPPED), this.tick();
  }
  onManifestLoading() {
    this.fragmentTracker.removeAllFragments(), this.startPosition = this.lastCurrentTime = 0, this.bufferFlushed = this.flushing = false, this.levels = this.mainDetails = this.waitingData = this.bufferedTrack = this.cachedTrackLoadedData = this.switchingTrack = null, this.startFragRequested = false, this.trackId = this.videoTrackCC = this.waitingVideoCC = -1;
  }
  onLevelLoaded(e, t) {
    this.mainDetails = t.details, this.cachedTrackLoadedData !== null && (this.hls.trigger(m2.AUDIO_TRACK_LOADED, this.cachedTrackLoadedData), this.cachedTrackLoadedData = null);
  }
  onAudioTrackLoaded(e, t) {
    var s2;
    if (this.mainDetails == null) {
      this.cachedTrackLoadedData = t;
      return;
    }
    let { levels: i } = this, { details: r2, id: a2 } = t;
    if (!i) {
      this.warn(`Audio tracks were reset while loading level ${a2}`);
      return;
    }
    this.log(`Audio track ${a2} loaded [${r2.startSN},${r2.endSN}]${r2.lastPartSn ? `[part-${r2.lastPartSn}-${r2.lastPartIndex}]` : ""},duration:${r2.totalduration}`);
    let o = i[a2], l2 = 0;
    if (r2.live || (s2 = o.details) != null && s2.live) {
      this.checkLiveUpdate(r2);
      let h2 = this.mainDetails;
      if (r2.deltaUpdateFailed || !h2)
        return;
      if (!o.details && r2.hasProgramDateTime && h2.hasProgramDateTime)
        Ut(r2, h2), l2 = r2.fragments[0].start;
      else {
        var c;
        l2 = this.alignPlaylists(r2, o.details, (c = this.levelLastLoaded) == null ? void 0 : c.details);
      }
    }
    o.details = r2, this.levelLastLoaded = o, !this.startFragRequested && (this.mainDetails || !r2.live) && this.setStartPosition(this.mainDetails || r2, l2), this.state === D.WAITING_TRACK && !this.waitForCdnTuneIn(r2) && (this.state = D.IDLE), this.tick();
  }
  _handleFragmentLoadProgress(e) {
    var t;
    let { frag: s2, part: i, payload: r2 } = e, { config: a2, trackId: o, levels: l2 } = this;
    if (!l2) {
      this.warn(`Audio tracks were reset while fragment load was in progress. Fragment ${s2.sn} of level ${s2.level} will not be buffered`);
      return;
    }
    let c = l2[o];
    if (!c) {
      this.warn("Audio track is undefined on fragment load progress");
      return;
    }
    let h2 = c.details;
    if (!h2) {
      this.warn("Audio track details undefined on fragment load progress"), this.removeUnbufferedFrags(s2.start);
      return;
    }
    let u2 = a2.defaultAudioCodec || c.audioCodec || "mp4a.40.2", d3 = this.transmuxer;
    d3 || (d3 = this.transmuxer = new Wt(this.hls, N2.AUDIO, this._handleTransmuxComplete.bind(this), this._handleTransmuxerFlush.bind(this)));
    let f3 = this.initPTS[s2.cc], g2 = (t = s2.initSegment) == null ? void 0 : t.data;
    if (f3 !== void 0) {
      let T2 = i ? i.index : -1, E4 = T2 !== -1, x3 = new at(s2.level, s2.sn, s2.stats.chunkCount, r2.byteLength, T2, E4);
      d3.push(r2, g2, u2, "", s2, i, h2.totalduration, false, x3, f3);
    } else {
      this.log(`Unknown video PTS for cc ${s2.cc}, waiting for video PTS before demuxing audio frag ${s2.sn} of [${h2.startSN} ,${h2.endSN}],track ${o}`);
      let { cache: p3 } = this.waitingData = this.waitingData || { frag: s2, part: i, cache: new Bt(), complete: false };
      p3.push(new Uint8Array(r2)), this.waitingVideoCC = this.videoTrackCC, this.state = D.WAITING_INIT_PTS;
    }
  }
  _handleFragmentLoadComplete(e) {
    if (this.waitingData) {
      this.waitingData.complete = true;
      return;
    }
    super._handleFragmentLoadComplete(e);
  }
  onBufferReset() {
    this.mediaBuffer = this.videoBuffer = null, this.loadedmetadata = false;
  }
  onBufferCreated(e, t) {
    let s2 = t.tracks.audio;
    s2 && (this.mediaBuffer = s2.buffer || null), t.tracks.video && (this.videoBuffer = t.tracks.video.buffer || null);
  }
  onFragBuffered(e, t) {
    let { frag: s2, part: i } = t;
    if (s2.type !== N2.AUDIO) {
      if (!this.loadedmetadata && s2.type === N2.MAIN) {
        let r2 = this.videoBuffer || this.media;
        r2 && Q.getBuffered(r2).length && (this.loadedmetadata = true);
      }
      return;
    }
    if (this.fragContextChanged(s2)) {
      this.warn(`Fragment ${s2.sn}${i ? " p: " + i.index : ""} of level ${s2.level} finished buffering, but was aborted. state: ${this.state}, audioSwitch: ${this.switchingTrack ? this.switchingTrack.name : "false"}`);
      return;
    }
    if (s2.sn !== "initSegment") {
      this.fragPrevious = s2;
      let r2 = this.switchingTrack;
      r2 && (this.bufferedTrack = r2, this.switchingTrack = null, this.hls.trigger(m2.AUDIO_TRACK_SWITCHED, le3({}, r2)));
    }
    this.fragBufferedComplete(s2, i);
  }
  onError(e, t) {
    var s2;
    if (t.fatal) {
      this.state = D.ERROR;
      return;
    }
    switch (t.details) {
      case A2.FRAG_GAP:
      case A2.FRAG_PARSING_ERROR:
      case A2.FRAG_DECRYPT_ERROR:
      case A2.FRAG_LOAD_ERROR:
      case A2.FRAG_LOAD_TIMEOUT:
      case A2.KEY_LOAD_ERROR:
      case A2.KEY_LOAD_TIMEOUT:
        this.onFragmentOrKeyLoadError(N2.AUDIO, t);
        break;
      case A2.AUDIO_TRACK_LOAD_ERROR:
      case A2.AUDIO_TRACK_LOAD_TIMEOUT:
      case A2.LEVEL_PARSING_ERROR:
        !t.levelRetry && this.state === D.WAITING_TRACK && ((s2 = t.context) == null ? void 0 : s2.type) === W3.AUDIO_TRACK && (this.state = D.IDLE);
        break;
      case A2.BUFFER_APPEND_ERROR:
      case A2.BUFFER_FULL_ERROR:
        if (!t.parent || t.parent !== "audio")
          return;
        if (t.details === A2.BUFFER_APPEND_ERROR) {
          this.resetLoadingState();
          return;
        }
        this.reduceLengthAndFlushBuffer(t) && (this.bufferedTrack = null, super.flushMainBuffer(0, Number.POSITIVE_INFINITY, "audio"));
        break;
      case A2.INTERNAL_EXCEPTION:
        this.recoverWorkerError(t);
        break;
    }
  }
  onBufferFlushing(e, { type: t }) {
    t !== z2.VIDEO && (this.flushing = true);
  }
  onBufferFlushed(e, { type: t }) {
    if (t !== z2.VIDEO) {
      this.flushing = false, this.bufferFlushed = true, this.state === D.ENDED && (this.state = D.IDLE);
      let s2 = this.mediaBuffer || this.media;
      s2 && (this.afterBufferFlushed(s2, t, N2.AUDIO), this.tick());
    }
  }
  _handleTransmuxComplete(e) {
    var t;
    let s2 = "audio", { hls: i } = this, { remuxResult: r2, chunkMeta: a2 } = e, o = this.getCurrentContext(a2);
    if (!o) {
      this.resetWhenMissingContext(a2);
      return;
    }
    let { frag: l2, part: c, level: h2 } = o, { details: u2 } = h2, { audio: d3, text: f3, id3: g2, initSegment: p3 } = r2;
    if (this.fragContextChanged(l2) || !u2) {
      this.fragmentTracker.removeFragment(l2);
      return;
    }
    if (this.state = D.PARSING, this.switchingTrack && d3 && this.completeAudioSwitch(this.switchingTrack), p3 != null && p3.tracks) {
      let T2 = l2.initSegment || l2;
      this._bufferInitSegment(h2, p3.tracks, T2, a2), i.trigger(m2.FRAG_PARSING_INIT_SEGMENT, { frag: T2, id: s2, tracks: p3.tracks });
    }
    if (d3) {
      let { startPTS: T2, endPTS: E4, startDTS: x3, endDTS: y3 } = d3;
      c && (c.elementaryStreams[z2.AUDIO] = { startPTS: T2, endPTS: E4, startDTS: x3, endDTS: y3 }), l2.setElementaryStreamInfo(z2.AUDIO, T2, E4, x3, y3), this.bufferFragmentData(d3, l2, c, a2);
    }
    if (g2 != null && (t = g2.samples) != null && t.length) {
      let T2 = te2({ id: s2, frag: l2, details: u2 }, g2);
      i.trigger(m2.FRAG_PARSING_METADATA, T2);
    }
    if (f3) {
      let T2 = te2({ id: s2, frag: l2, details: u2 }, f3);
      i.trigger(m2.FRAG_PARSING_USERDATA, T2);
    }
  }
  _bufferInitSegment(e, t, s2, i) {
    if (this.state !== D.PARSING)
      return;
    t.video && delete t.video;
    let r2 = t.audio;
    if (!r2)
      return;
    r2.id = "audio";
    let a2 = e.audioCodec;
    this.log(`Init audio buffer, container:${r2.container}, codecs[level/parsed]=[${a2}/${r2.codec}]`), a2 && a2.split(",").length === 1 && (r2.levelCodec = a2), this.hls.trigger(m2.BUFFER_CODECS, t);
    let o = r2.initSegment;
    if (o != null && o.byteLength) {
      let l2 = { type: "audio", frag: s2, part: null, chunkMeta: i, parent: s2.type, data: o };
      this.hls.trigger(m2.BUFFER_APPENDING, l2);
    }
    this.tickImmediate();
  }
  loadFragment(e, t, s2) {
    let i = this.fragmentTracker.getState(e);
    if (this.fragCurrent = e, this.switchingTrack || i === oe2.NOT_LOADED || i === oe2.PARTIAL) {
      var r2;
      if (e.sn === "initSegment")
        this._loadInitSegment(e, t);
      else if ((r2 = t.details) != null && r2.live && !this.initPTS[e.cc]) {
        this.log(`Waiting for video PTS in continuity counter ${e.cc} of live stream before loading audio fragment ${e.sn} of level ${this.trackId}`), this.state = D.WAITING_INIT_PTS;
        let a2 = this.mainDetails;
        a2 && a2.fragments[0].start !== t.details.fragments[0].start && Ut(t.details, a2);
      } else
        this.startFragRequested = true, super.loadFragment(e, t, s2);
    } else
      this.clearTrackerIfNeeded(e);
  }
  flushAudioIfNeeded(e) {
    let { media: t, bufferedTrack: s2 } = this, i = s2?.attrs, r2 = e.attrs;
    t && i && (i.CHANNELS !== r2.CHANNELS || s2.name !== e.name || s2.lang !== e.lang) && (this.log("Switching audio track : flushing all audio"), super.flushMainBuffer(0, Number.POSITIVE_INFINITY, "audio"), this.bufferedTrack = null);
  }
  completeAudioSwitch(e) {
    let { hls: t } = this;
    this.flushAudioIfNeeded(e), this.bufferedTrack = e, this.switchingTrack = null, t.trigger(m2.AUDIO_TRACK_SWITCHED, le3({}, e));
  }
};
var si = class extends nt {
  constructor(e) {
    super(e, "[audio-track-controller]"), this.tracks = [], this.groupIds = null, this.tracksInGroup = [], this.trackId = -1, this.currentTrack = null, this.selectDefaultTrack = true, this.registerListeners();
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.LEVEL_LOADING, this.onLevelLoading, this), e.on(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.on(m2.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this), e.on(m2.ERROR, this.onError, this);
  }
  unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.LEVEL_LOADING, this.onLevelLoading, this), e.off(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.off(m2.AUDIO_TRACK_LOADED, this.onAudioTrackLoaded, this), e.off(m2.ERROR, this.onError, this);
  }
  destroy() {
    this.unregisterListeners(), this.tracks.length = 0, this.tracksInGroup.length = 0, this.currentTrack = null, super.destroy();
  }
  onManifestLoading() {
    this.tracks = [], this.tracksInGroup = [], this.groupIds = null, this.currentTrack = null, this.trackId = -1, this.selectDefaultTrack = true;
  }
  onManifestParsed(e, t) {
    this.tracks = t.audioTracks || [];
  }
  onAudioTrackLoaded(e, t) {
    let { id: s2, groupId: i, details: r2 } = t, a2 = this.tracksInGroup[s2];
    if (!a2 || a2.groupId !== i) {
      this.warn(`Audio track with id:${s2} and group:${i} not found in active group ${a2?.groupId}`);
      return;
    }
    let o = a2.details;
    a2.details = t.details, this.log(`Audio track ${s2} "${a2.name}" lang:${a2.lang} group:${i} loaded [${r2.startSN}-${r2.endSN}]`), s2 === this.trackId && this.playlistLoaded(s2, t, o);
  }
  onLevelLoading(e, t) {
    this.switchLevel(t.level);
  }
  onLevelSwitching(e, t) {
    this.switchLevel(t.level);
  }
  switchLevel(e) {
    let t = this.hls.levels[e];
    if (!t)
      return;
    let s2 = t.audioGroups || null, i = this.groupIds, r2 = this.currentTrack;
    if (!s2 || i?.length !== s2?.length || s2 != null && s2.some((o) => i?.indexOf(o) === -1)) {
      this.groupIds = s2, this.trackId = -1, this.currentTrack = null;
      let o = this.tracks.filter((d3) => !s2 || s2.indexOf(d3.groupId) !== -1);
      if (o.length)
        this.selectDefaultTrack && !o.some((d3) => d3.default) && (this.selectDefaultTrack = false), o.forEach((d3, f3) => {
          d3.id = f3;
        });
      else if (!r2 && !this.tracksInGroup.length)
        return;
      this.tracksInGroup = o;
      let l2 = this.hls.config.audioPreference;
      if (!r2 && l2) {
        let d3 = Le2(l2, o, Be2);
        if (d3 > -1)
          r2 = o[d3];
        else {
          let f3 = Le2(l2, this.tracks);
          r2 = this.tracks[f3];
        }
      }
      let c = this.findTrackId(r2);
      c === -1 && r2 && (c = this.findTrackId(null));
      let h2 = { audioTracks: o };
      this.log(`Updating audio tracks, ${o.length} track(s) found in group(s): ${s2?.join(",")}`), this.hls.trigger(m2.AUDIO_TRACKS_UPDATED, h2);
      let u2 = this.trackId;
      if (c !== -1 && u2 === -1)
        this.setAudioTrack(c);
      else if (o.length && u2 === -1) {
        var a2;
        let d3 = new Error(`No audio track selected for current audio group-ID(s): ${(a2 = this.groupIds) == null ? void 0 : a2.join(",")} track count: ${o.length}`);
        this.warn(d3.message), this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.AUDIO_TRACK_LOAD_ERROR, fatal: true, error: d3 });
      }
    } else
      this.shouldReloadPlaylist(r2) && this.setAudioTrack(this.trackId);
  }
  onError(e, t) {
    t.fatal || !t.context || t.context.type === W3.AUDIO_TRACK && t.context.id === this.trackId && (!this.groupIds || this.groupIds.indexOf(t.context.groupId) !== -1) && (this.requestScheduled = -1, this.checkRetry(t));
  }
  get allAudioTracks() {
    return this.tracks;
  }
  get audioTracks() {
    return this.tracksInGroup;
  }
  get audioTrack() {
    return this.trackId;
  }
  set audioTrack(e) {
    this.selectDefaultTrack = false, this.setAudioTrack(e);
  }
  setAudioOption(e) {
    let t = this.hls;
    if (t.config.audioPreference = e, e) {
      let s2 = this.allAudioTracks;
      if (this.selectDefaultTrack = false, s2.length) {
        let i = this.currentTrack;
        if (i && Ye(e, i, Be2))
          return i;
        let r2 = Le2(e, this.tracksInGroup, Be2);
        if (r2 > -1) {
          let a2 = this.tracksInGroup[r2];
          return this.setAudioTrack(r2), a2;
        } else if (i) {
          let a2 = t.loadLevel;
          a2 === -1 && (a2 = t.firstAutoLevel);
          let o = go(e, t.levels, s2, a2, Be2);
          if (o === -1)
            return null;
          t.nextLoadLevel = o;
        }
        if (e.channels || e.audioCodec) {
          let a2 = Le2(e, s2);
          if (a2 > -1)
            return s2[a2];
        }
      }
    }
    return null;
  }
  setAudioTrack(e) {
    let t = this.tracksInGroup;
    if (e < 0 || e >= t.length) {
      this.warn(`Invalid audio track id: ${e}`);
      return;
    }
    this.clearTimer(), this.selectDefaultTrack = false;
    let s2 = this.currentTrack, i = t[e], r2 = i.details && !i.details.live;
    if (e === this.trackId && i === s2 && r2 || (this.log(`Switching to audio-track ${e} "${i.name}" lang:${i.lang} group:${i.groupId} channels:${i.channels}`), this.trackId = e, this.currentTrack = i, this.hls.trigger(m2.AUDIO_TRACK_SWITCHING, le3({}, i)), r2))
      return;
    let a2 = this.switchParams(i.url, s2?.details, i.details);
    this.loadPlaylist(a2);
  }
  findTrackId(e) {
    let t = this.tracksInGroup;
    for (let s2 = 0; s2 < t.length; s2++) {
      let i = t[s2];
      if (!(this.selectDefaultTrack && !i.default) && (!e || Ye(e, i, Be2)))
        return s2;
    }
    if (e) {
      let { name: s2, lang: i, assocLang: r2, characteristics: a2, audioCodec: o, channels: l2 } = e;
      for (let c = 0; c < t.length; c++) {
        let h2 = t[c];
        if (Ye({ name: s2, lang: i, assocLang: r2, characteristics: a2, audioCodec: o, channels: l2 }, h2, Be2))
          return c;
      }
      for (let c = 0; c < t.length; c++) {
        let h2 = t[c];
        if (Xe(e.attrs, h2.attrs, ["LANGUAGE", "ASSOC-LANGUAGE", "CHARACTERISTICS"]))
          return c;
      }
      for (let c = 0; c < t.length; c++) {
        let h2 = t[c];
        if (Xe(e.attrs, h2.attrs, ["LANGUAGE"]))
          return c;
      }
    }
    return -1;
  }
  loadPlaylist(e) {
    let t = this.currentTrack;
    if (this.shouldLoadPlaylist(t) && t) {
      super.loadPlaylist();
      let s2 = t.id, i = t.groupId, r2 = t.url;
      if (e)
        try {
          r2 = e.addDirectives(r2);
        } catch (a2) {
          this.warn(`Could not construct new URL with HLS Delivery Directives: ${a2}`);
        }
      this.log(`loading audio-track playlist ${s2} "${t.name}" lang:${t.lang} group:${i}`), this.clearTimer(), this.hls.trigger(m2.AUDIO_TRACK_LOADING, { url: r2, id: s2, groupId: i, deliveryDirectives: e || null });
    }
  }
};
var Ar = 500;
var ii = class extends lt {
  constructor(e, t, s2) {
    super(e, t, s2, "[subtitle-stream-controller]", N2.SUBTITLE), this.currentTrackId = -1, this.tracksBuffered = [], this.mainDetails = null, this._registerListeners();
  }
  onHandlerDestroying() {
    this._unregisterListeners(), super.onHandlerDestroying(), this.mainDetails = null;
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.on(m2.ERROR, this.onError, this), e.on(m2.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this), e.on(m2.SUBTITLE_TRACK_SWITCH, this.onSubtitleTrackSwitch, this), e.on(m2.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this), e.on(m2.SUBTITLE_FRAG_PROCESSED, this.onSubtitleFragProcessed, this), e.on(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.off(m2.ERROR, this.onError, this), e.off(m2.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this), e.off(m2.SUBTITLE_TRACK_SWITCH, this.onSubtitleTrackSwitch, this), e.off(m2.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this), e.off(m2.SUBTITLE_FRAG_PROCESSED, this.onSubtitleFragProcessed, this), e.off(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  startLoad(e) {
    this.stopLoad(), this.state = D.IDLE, this.setInterval(Ar), this.nextLoadPosition = this.startPosition = this.lastCurrentTime = e, this.tick();
  }
  onManifestLoading() {
    this.mainDetails = null, this.fragmentTracker.removeAllFragments();
  }
  onMediaDetaching() {
    this.tracksBuffered = [], super.onMediaDetaching();
  }
  onLevelLoaded(e, t) {
    this.mainDetails = t.details;
  }
  onSubtitleFragProcessed(e, t) {
    let { frag: s2, success: i } = t;
    if (this.fragPrevious = s2, this.state = D.IDLE, !i)
      return;
    let r2 = this.tracksBuffered[this.currentTrackId];
    if (!r2)
      return;
    let a2, o = s2.start;
    for (let c = 0; c < r2.length; c++)
      if (o >= r2[c].start && o <= r2[c].end) {
        a2 = r2[c];
        break;
      }
    let l2 = s2.start + s2.duration;
    a2 ? a2.end = l2 : (a2 = { start: o, end: l2 }, r2.push(a2)), this.fragmentTracker.fragBuffered(s2), this.fragBufferedComplete(s2, null);
  }
  onBufferFlushing(e, t) {
    let { startOffset: s2, endOffset: i } = t;
    if (s2 === 0 && i !== Number.POSITIVE_INFINITY) {
      let r2 = i - 1;
      if (r2 <= 0)
        return;
      t.endOffsetSubtitles = Math.max(0, r2), this.tracksBuffered.forEach((a2) => {
        for (let o = 0; o < a2.length; ) {
          if (a2[o].end <= r2) {
            a2.shift();
            continue;
          } else if (a2[o].start < r2)
            a2[o].start = r2;
          else
            break;
          o++;
        }
      }), this.fragmentTracker.removeFragmentsInRange(s2, r2, N2.SUBTITLE);
    }
  }
  onFragBuffered(e, t) {
    if (!this.loadedmetadata && t.frag.type === N2.MAIN) {
      var s2;
      (s2 = this.media) != null && s2.buffered.length && (this.loadedmetadata = true);
    }
  }
  onError(e, t) {
    let s2 = t.frag;
    s2?.type === N2.SUBTITLE && (this.fragCurrent && this.fragCurrent.abortRequests(), this.state !== D.STOPPED && (this.state = D.IDLE));
  }
  onSubtitleTracksUpdated(e, { subtitleTracks: t }) {
    if (this.levels && Rn(this.levels, t)) {
      this.levels = t.map((s2) => new Pe3(s2));
      return;
    }
    this.tracksBuffered = [], this.levels = t.map((s2) => {
      let i = new Pe3(s2);
      return this.tracksBuffered[i.id] = [], i;
    }), this.fragmentTracker.removeFragmentsInRange(0, Number.POSITIVE_INFINITY, N2.SUBTITLE), this.fragPrevious = null, this.mediaBuffer = null;
  }
  onSubtitleTrackSwitch(e, t) {
    var s2;
    if (this.currentTrackId = t.id, !((s2 = this.levels) != null && s2.length) || this.currentTrackId === -1) {
      this.clearInterval();
      return;
    }
    let i = this.levels[this.currentTrackId];
    i != null && i.details ? this.mediaBuffer = this.mediaBufferTimeRanges : this.mediaBuffer = null, i && this.setInterval(Ar);
  }
  onSubtitleTrackLoaded(e, t) {
    var s2;
    let { currentTrackId: i, levels: r2 } = this, { details: a2, id: o } = t;
    if (!r2) {
      this.warn(`Subtitle tracks were reset while loading level ${o}`);
      return;
    }
    let l2 = r2[i];
    if (o >= r2.length || o !== i || !l2)
      return;
    this.log(`Subtitle track ${o} loaded [${a2.startSN},${a2.endSN}]${a2.lastPartSn ? `[part-${a2.lastPartSn}-${a2.lastPartIndex}]` : ""},duration:${a2.totalduration}`), this.mediaBuffer = this.mediaBufferTimeRanges;
    let c = 0;
    if (a2.live || (s2 = l2.details) != null && s2.live) {
      let u2 = this.mainDetails;
      if (a2.deltaUpdateFailed || !u2)
        return;
      let d3 = u2.fragments[0];
      if (!l2.details)
        a2.hasProgramDateTime && u2.hasProgramDateTime ? (Ut(a2, u2), c = a2.fragments[0].start) : d3 && (c = d3.start, _s(a2, c));
      else {
        var h2;
        c = this.alignPlaylists(a2, l2.details, (h2 = this.levelLastLoaded) == null ? void 0 : h2.details), c === 0 && d3 && (c = d3.start, _s(a2, c));
      }
    }
    l2.details = a2, this.levelLastLoaded = l2, !this.startFragRequested && (this.mainDetails || !a2.live) && this.setStartPosition(this.mainDetails || a2, c), this.tick(), a2.live && !this.fragCurrent && this.media && this.state === D.IDLE && (Nt(null, a2.fragments, this.media.currentTime, 0) || (this.warn("Subtitle playlist not aligned with playback"), l2.details = void 0));
  }
  _handleFragmentLoadComplete(e) {
    let { frag: t, payload: s2 } = e, i = t.decryptdata, r2 = this.hls;
    if (!this.fragContextChanged(t) && s2 && s2.byteLength > 0 && i != null && i.key && i.iv && i.method === "AES-128") {
      let a2 = performance.now();
      this.decrypter.decrypt(new Uint8Array(s2), i.key.buffer, i.iv.buffer).catch((o) => {
        throw r2.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.FRAG_DECRYPT_ERROR, fatal: false, error: o, reason: o.message, frag: t }), o;
      }).then((o) => {
        let l2 = performance.now();
        r2.trigger(m2.FRAG_DECRYPTED, { frag: t, payload: o, stats: { tstart: a2, tdecrypt: l2 } });
      }).catch((o) => {
        this.warn(`${o.name}: ${o.message}`), this.state = D.IDLE;
      });
    }
  }
  doTick() {
    if (!this.media) {
      this.state = D.IDLE;
      return;
    }
    if (this.state === D.IDLE) {
      let { currentTrackId: e, levels: t } = this, s2 = t?.[e];
      if (!s2 || !t.length || !s2.details)
        return;
      let { config: i } = this, r2 = this.getLoadPosition(), a2 = Q.bufferedInfo(this.tracksBuffered[this.currentTrackId] || [], r2, i.maxBufferHole), { end: o, len: l2 } = a2, c = this.getFwdBufferInfo(this.media, N2.MAIN), h2 = s2.details, u2 = this.getMaxBufferLength(c?.len) + h2.levelTargetDuration;
      if (l2 > u2)
        return;
      let d3 = h2.fragments, f3 = d3.length, g2 = h2.edge, p3 = null, T2 = this.fragPrevious;
      if (o < g2) {
        let E4 = i.maxFragLookUpTolerance, x3 = o > g2 - E4 ? 0 : E4;
        p3 = Nt(T2, d3, Math.max(d3[0].start, o), x3), !p3 && T2 && T2.start < d3[0].start && (p3 = d3[0]);
      } else
        p3 = d3[f3 - 1];
      if (!p3)
        return;
      if (p3 = this.mapToInitFragWhenRequired(p3), p3.sn !== "initSegment") {
        let E4 = p3.sn - h2.startSN, x3 = d3[E4 - 1];
        x3 && x3.cc === p3.cc && this.fragmentTracker.getState(x3) === oe2.NOT_LOADED && (p3 = x3);
      }
      this.fragmentTracker.getState(p3) === oe2.NOT_LOADED && this.loadFragment(p3, s2, o);
    }
  }
  getMaxBufferLength(e) {
    let t = super.getMaxBufferLength();
    return e ? Math.max(t, e) : t;
  }
  loadFragment(e, t, s2) {
    this.fragCurrent = e, e.sn === "initSegment" ? this._loadInitSegment(e, t) : (this.startFragRequested = true, super.loadFragment(e, t, s2));
  }
  get mediaBufferTimeRanges() {
    return new ri(this.tracksBuffered[this.currentTrackId] || []);
  }
};
var ri = class {
  constructor(e) {
    this.buffered = void 0;
    let t = (s2, i, r2) => {
      if (i = i >>> 0, i > r2 - 1)
        throw new DOMException(`Failed to execute '${s2}' on 'TimeRanges': The index provided (${i}) is greater than the maximum bound (${r2})`);
      return e[i][s2];
    };
    this.buffered = { get length() {
      return e.length;
    }, end(s2) {
      return t("end", s2, e.length);
    }, start(s2) {
      return t("start", s2, e.length);
    } };
  }
};
var ni = class extends nt {
  constructor(e) {
    super(e, "[subtitle-track-controller]"), this.media = null, this.tracks = [], this.groupIds = null, this.tracksInGroup = [], this.trackId = -1, this.currentTrack = null, this.selectDefaultTrack = true, this.queuedDefaultTrack = -1, this.asyncPollTrackChange = () => this.pollTrackChange(0), this.useTextTrackPolling = false, this.subtitlePollingInterval = -1, this._subtitleDisplay = true, this.onTextTracksChanged = () => {
      if (this.useTextTrackPolling || self.clearInterval(this.subtitlePollingInterval), !this.media || !this.hls.config.renderTextTracksNatively)
        return;
      let t = null, s2 = xt(this.media.textTracks);
      for (let r2 = 0; r2 < s2.length; r2++)
        if (s2[r2].mode === "hidden")
          t = s2[r2];
        else if (s2[r2].mode === "showing") {
          t = s2[r2];
          break;
        }
      let i = this.findTrackForTextTrack(t);
      this.subtitleTrack !== i && this.setSubtitleTrack(i);
    }, this.registerListeners();
  }
  destroy() {
    this.unregisterListeners(), this.tracks.length = 0, this.tracksInGroup.length = 0, this.currentTrack = null, this.onTextTracksChanged = this.asyncPollTrackChange = null, super.destroy();
  }
  get subtitleDisplay() {
    return this._subtitleDisplay;
  }
  set subtitleDisplay(e) {
    this._subtitleDisplay = e, this.trackId > -1 && this.toggleTrackModes();
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.LEVEL_LOADING, this.onLevelLoading, this), e.on(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.on(m2.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this), e.on(m2.ERROR, this.onError, this);
  }
  unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.LEVEL_LOADING, this.onLevelLoading, this), e.off(m2.LEVEL_SWITCHING, this.onLevelSwitching, this), e.off(m2.SUBTITLE_TRACK_LOADED, this.onSubtitleTrackLoaded, this), e.off(m2.ERROR, this.onError, this);
  }
  onMediaAttached(e, t) {
    this.media = t.media, this.media && (this.queuedDefaultTrack > -1 && (this.subtitleTrack = this.queuedDefaultTrack, this.queuedDefaultTrack = -1), this.useTextTrackPolling = !(this.media.textTracks && "onchange" in this.media.textTracks), this.useTextTrackPolling ? this.pollTrackChange(500) : this.media.textTracks.addEventListener("change", this.asyncPollTrackChange));
  }
  pollTrackChange(e) {
    self.clearInterval(this.subtitlePollingInterval), this.subtitlePollingInterval = self.setInterval(this.onTextTracksChanged, e);
  }
  onMediaDetaching() {
    if (!this.media)
      return;
    self.clearInterval(this.subtitlePollingInterval), this.useTextTrackPolling || this.media.textTracks.removeEventListener("change", this.asyncPollTrackChange), this.trackId > -1 && (this.queuedDefaultTrack = this.trackId), xt(this.media.textTracks).forEach((t) => {
      Ve2(t);
    }), this.subtitleTrack = -1, this.media = null;
  }
  onManifestLoading() {
    this.tracks = [], this.groupIds = null, this.tracksInGroup = [], this.trackId = -1, this.currentTrack = null, this.selectDefaultTrack = true;
  }
  onManifestParsed(e, t) {
    this.tracks = t.subtitleTracks;
  }
  onSubtitleTrackLoaded(e, t) {
    let { id: s2, groupId: i, details: r2 } = t, a2 = this.tracksInGroup[s2];
    if (!a2 || a2.groupId !== i) {
      this.warn(`Subtitle track with id:${s2} and group:${i} not found in active group ${a2?.groupId}`);
      return;
    }
    let o = a2.details;
    a2.details = t.details, this.log(`Subtitle track ${s2} "${a2.name}" lang:${a2.lang} group:${i} loaded [${r2.startSN}-${r2.endSN}]`), s2 === this.trackId && this.playlistLoaded(s2, t, o);
  }
  onLevelLoading(e, t) {
    this.switchLevel(t.level);
  }
  onLevelSwitching(e, t) {
    this.switchLevel(t.level);
  }
  switchLevel(e) {
    let t = this.hls.levels[e];
    if (!t)
      return;
    let s2 = t.subtitleGroups || null, i = this.groupIds, r2 = this.currentTrack;
    if (!s2 || i?.length !== s2?.length || s2 != null && s2.some((a2) => i?.indexOf(a2) === -1)) {
      this.groupIds = s2, this.trackId = -1, this.currentTrack = null;
      let a2 = this.tracks.filter((h2) => !s2 || s2.indexOf(h2.groupId) !== -1);
      if (a2.length)
        this.selectDefaultTrack && !a2.some((h2) => h2.default) && (this.selectDefaultTrack = false), a2.forEach((h2, u2) => {
          h2.id = u2;
        });
      else if (!r2 && !this.tracksInGroup.length)
        return;
      this.tracksInGroup = a2;
      let o = this.hls.config.subtitlePreference;
      if (!r2 && o) {
        this.selectDefaultTrack = false;
        let h2 = Le2(o, a2);
        if (h2 > -1)
          r2 = a2[h2];
        else {
          let u2 = Le2(o, this.tracks);
          r2 = this.tracks[u2];
        }
      }
      let l2 = this.findTrackId(r2);
      l2 === -1 && r2 && (l2 = this.findTrackId(null));
      let c = { subtitleTracks: a2 };
      this.log(`Updating subtitle tracks, ${a2.length} track(s) found in "${s2?.join(",")}" group-id`), this.hls.trigger(m2.SUBTITLE_TRACKS_UPDATED, c), l2 !== -1 && this.trackId === -1 && this.setSubtitleTrack(l2);
    } else
      this.shouldReloadPlaylist(r2) && this.setSubtitleTrack(this.trackId);
  }
  findTrackId(e) {
    let t = this.tracksInGroup, s2 = this.selectDefaultTrack;
    for (let i = 0; i < t.length; i++) {
      let r2 = t[i];
      if (!(s2 && !r2.default || !s2 && !e) && (!e || Ye(r2, e)))
        return i;
    }
    if (e) {
      for (let i = 0; i < t.length; i++) {
        let r2 = t[i];
        if (Xe(e.attrs, r2.attrs, ["LANGUAGE", "ASSOC-LANGUAGE", "CHARACTERISTICS"]))
          return i;
      }
      for (let i = 0; i < t.length; i++) {
        let r2 = t[i];
        if (Xe(e.attrs, r2.attrs, ["LANGUAGE"]))
          return i;
      }
    }
    return -1;
  }
  findTrackForTextTrack(e) {
    if (e) {
      let t = this.tracksInGroup;
      for (let s2 = 0; s2 < t.length; s2++) {
        let i = t[s2];
        if (ei(i, e))
          return s2;
      }
    }
    return -1;
  }
  onError(e, t) {
    t.fatal || !t.context || t.context.type === W3.SUBTITLE_TRACK && t.context.id === this.trackId && (!this.groupIds || this.groupIds.indexOf(t.context.groupId) !== -1) && this.checkRetry(t);
  }
  get allSubtitleTracks() {
    return this.tracks;
  }
  get subtitleTracks() {
    return this.tracksInGroup;
  }
  get subtitleTrack() {
    return this.trackId;
  }
  set subtitleTrack(e) {
    this.selectDefaultTrack = false, this.setSubtitleTrack(e);
  }
  setSubtitleOption(e) {
    if (this.hls.config.subtitlePreference = e, e) {
      let t = this.allSubtitleTracks;
      if (this.selectDefaultTrack = false, t.length) {
        let s2 = this.currentTrack;
        if (s2 && Ye(e, s2))
          return s2;
        let i = Le2(e, this.tracksInGroup);
        if (i > -1) {
          let r2 = this.tracksInGroup[i];
          return this.setSubtitleTrack(i), r2;
        } else {
          if (s2)
            return null;
          {
            let r2 = Le2(e, t);
            if (r2 > -1)
              return t[r2];
          }
        }
      }
    }
    return null;
  }
  loadPlaylist(e) {
    super.loadPlaylist();
    let t = this.currentTrack;
    if (this.shouldLoadPlaylist(t) && t) {
      let s2 = t.id, i = t.groupId, r2 = t.url;
      if (e)
        try {
          r2 = e.addDirectives(r2);
        } catch (a2) {
          this.warn(`Could not construct new URL with HLS Delivery Directives: ${a2}`);
        }
      this.log(`Loading subtitle playlist for id ${s2}`), this.hls.trigger(m2.SUBTITLE_TRACK_LOADING, { url: r2, id: s2, groupId: i, deliveryDirectives: e || null });
    }
  }
  toggleTrackModes() {
    let { media: e } = this;
    if (!e)
      return;
    let t = xt(e.textTracks), s2 = this.currentTrack, i;
    if (s2 && (i = t.filter((r2) => ei(s2, r2))[0], i || this.warn(`Unable to find subtitle TextTrack with name "${s2.name}" and language "${s2.lang}"`)), [].slice.call(t).forEach((r2) => {
      r2.mode !== "disabled" && r2 !== i && (r2.mode = "disabled");
    }), i) {
      let r2 = this.subtitleDisplay ? "showing" : "hidden";
      i.mode !== r2 && (i.mode = r2);
    }
  }
  setSubtitleTrack(e) {
    let t = this.tracksInGroup;
    if (!this.media) {
      this.queuedDefaultTrack = e;
      return;
    }
    if (e < -1 || e >= t.length || !F(e)) {
      this.warn(`Invalid subtitle track id: ${e}`);
      return;
    }
    this.clearTimer(), this.selectDefaultTrack = false;
    let s2 = this.currentTrack, i = t[e] || null;
    if (this.trackId = e, this.currentTrack = i, this.toggleTrackModes(), !i) {
      this.hls.trigger(m2.SUBTITLE_TRACK_SWITCH, { id: e });
      return;
    }
    let r2 = !!i.details && !i.details.live;
    if (e === this.trackId && i === s2 && r2)
      return;
    this.log(`Switching to subtitle-track ${e}` + (i ? ` "${i.name}" lang:${i.lang} group:${i.groupId}` : ""));
    let { id: a2, groupId: o = "", name: l2, type: c, url: h2 } = i;
    this.hls.trigger(m2.SUBTITLE_TRACK_SWITCH, { id: a2, groupId: o, name: l2, type: c, url: h2 });
    let u2 = this.switchParams(i.url, s2?.details, i.details);
    this.loadPlaylist(u2);
  }
};
var ai = class {
  constructor(e) {
    this.buffers = void 0, this.queues = { video: [], audio: [], audiovideo: [] }, this.buffers = e;
  }
  append(e, t, s2) {
    let i = this.queues[t];
    i.push(e), i.length === 1 && !s2 && this.executeNext(t);
  }
  insertAbort(e, t) {
    this.queues[t].unshift(e), this.executeNext(t);
  }
  appendBlocker(e) {
    let t, s2 = new Promise((r2) => {
      t = r2;
    }), i = { execute: t, onStart: () => {
    }, onComplete: () => {
    }, onError: () => {
    } };
    return this.append(i, e), s2;
  }
  executeNext(e) {
    let t = this.queues[e];
    if (t.length) {
      let s2 = t[0];
      try {
        s2.execute();
      } catch (i) {
        v2.warn(`[buffer-operation-queue]: Exception executing "${e}" SourceBuffer operation: ${i}`), s2.onError(i);
        let r2 = this.buffers[e];
        r2 != null && r2.updating || this.shiftAndExecuteNext(e);
      }
    }
  }
  shiftAndExecuteNext(e) {
    this.queues[e].shift(), this.executeNext(e);
  }
  current(e) {
    return this.queues[e][0];
  }
};
var Lr = /(avc[1234]|hvc1|hev1|dvh[1e]|vp09|av01)(?:\.[^.,]+)+/;
var oi = class {
  constructor(e) {
    this.details = null, this._objectUrl = null, this.operationQueue = void 0, this.listeners = void 0, this.hls = void 0, this.bufferCodecEventsExpected = 0, this._bufferCodecEventsTotal = 0, this.media = null, this.mediaSource = null, this.lastMpegAudioChunk = null, this.appendSource = void 0, this.appendErrors = { audio: 0, video: 0, audiovideo: 0 }, this.tracks = {}, this.pendingTracks = {}, this.sourceBuffer = void 0, this.log = void 0, this.warn = void 0, this.error = void 0, this._onEndStreaming = (s2) => {
      this.hls && this.hls.pauseBuffering();
    }, this._onStartStreaming = (s2) => {
      this.hls && this.hls.resumeBuffering();
    }, this._onMediaSourceOpen = () => {
      let { media: s2, mediaSource: i } = this;
      this.log("Media source opened"), s2 && (s2.removeEventListener("emptied", this._onMediaEmptied), this.updateMediaElementDuration(), this.hls.trigger(m2.MEDIA_ATTACHED, { media: s2, mediaSource: i })), i && i.removeEventListener("sourceopen", this._onMediaSourceOpen), this.checkPendingTracks();
    }, this._onMediaSourceClose = () => {
      this.log("Media source closed");
    }, this._onMediaSourceEnded = () => {
      this.log("Media source ended");
    }, this._onMediaEmptied = () => {
      let { mediaSrc: s2, _objectUrl: i } = this;
      s2 !== i && v2.error(`Media element src was set while attaching MediaSource (${i} > ${s2})`);
    }, this.hls = e;
    let t = "[buffer-controller]";
    this.appendSource = ka2(Ue2(e.config.preferManagedMediaSource)), this.log = v2.log.bind(v2, t), this.warn = v2.warn.bind(v2, t), this.error = v2.error.bind(v2, t), this._initSourceBuffer(), this.registerListeners();
  }
  hasSourceTypes() {
    return this.getSourceBufferTypes().length > 0 || Object.keys(this.pendingTracks).length > 0;
  }
  destroy() {
    this.unregisterListeners(), this.details = null, this.lastMpegAudioChunk = null, this.hls = null;
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.BUFFER_RESET, this.onBufferReset, this), e.on(m2.BUFFER_APPENDING, this.onBufferAppending, this), e.on(m2.BUFFER_CODECS, this.onBufferCodecs, this), e.on(m2.BUFFER_EOS, this.onBufferEos, this), e.on(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.on(m2.LEVEL_UPDATED, this.onLevelUpdated, this), e.on(m2.FRAG_PARSED, this.onFragParsed, this), e.on(m2.FRAG_CHANGED, this.onFragChanged, this);
  }
  unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.BUFFER_RESET, this.onBufferReset, this), e.off(m2.BUFFER_APPENDING, this.onBufferAppending, this), e.off(m2.BUFFER_CODECS, this.onBufferCodecs, this), e.off(m2.BUFFER_EOS, this.onBufferEos, this), e.off(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), e.off(m2.LEVEL_UPDATED, this.onLevelUpdated, this), e.off(m2.FRAG_PARSED, this.onFragParsed, this), e.off(m2.FRAG_CHANGED, this.onFragChanged, this);
  }
  _initSourceBuffer() {
    this.sourceBuffer = {}, this.operationQueue = new ai(this.sourceBuffer), this.listeners = { audio: [], video: [], audiovideo: [] }, this.appendErrors = { audio: 0, video: 0, audiovideo: 0 }, this.lastMpegAudioChunk = null;
  }
  onManifestLoading() {
    this.bufferCodecEventsExpected = this._bufferCodecEventsTotal = 0, this.details = null;
  }
  onManifestParsed(e, t) {
    let s2 = 2;
    (t.audio && !t.video || !t.altAudio) && (s2 = 1), this.bufferCodecEventsExpected = this._bufferCodecEventsTotal = s2, this.log(`${this.bufferCodecEventsExpected} bufferCodec event(s) expected`);
  }
  onMediaAttaching(e, t) {
    let s2 = this.media = t.media, i = Ue2(this.appendSource);
    if (s2 && i) {
      var r2;
      let a2 = this.mediaSource = new i();
      this.log(`created media source: ${(r2 = a2.constructor) == null ? void 0 : r2.name}`), a2.addEventListener("sourceopen", this._onMediaSourceOpen), a2.addEventListener("sourceended", this._onMediaSourceEnded), a2.addEventListener("sourceclose", this._onMediaSourceClose), this.appendSource && (a2.addEventListener("startstreaming", this._onStartStreaming), a2.addEventListener("endstreaming", this._onEndStreaming));
      let o = this._objectUrl = self.URL.createObjectURL(a2);
      if (this.appendSource)
        try {
          s2.removeAttribute("src");
          let l2 = self.ManagedMediaSource;
          s2.disableRemotePlayback = s2.disableRemotePlayback || l2 && a2 instanceof l2, Rr(s2), Xo(s2, o), s2.load();
        } catch {
          s2.src = o;
        }
      else
        s2.src = o;
      s2.addEventListener("emptied", this._onMediaEmptied);
    }
  }
  onMediaDetaching() {
    let { media: e, mediaSource: t, _objectUrl: s2 } = this;
    if (t) {
      if (this.log("media source detaching"), t.readyState === "open")
        try {
          t.endOfStream();
        } catch (i) {
          this.warn(`onMediaDetaching: ${i.message} while calling endOfStream`);
        }
      this.onBufferReset(), t.removeEventListener("sourceopen", this._onMediaSourceOpen), t.removeEventListener("sourceended", this._onMediaSourceEnded), t.removeEventListener("sourceclose", this._onMediaSourceClose), this.appendSource && (t.removeEventListener("startstreaming", this._onStartStreaming), t.removeEventListener("endstreaming", this._onEndStreaming)), e && (e.removeEventListener("emptied", this._onMediaEmptied), s2 && self.URL.revokeObjectURL(s2), this.mediaSrc === s2 ? (e.removeAttribute("src"), this.appendSource && Rr(e), e.load()) : this.warn("media|source.src was changed by a third party - skip cleanup")), this.mediaSource = null, this.media = null, this._objectUrl = null, this.bufferCodecEventsExpected = this._bufferCodecEventsTotal, this.pendingTracks = {}, this.tracks = {};
    }
    this.hls.trigger(m2.MEDIA_DETACHED, void 0);
  }
  onBufferReset() {
    this.getSourceBufferTypes().forEach((e) => {
      this.resetBuffer(e);
    }), this._initSourceBuffer();
  }
  resetBuffer(e) {
    let t = this.sourceBuffer[e];
    try {
      if (t) {
        var s2;
        this.removeBufferListeners(e), this.sourceBuffer[e] = void 0, (s2 = this.mediaSource) != null && s2.sourceBuffers.length && this.mediaSource.removeSourceBuffer(t);
      }
    } catch (i) {
      this.warn(`onBufferReset ${e}`, i);
    }
  }
  onBufferCodecs(e, t) {
    let s2 = this.getSourceBufferTypes().length, i = Object.keys(t);
    if (i.forEach((a2) => {
      if (s2) {
        let l2 = this.tracks[a2];
        if (l2 && typeof l2.buffer.changeType == "function") {
          var o;
          let { id: c, codec: h2, levelCodec: u2, container: d3, metadata: f3 } = t[a2], g2 = Qi(l2.codec, l2.levelCodec), p3 = g2?.replace(Lr, "$1"), T2 = Qi(h2, u2), E4 = (o = T2) == null ? void 0 : o.replace(Lr, "$1");
          if (T2 && p3 !== E4) {
            a2.slice(0, 5) === "audio" && (T2 = Pt(T2, this.appendSource));
            let x3 = `${d3};codecs=${T2}`;
            this.appendChangeType(a2, x3), this.log(`switching codec ${g2} to ${T2}`), this.tracks[a2] = { buffer: l2.buffer, codec: h2, container: d3, levelCodec: u2, metadata: f3, id: c };
          }
        }
      } else
        this.pendingTracks[a2] = t[a2];
    }), s2)
      return;
    let r2 = Math.max(this.bufferCodecEventsExpected - 1, 0);
    this.bufferCodecEventsExpected !== r2 && (this.log(`${r2} bufferCodec event(s) expected ${i.join(",")}`), this.bufferCodecEventsExpected = r2), this.mediaSource && this.mediaSource.readyState === "open" && this.checkPendingTracks();
  }
  appendChangeType(e, t) {
    let { operationQueue: s2 } = this, i = { execute: () => {
      let r2 = this.sourceBuffer[e];
      r2 && (this.log(`changing ${e} sourceBuffer type to ${t}`), r2.changeType(t)), s2.shiftAndExecuteNext(e);
    }, onStart: () => {
    }, onComplete: () => {
    }, onError: (r2) => {
      this.warn(`Failed to change ${e} SourceBuffer type`, r2);
    } };
    s2.append(i, e, !!this.pendingTracks[e]);
  }
  onBufferAppending(e, t) {
    let { hls: s2, operationQueue: i, tracks: r2 } = this, { data: a2, type: o, frag: l2, part: c, chunkMeta: h2 } = t, u2 = h2.buffering[o], d3 = self.performance.now();
    u2.start = d3;
    let f3 = l2.stats.buffering, g2 = c ? c.stats.buffering : null;
    f3.start === 0 && (f3.start = d3), g2 && g2.start === 0 && (g2.start = d3);
    let p3 = r2.audio, T2 = false;
    o === "audio" && p3?.container === "audio/mpeg" && (T2 = !this.lastMpegAudioChunk || h2.id === 1 || this.lastMpegAudioChunk.sn !== h2.sn, this.lastMpegAudioChunk = h2);
    let E4 = l2.start, x3 = { execute: () => {
      if (u2.executeStart = self.performance.now(), T2) {
        let y3 = this.sourceBuffer[o];
        if (y3) {
          let R = E4 - y3.timestampOffset;
          Math.abs(R) >= 0.1 && (this.log(`Updating audio SourceBuffer timestampOffset to ${E4} (delta: ${R}) sn: ${l2.sn})`), y3.timestampOffset = E4);
        }
      }
      this.appendExecutor(a2, o);
    }, onStart: () => {
    }, onComplete: () => {
      let y3 = self.performance.now();
      u2.executeEnd = u2.end = y3, f3.first === 0 && (f3.first = y3), g2 && g2.first === 0 && (g2.first = y3);
      let { sourceBuffer: R } = this, S2 = {};
      for (let b in R)
        S2[b] = Q.getBuffered(R[b]);
      this.appendErrors[o] = 0, o === "audio" || o === "video" ? this.appendErrors.audiovideo = 0 : (this.appendErrors.audio = 0, this.appendErrors.video = 0), this.hls.trigger(m2.BUFFER_APPENDED, { type: o, frag: l2, part: c, chunkMeta: h2, parent: l2.type, timeRanges: S2 });
    }, onError: (y3) => {
      let R = { type: B2.MEDIA_ERROR, parent: l2.type, details: A2.BUFFER_APPEND_ERROR, sourceBufferName: o, frag: l2, part: c, chunkMeta: h2, error: y3, err: y3, fatal: false };
      if (y3.code === DOMException.QUOTA_EXCEEDED_ERR)
        R.details = A2.BUFFER_FULL_ERROR;
      else {
        let S2 = ++this.appendErrors[o];
        R.details = A2.BUFFER_APPEND_ERROR, this.warn(`Failed ${S2}/${s2.config.appendErrorMaxRetry} times to append segment in "${o}" sourceBuffer`), S2 >= s2.config.appendErrorMaxRetry && (R.fatal = true);
      }
      s2.trigger(m2.ERROR, R);
    } };
    i.append(x3, o, !!this.pendingTracks[o]);
  }
  onBufferFlushing(e, t) {
    let { operationQueue: s2 } = this, i = (r2) => ({ execute: this.removeExecutor.bind(this, r2, t.startOffset, t.endOffset), onStart: () => {
    }, onComplete: () => {
      this.hls.trigger(m2.BUFFER_FLUSHED, { type: r2 });
    }, onError: (a2) => {
      this.warn(`Failed to remove from ${r2} SourceBuffer`, a2);
    } });
    t.type ? s2.append(i(t.type), t.type) : this.getSourceBufferTypes().forEach((r2) => {
      s2.append(i(r2), r2);
    });
  }
  onFragParsed(e, t) {
    let { frag: s2, part: i } = t, r2 = [], a2 = i ? i.elementaryStreams : s2.elementaryStreams;
    a2[z2.AUDIOVIDEO] ? r2.push("audiovideo") : (a2[z2.AUDIO] && r2.push("audio"), a2[z2.VIDEO] && r2.push("video"));
    let o = () => {
      let l2 = self.performance.now();
      s2.stats.buffering.end = l2, i && (i.stats.buffering.end = l2);
      let c = i ? i.stats : s2.stats;
      this.hls.trigger(m2.FRAG_BUFFERED, { frag: s2, part: i, stats: c, id: s2.type });
    };
    r2.length === 0 && this.warn(`Fragments must have at least one ElementaryStreamType set. type: ${s2.type} level: ${s2.level} sn: ${s2.sn}`), this.blockBuffers(o, r2);
  }
  onFragChanged(e, t) {
    this.trimBuffers();
  }
  onBufferEos(e, t) {
    this.getSourceBufferTypes().reduce((i, r2) => {
      let a2 = this.sourceBuffer[r2];
      return a2 && (!t.type || t.type === r2) && (a2.ending = true, a2.ended || (a2.ended = true, this.log(`${r2} sourceBuffer now EOS`))), i && !!(!a2 || a2.ended);
    }, true) && (this.log("Queueing mediaSource.endOfStream()"), this.blockBuffers(() => {
      this.getSourceBufferTypes().forEach((r2) => {
        let a2 = this.sourceBuffer[r2];
        a2 && (a2.ending = false);
      });
      let { mediaSource: i } = this;
      if (!i || i.readyState !== "open") {
        i && this.log(`Could not call mediaSource.endOfStream(). mediaSource.readyState: ${i.readyState}`);
        return;
      }
      this.log("Calling mediaSource.endOfStream()"), i.endOfStream();
    }));
  }
  onLevelUpdated(e, { details: t }) {
    t.fragments.length && (this.details = t, this.getSourceBufferTypes().length ? this.blockBuffers(this.updateMediaElementDuration.bind(this)) : this.updateMediaElementDuration());
  }
  trimBuffers() {
    let { hls: e, details: t, media: s2 } = this;
    if (!s2 || t === null || !this.getSourceBufferTypes().length)
      return;
    let r2 = e.config, a2 = s2.currentTime, o = t.levelTargetDuration, l2 = t.live && r2.liveBackBufferLength !== null ? r2.liveBackBufferLength : r2.backBufferLength;
    if (F(l2) && l2 > 0) {
      let c = Math.max(l2, o), h2 = Math.floor(a2 / o) * o - c;
      this.flushBackBuffer(a2, o, h2);
    }
    if (F(r2.frontBufferFlushThreshold) && r2.frontBufferFlushThreshold > 0) {
      let c = Math.max(r2.maxBufferLength, r2.frontBufferFlushThreshold), h2 = Math.max(c, o), u2 = Math.floor(a2 / o) * o + h2;
      this.flushFrontBuffer(a2, o, u2);
    }
  }
  flushBackBuffer(e, t, s2) {
    let { details: i, sourceBuffer: r2 } = this;
    this.getSourceBufferTypes().forEach((o) => {
      let l2 = r2[o];
      if (l2) {
        let c = Q.getBuffered(l2);
        if (c.length > 0 && s2 > c.start(0)) {
          if (this.hls.trigger(m2.BACK_BUFFER_REACHED, { bufferEnd: s2 }), i != null && i.live)
            this.hls.trigger(m2.LIVE_BACK_BUFFER_REACHED, { bufferEnd: s2 });
          else if (l2.ended && c.end(c.length - 1) - e < t * 2) {
            this.log(`Cannot flush ${o} back buffer while SourceBuffer is in ended state`);
            return;
          }
          this.hls.trigger(m2.BUFFER_FLUSHING, { startOffset: 0, endOffset: s2, type: o });
        }
      }
    });
  }
  flushFrontBuffer(e, t, s2) {
    let { sourceBuffer: i } = this;
    this.getSourceBufferTypes().forEach((a2) => {
      let o = i[a2];
      if (o) {
        let l2 = Q.getBuffered(o), c = l2.length;
        if (c < 2)
          return;
        let h2 = l2.start(c - 1), u2 = l2.end(c - 1);
        if (s2 > h2 || e >= h2 && e <= u2)
          return;
        if (o.ended && e - u2 < 2 * t) {
          this.log(`Cannot flush ${a2} front buffer while SourceBuffer is in ended state`);
          return;
        }
        this.hls.trigger(m2.BUFFER_FLUSHING, { startOffset: h2, endOffset: 1 / 0, type: a2 });
      }
    });
  }
  updateMediaElementDuration() {
    if (!this.details || !this.media || !this.mediaSource || this.mediaSource.readyState !== "open")
      return;
    let { details: e, hls: t, media: s2, mediaSource: i } = this, r2 = e.fragments[0].start + e.totalduration, a2 = s2.duration, o = F(i.duration) ? i.duration : 0;
    e.live && t.config.liveDurationInfinity ? (i.duration = 1 / 0, this.updateSeekableRange(e)) : (r2 > o && r2 > a2 || !F(a2)) && (this.log(`Updating Media Source duration to ${r2.toFixed(3)}`), i.duration = r2);
  }
  updateSeekableRange(e) {
    let t = this.mediaSource, s2 = e.fragments;
    if (s2.length && e.live && t != null && t.setLiveSeekableRange) {
      let r2 = Math.max(0, s2[0].start), a2 = Math.max(r2, r2 + e.totalduration);
      this.log(`Media Source duration is set to ${t.duration}. Setting seekable range to ${r2}-${a2}.`), t.setLiveSeekableRange(r2, a2);
    }
  }
  checkPendingTracks() {
    let { bufferCodecEventsExpected: e, operationQueue: t, pendingTracks: s2 } = this, i = Object.keys(s2).length;
    if (i && (!e || i === 2 || "audiovideo" in s2)) {
      this.createSourceBuffers(s2), this.pendingTracks = {};
      let r2 = this.getSourceBufferTypes();
      if (r2.length)
        this.hls.trigger(m2.BUFFER_CREATED, { tracks: this.tracks }), r2.forEach((a2) => {
          t.executeNext(a2);
        });
      else {
        let a2 = new Error("could not create source buffer for media codec(s)");
        this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_INCOMPATIBLE_CODECS_ERROR, fatal: true, error: a2, reason: a2.message });
      }
    }
  }
  createSourceBuffers(e) {
    let { sourceBuffer: t, mediaSource: s2 } = this;
    if (!s2)
      throw Error("createSourceBuffers called when mediaSource was null");
    for (let r2 in e)
      if (!t[r2]) {
        var i;
        let a2 = e[r2];
        if (!a2)
          throw Error(`source buffer exists for track ${r2}, however track does not`);
        let o = ((i = a2.levelCodec) == null ? void 0 : i.indexOf(",")) === -1 ? a2.levelCodec : a2.codec;
        o && r2.slice(0, 5) === "audio" && (o = Pt(o, this.appendSource));
        let l2 = `${a2.container};codecs=${o}`;
        this.log(`creating sourceBuffer(${l2})`);
        try {
          let c = t[r2] = s2.addSourceBuffer(l2), h2 = r2;
          this.addBufferListener(h2, "updatestart", this._onSBUpdateStart), this.addBufferListener(h2, "updateend", this._onSBUpdateEnd), this.addBufferListener(h2, "error", this._onSBUpdateError), this.appendSource && this.addBufferListener(h2, "bufferedchange", (u2, d3) => {
            let f3 = d3.removedRanges;
            f3 != null && f3.length && this.hls.trigger(m2.BUFFER_FLUSHED, { type: r2 });
          }), this.tracks[r2] = { buffer: c, codec: o, container: a2.container, levelCodec: a2.levelCodec, metadata: a2.metadata, id: a2.id };
        } catch (c) {
          this.error(`error while trying to add sourceBuffer: ${c.message}`), this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_ADD_CODEC_ERROR, fatal: false, error: c, sourceBufferName: r2, mimeType: l2 });
        }
      }
  }
  get mediaSrc() {
    var e;
    let t = ((e = this.media) == null ? void 0 : e.firstChild) || this.media;
    return t?.src;
  }
  _onSBUpdateStart(e) {
    let { operationQueue: t } = this;
    t.current(e).onStart();
  }
  _onSBUpdateEnd(e) {
    var t;
    if (((t = this.mediaSource) == null ? void 0 : t.readyState) === "closed") {
      this.resetBuffer(e);
      return;
    }
    let { operationQueue: s2 } = this;
    s2.current(e).onComplete(), s2.shiftAndExecuteNext(e);
  }
  _onSBUpdateError(e, t) {
    var s2;
    let i = new Error(`${e} SourceBuffer error. MediaSource readyState: ${(s2 = this.mediaSource) == null ? void 0 : s2.readyState}`);
    this.error(`${i}`, t), this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_APPENDING_ERROR, sourceBufferName: e, error: i, fatal: false });
    let r2 = this.operationQueue.current(e);
    r2 && r2.onError(i);
  }
  removeExecutor(e, t, s2) {
    let { media: i, mediaSource: r2, operationQueue: a2, sourceBuffer: o } = this, l2 = o[e];
    if (!i || !r2 || !l2) {
      this.warn(`Attempting to remove from the ${e} SourceBuffer, but it does not exist`), a2.shiftAndExecuteNext(e);
      return;
    }
    let c = F(i.duration) ? i.duration : 1 / 0, h2 = F(r2.duration) ? r2.duration : 1 / 0, u2 = Math.max(0, t), d3 = Math.min(s2, c, h2);
    d3 > u2 && (!l2.ending || l2.ended) ? (l2.ended = false, this.log(`Removing [${u2},${d3}] from the ${e} SourceBuffer`), l2.remove(u2, d3)) : a2.shiftAndExecuteNext(e);
  }
  appendExecutor(e, t) {
    let s2 = this.sourceBuffer[t];
    if (!s2) {
      if (!this.pendingTracks[t])
        throw new Error(`Attempting to append to the ${t} SourceBuffer, but it does not exist`);
      return;
    }
    s2.ended = false, s2.appendBuffer(e);
  }
  blockBuffers(e, t = this.getSourceBufferTypes()) {
    if (!t.length) {
      this.log("Blocking operation requested, but no SourceBuffers exist"), Promise.resolve().then(e);
      return;
    }
    let { operationQueue: s2 } = this, i = t.map((r2) => s2.appendBlocker(r2));
    Promise.all(i).then(() => {
      e(), t.forEach((r2) => {
        let a2 = this.sourceBuffer[r2];
        a2 != null && a2.updating || s2.shiftAndExecuteNext(r2);
      });
    });
  }
  getSourceBufferTypes() {
    return Object.keys(this.sourceBuffer);
  }
  addBufferListener(e, t, s2) {
    let i = this.sourceBuffer[e];
    if (!i)
      return;
    let r2 = s2.bind(this, e);
    this.listeners[e].push({ event: t, listener: r2 }), i.addEventListener(t, r2);
  }
  removeBufferListeners(e) {
    let t = this.sourceBuffer[e];
    t && this.listeners[e].forEach((s2) => {
      t.removeEventListener(s2.event, s2.listener);
    });
  }
};
function Rr(n12) {
  let e = n12.querySelectorAll("source");
  [].slice.call(e).forEach((t) => {
    n12.removeChild(t);
  });
}
function Xo(n12, e) {
  let t = self.document.createElement("source");
  t.type = "video/mp4", t.src = e, n12.appendChild(t);
}
var Ir = { 42: 225, 92: 233, 94: 237, 95: 243, 96: 250, 123: 231, 124: 247, 125: 209, 126: 241, 127: 9608, 128: 174, 129: 176, 130: 189, 131: 191, 132: 8482, 133: 162, 134: 163, 135: 9834, 136: 224, 137: 32, 138: 232, 139: 226, 140: 234, 141: 238, 142: 244, 143: 251, 144: 193, 145: 201, 146: 211, 147: 218, 148: 220, 149: 252, 150: 8216, 151: 161, 152: 42, 153: 8217, 154: 9473, 155: 169, 156: 8480, 157: 8226, 158: 8220, 159: 8221, 160: 192, 161: 194, 162: 199, 163: 200, 164: 202, 165: 203, 166: 235, 167: 206, 168: 207, 169: 239, 170: 212, 171: 217, 172: 249, 173: 219, 174: 171, 175: 187, 176: 195, 177: 227, 178: 205, 179: 204, 180: 236, 181: 210, 182: 242, 183: 213, 184: 245, 185: 123, 186: 125, 187: 92, 188: 94, 189: 95, 190: 124, 191: 8764, 192: 196, 193: 228, 194: 214, 195: 246, 196: 223, 197: 165, 198: 164, 199: 9475, 200: 197, 201: 229, 202: 216, 203: 248, 204: 9487, 205: 9491, 206: 9495, 207: 9499 };
var In = function(e) {
  let t = e;
  return Ir.hasOwnProperty(e) && (t = Ir[e]), String.fromCharCode(t);
};
var Ee3 = 15;
var De2 = 100;
var Qo = { 17: 1, 18: 3, 21: 5, 22: 7, 23: 9, 16: 11, 19: 12, 20: 14 };
var Jo = { 17: 2, 18: 4, 21: 6, 22: 8, 23: 10, 19: 13, 20: 15 };
var Zo = { 25: 1, 26: 3, 29: 5, 30: 7, 31: 9, 24: 11, 27: 12, 28: 14 };
var el = { 25: 2, 26: 4, 29: 6, 30: 8, 31: 10, 27: 13, 28: 15 };
var tl2 = ["white", "green", "blue", "cyan", "red", "yellow", "magenta", "black", "transparent"];
var li = class {
  constructor() {
    this.time = null, this.verboseLevel = 0;
  }
  log(e, t) {
    if (this.verboseLevel >= e) {
      let s2 = typeof t == "function" ? t() : t;
      v2.log(`${this.time} [${e}] ${s2}`);
    }
  }
};
var ke3 = function(e) {
  let t = [];
  for (let s2 = 0; s2 < e.length; s2++)
    t.push(e[s2].toString(16));
  return t;
};
var Yt = class {
  constructor() {
    this.foreground = "white", this.underline = false, this.italics = false, this.background = "black", this.flash = false;
  }
  reset() {
    this.foreground = "white", this.underline = false, this.italics = false, this.background = "black", this.flash = false;
  }
  setStyles(e) {
    let t = ["foreground", "underline", "italics", "background", "flash"];
    for (let s2 = 0; s2 < t.length; s2++) {
      let i = t[s2];
      e.hasOwnProperty(i) && (this[i] = e[i]);
    }
  }
  isDefault() {
    return this.foreground === "white" && !this.underline && !this.italics && this.background === "black" && !this.flash;
  }
  equals(e) {
    return this.foreground === e.foreground && this.underline === e.underline && this.italics === e.italics && this.background === e.background && this.flash === e.flash;
  }
  copy(e) {
    this.foreground = e.foreground, this.underline = e.underline, this.italics = e.italics, this.background = e.background, this.flash = e.flash;
  }
  toString() {
    return "color=" + this.foreground + ", underline=" + this.underline + ", italics=" + this.italics + ", background=" + this.background + ", flash=" + this.flash;
  }
};
var ci = class {
  constructor() {
    this.uchar = " ", this.penState = new Yt();
  }
  reset() {
    this.uchar = " ", this.penState.reset();
  }
  setChar(e, t) {
    this.uchar = e, this.penState.copy(t);
  }
  setPenState(e) {
    this.penState.copy(e);
  }
  equals(e) {
    return this.uchar === e.uchar && this.penState.equals(e.penState);
  }
  copy(e) {
    this.uchar = e.uchar, this.penState.copy(e.penState);
  }
  isEmpty() {
    return this.uchar === " " && this.penState.isDefault();
  }
};
var hi = class {
  constructor(e) {
    this.chars = [], this.pos = 0, this.currPenState = new Yt(), this.cueStartTime = null, this.logger = void 0;
    for (let t = 0; t < De2; t++)
      this.chars.push(new ci());
    this.logger = e;
  }
  equals(e) {
    for (let t = 0; t < De2; t++)
      if (!this.chars[t].equals(e.chars[t]))
        return false;
    return true;
  }
  copy(e) {
    for (let t = 0; t < De2; t++)
      this.chars[t].copy(e.chars[t]);
  }
  isEmpty() {
    let e = true;
    for (let t = 0; t < De2; t++)
      if (!this.chars[t].isEmpty()) {
        e = false;
        break;
      }
    return e;
  }
  setCursor(e) {
    this.pos !== e && (this.pos = e), this.pos < 0 ? (this.logger.log(3, "Negative cursor position " + this.pos), this.pos = 0) : this.pos > De2 && (this.logger.log(3, "Too large cursor position " + this.pos), this.pos = De2);
  }
  moveCursor(e) {
    let t = this.pos + e;
    if (e > 1)
      for (let s2 = this.pos + 1; s2 < t + 1; s2++)
        this.chars[s2].setPenState(this.currPenState);
    this.setCursor(t);
  }
  backSpace() {
    this.moveCursor(-1), this.chars[this.pos].setChar(" ", this.currPenState);
  }
  insertChar(e) {
    e >= 144 && this.backSpace();
    let t = In(e);
    if (this.pos >= De2) {
      this.logger.log(0, () => "Cannot insert " + e.toString(16) + " (" + t + ") at position " + this.pos + ". Skipping it!");
      return;
    }
    this.chars[this.pos].setChar(t, this.currPenState), this.moveCursor(1);
  }
  clearFromPos(e) {
    let t;
    for (t = e; t < De2; t++)
      this.chars[t].reset();
  }
  clear() {
    this.clearFromPos(0), this.pos = 0, this.currPenState.reset();
  }
  clearToEndOfRow() {
    this.clearFromPos(this.pos);
  }
  getTextString() {
    let e = [], t = true;
    for (let s2 = 0; s2 < De2; s2++) {
      let i = this.chars[s2].uchar;
      i !== " " && (t = false), e.push(i);
    }
    return t ? "" : e.join("");
  }
  setPenStyles(e) {
    this.currPenState.setStyles(e), this.chars[this.pos].setPenState(this.currPenState);
  }
};
var tt = class {
  constructor(e) {
    this.rows = [], this.currRow = Ee3 - 1, this.nrRollUpRows = null, this.lastOutputScreen = null, this.logger = void 0;
    for (let t = 0; t < Ee3; t++)
      this.rows.push(new hi(e));
    this.logger = e;
  }
  reset() {
    for (let e = 0; e < Ee3; e++)
      this.rows[e].clear();
    this.currRow = Ee3 - 1;
  }
  equals(e) {
    let t = true;
    for (let s2 = 0; s2 < Ee3; s2++)
      if (!this.rows[s2].equals(e.rows[s2])) {
        t = false;
        break;
      }
    return t;
  }
  copy(e) {
    for (let t = 0; t < Ee3; t++)
      this.rows[t].copy(e.rows[t]);
  }
  isEmpty() {
    let e = true;
    for (let t = 0; t < Ee3; t++)
      if (!this.rows[t].isEmpty()) {
        e = false;
        break;
      }
    return e;
  }
  backSpace() {
    this.rows[this.currRow].backSpace();
  }
  clearToEndOfRow() {
    this.rows[this.currRow].clearToEndOfRow();
  }
  insertChar(e) {
    this.rows[this.currRow].insertChar(e);
  }
  setPen(e) {
    this.rows[this.currRow].setPenStyles(e);
  }
  moveCursor(e) {
    this.rows[this.currRow].moveCursor(e);
  }
  setCursor(e) {
    this.logger.log(2, "setCursor: " + e), this.rows[this.currRow].setCursor(e);
  }
  setPAC(e) {
    this.logger.log(2, () => "pacData = " + JSON.stringify(e));
    let t = e.row - 1;
    if (this.nrRollUpRows && t < this.nrRollUpRows - 1 && (t = this.nrRollUpRows - 1), this.nrRollUpRows && this.currRow !== t) {
      for (let o = 0; o < Ee3; o++)
        this.rows[o].clear();
      let r2 = this.currRow + 1 - this.nrRollUpRows, a2 = this.lastOutputScreen;
      if (a2) {
        let o = a2.rows[r2].cueStartTime, l2 = this.logger.time;
        if (o !== null && l2 !== null && o < l2)
          for (let c = 0; c < this.nrRollUpRows; c++)
            this.rows[t - this.nrRollUpRows + c + 1].copy(a2.rows[r2 + c]);
      }
    }
    this.currRow = t;
    let s2 = this.rows[this.currRow];
    if (e.indent !== null) {
      let r2 = e.indent, a2 = Math.max(r2 - 1, 0);
      s2.setCursor(e.indent), e.color = s2.chars[a2].penState.foreground;
    }
    let i = { foreground: e.color, underline: e.underline, italics: e.italics, background: "black", flash: false };
    this.setPen(i);
  }
  setBkgData(e) {
    this.logger.log(2, () => "bkgData = " + JSON.stringify(e)), this.backSpace(), this.setPen(e), this.insertChar(32);
  }
  setRollUpRows(e) {
    this.nrRollUpRows = e;
  }
  rollUp() {
    if (this.nrRollUpRows === null) {
      this.logger.log(3, "roll_up but nrRollUpRows not set yet");
      return;
    }
    this.logger.log(1, () => this.getDisplayText());
    let e = this.currRow + 1 - this.nrRollUpRows, t = this.rows.splice(e, 1)[0];
    t.clear(), this.rows.splice(this.currRow, 0, t), this.logger.log(2, "Rolling up");
  }
  getDisplayText(e) {
    e = e || false;
    let t = [], s2 = "", i = -1;
    for (let r2 = 0; r2 < Ee3; r2++) {
      let a2 = this.rows[r2].getTextString();
      a2 && (i = r2 + 1, e ? t.push("Row " + i + ": '" + a2 + "'") : t.push(a2.trim()));
    }
    return t.length > 0 && (e ? s2 = "[" + t.join(" | ") + "]" : s2 = t.join(`
`)), s2;
  }
  getTextAndFormat() {
    return this.rows;
  }
};
var qt = class {
  constructor(e, t, s2) {
    this.chNr = void 0, this.outputFilter = void 0, this.mode = void 0, this.verbose = void 0, this.displayedMemory = void 0, this.nonDisplayedMemory = void 0, this.lastOutputScreen = void 0, this.currRollUpRow = void 0, this.writeScreen = void 0, this.cueStartTime = void 0, this.logger = void 0, this.chNr = e, this.outputFilter = t, this.mode = null, this.verbose = 0, this.displayedMemory = new tt(s2), this.nonDisplayedMemory = new tt(s2), this.lastOutputScreen = new tt(s2), this.currRollUpRow = this.displayedMemory.rows[Ee3 - 1], this.writeScreen = this.displayedMemory, this.mode = null, this.cueStartTime = null, this.logger = s2;
  }
  reset() {
    this.mode = null, this.displayedMemory.reset(), this.nonDisplayedMemory.reset(), this.lastOutputScreen.reset(), this.outputFilter.reset(), this.currRollUpRow = this.displayedMemory.rows[Ee3 - 1], this.writeScreen = this.displayedMemory, this.mode = null, this.cueStartTime = null;
  }
  getHandler() {
    return this.outputFilter;
  }
  setHandler(e) {
    this.outputFilter = e;
  }
  setPAC(e) {
    this.writeScreen.setPAC(e);
  }
  setBkgData(e) {
    this.writeScreen.setBkgData(e);
  }
  setMode(e) {
    e !== this.mode && (this.mode = e, this.logger.log(2, () => "MODE=" + e), this.mode === "MODE_POP-ON" ? this.writeScreen = this.nonDisplayedMemory : (this.writeScreen = this.displayedMemory, this.writeScreen.reset()), this.mode !== "MODE_ROLL-UP" && (this.displayedMemory.nrRollUpRows = null, this.nonDisplayedMemory.nrRollUpRows = null), this.mode = e);
  }
  insertChars(e) {
    for (let s2 = 0; s2 < e.length; s2++)
      this.writeScreen.insertChar(e[s2]);
    let t = this.writeScreen === this.displayedMemory ? "DISP" : "NON_DISP";
    this.logger.log(2, () => t + ": " + this.writeScreen.getDisplayText(true)), (this.mode === "MODE_PAINT-ON" || this.mode === "MODE_ROLL-UP") && (this.logger.log(1, () => "DISPLAYED: " + this.displayedMemory.getDisplayText(true)), this.outputDataUpdate());
  }
  ccRCL() {
    this.logger.log(2, "RCL - Resume Caption Loading"), this.setMode("MODE_POP-ON");
  }
  ccBS() {
    this.logger.log(2, "BS - BackSpace"), this.mode !== "MODE_TEXT" && (this.writeScreen.backSpace(), this.writeScreen === this.displayedMemory && this.outputDataUpdate());
  }
  ccAOF() {
  }
  ccAON() {
  }
  ccDER() {
    this.logger.log(2, "DER- Delete to End of Row"), this.writeScreen.clearToEndOfRow(), this.outputDataUpdate();
  }
  ccRU(e) {
    this.logger.log(2, "RU(" + e + ") - Roll Up"), this.writeScreen = this.displayedMemory, this.setMode("MODE_ROLL-UP"), this.writeScreen.setRollUpRows(e);
  }
  ccFON() {
    this.logger.log(2, "FON - Flash On"), this.writeScreen.setPen({ flash: true });
  }
  ccRDC() {
    this.logger.log(2, "RDC - Resume Direct Captioning"), this.setMode("MODE_PAINT-ON");
  }
  ccTR() {
    this.logger.log(2, "TR"), this.setMode("MODE_TEXT");
  }
  ccRTD() {
    this.logger.log(2, "RTD"), this.setMode("MODE_TEXT");
  }
  ccEDM() {
    this.logger.log(2, "EDM - Erase Displayed Memory"), this.displayedMemory.reset(), this.outputDataUpdate(true);
  }
  ccCR() {
    this.logger.log(2, "CR - Carriage Return"), this.writeScreen.rollUp(), this.outputDataUpdate(true);
  }
  ccENM() {
    this.logger.log(2, "ENM - Erase Non-displayed Memory"), this.nonDisplayedMemory.reset();
  }
  ccEOC() {
    if (this.logger.log(2, "EOC - End Of Caption"), this.mode === "MODE_POP-ON") {
      let e = this.displayedMemory;
      this.displayedMemory = this.nonDisplayedMemory, this.nonDisplayedMemory = e, this.writeScreen = this.nonDisplayedMemory, this.logger.log(1, () => "DISP: " + this.displayedMemory.getDisplayText());
    }
    this.outputDataUpdate(true);
  }
  ccTO(e) {
    this.logger.log(2, "TO(" + e + ") - Tab Offset"), this.writeScreen.moveCursor(e);
  }
  ccMIDROW(e) {
    let t = { flash: false };
    if (t.underline = e % 2 === 1, t.italics = e >= 46, t.italics)
      t.foreground = "white";
    else {
      let s2 = Math.floor(e / 2) - 16, i = ["white", "green", "blue", "cyan", "red", "yellow", "magenta"];
      t.foreground = i[s2];
    }
    this.logger.log(2, "MIDROW: " + JSON.stringify(t)), this.writeScreen.setPen(t);
  }
  outputDataUpdate(e = false) {
    let t = this.logger.time;
    t !== null && this.outputFilter && (this.cueStartTime === null && !this.displayedMemory.isEmpty() ? this.cueStartTime = t : this.displayedMemory.equals(this.lastOutputScreen) || (this.outputFilter.newCue(this.cueStartTime, t, this.lastOutputScreen), e && this.outputFilter.dispatchCue && this.outputFilter.dispatchCue(), this.cueStartTime = this.displayedMemory.isEmpty() ? null : t), this.lastOutputScreen.copy(this.displayedMemory));
  }
  cueSplitAtTime(e) {
    this.outputFilter && (this.displayedMemory.isEmpty() || (this.outputFilter.newCue && this.outputFilter.newCue(this.cueStartTime, e, this.displayedMemory), this.cueStartTime = e));
  }
};
var jt = class {
  constructor(e, t, s2) {
    this.channels = void 0, this.currentChannel = 0, this.cmdHistory = Dr(), this.logger = void 0;
    let i = this.logger = new li();
    this.channels = [null, new qt(e, t, i), new qt(e + 1, s2, i)];
  }
  getHandler(e) {
    return this.channels[e].getHandler();
  }
  setHandler(e, t) {
    this.channels[e].setHandler(t);
  }
  addData(e, t) {
    let s2, i, r2, a2 = false;
    this.logger.time = e;
    for (let o = 0; o < t.length; o += 2)
      if (i = t[o] & 127, r2 = t[o + 1] & 127, !(i === 0 && r2 === 0)) {
        if (this.logger.log(3, "[" + ke3([t[o], t[o + 1]]) + "] -> (" + ke3([i, r2]) + ")"), s2 = this.parseCmd(i, r2), s2 || (s2 = this.parseMidrow(i, r2)), s2 || (s2 = this.parsePAC(i, r2)), s2 || (s2 = this.parseBackgroundAttributes(i, r2)), !s2 && (a2 = this.parseChars(i, r2), a2)) {
          let l2 = this.currentChannel;
          l2 && l2 > 0 ? this.channels[l2].insertChars(a2) : this.logger.log(2, "No channel found yet. TEXT-MODE?");
        }
        !s2 && !a2 && this.logger.log(2, "Couldn't parse cleaned data " + ke3([i, r2]) + " orig: " + ke3([t[o], t[o + 1]]));
      }
  }
  parseCmd(e, t) {
    let { cmdHistory: s2 } = this, i = (e === 20 || e === 28 || e === 21 || e === 29) && t >= 32 && t <= 47, r2 = (e === 23 || e === 31) && t >= 33 && t <= 35;
    if (!(i || r2))
      return false;
    if (br(e, t, s2))
      return He2(null, null, s2), this.logger.log(3, "Repeated command (" + ke3([e, t]) + ") is dropped"), true;
    let a2 = e === 20 || e === 21 || e === 23 ? 1 : 2, o = this.channels[a2];
    return e === 20 || e === 21 || e === 28 || e === 29 ? t === 32 ? o.ccRCL() : t === 33 ? o.ccBS() : t === 34 ? o.ccAOF() : t === 35 ? o.ccAON() : t === 36 ? o.ccDER() : t === 37 ? o.ccRU(2) : t === 38 ? o.ccRU(3) : t === 39 ? o.ccRU(4) : t === 40 ? o.ccFON() : t === 41 ? o.ccRDC() : t === 42 ? o.ccTR() : t === 43 ? o.ccRTD() : t === 44 ? o.ccEDM() : t === 45 ? o.ccCR() : t === 46 ? o.ccENM() : t === 47 && o.ccEOC() : o.ccTO(t - 32), He2(e, t, s2), this.currentChannel = a2, true;
  }
  parseMidrow(e, t) {
    let s2 = 0;
    if ((e === 17 || e === 25) && t >= 32 && t <= 47) {
      if (e === 17 ? s2 = 1 : s2 = 2, s2 !== this.currentChannel)
        return this.logger.log(0, "Mismatch channel in midrow parsing"), false;
      let i = this.channels[s2];
      return i ? (i.ccMIDROW(t), this.logger.log(3, "MIDROW (" + ke3([e, t]) + ")"), true) : false;
    }
    return false;
  }
  parsePAC(e, t) {
    let s2, i = this.cmdHistory, r2 = (e >= 17 && e <= 23 || e >= 25 && e <= 31) && t >= 64 && t <= 127, a2 = (e === 16 || e === 24) && t >= 64 && t <= 95;
    if (!(r2 || a2))
      return false;
    if (br(e, t, i))
      return He2(null, null, i), true;
    let o = e <= 23 ? 1 : 2;
    t >= 64 && t <= 95 ? s2 = o === 1 ? Qo[e] : Zo[e] : s2 = o === 1 ? Jo[e] : el[e];
    let l2 = this.channels[o];
    return l2 ? (l2.setPAC(this.interpretPAC(s2, t)), He2(e, t, i), this.currentChannel = o, true) : false;
  }
  interpretPAC(e, t) {
    let s2, i = { color: null, italics: false, indent: null, underline: false, row: e };
    return t > 95 ? s2 = t - 96 : s2 = t - 64, i.underline = (s2 & 1) === 1, s2 <= 13 ? i.color = ["white", "green", "blue", "cyan", "red", "yellow", "magenta", "white"][Math.floor(s2 / 2)] : s2 <= 15 ? (i.italics = true, i.color = "white") : i.indent = Math.floor((s2 - 16) / 2) * 4, i;
  }
  parseChars(e, t) {
    let s2, i = null, r2 = null;
    if (e >= 25 ? (s2 = 2, r2 = e - 8) : (s2 = 1, r2 = e), r2 >= 17 && r2 <= 19) {
      let a2;
      r2 === 17 ? a2 = t + 80 : r2 === 18 ? a2 = t + 112 : a2 = t + 144, this.logger.log(2, "Special char '" + In(a2) + "' in channel " + s2), i = [a2];
    } else
      e >= 32 && e <= 127 && (i = t === 0 ? [e] : [e, t]);
    if (i) {
      let a2 = ke3(i);
      this.logger.log(3, "Char codes =  " + a2.join(",")), He2(e, t, this.cmdHistory);
    }
    return i;
  }
  parseBackgroundAttributes(e, t) {
    let s2 = (e === 16 || e === 24) && t >= 32 && t <= 47, i = (e === 23 || e === 31) && t >= 45 && t <= 47;
    if (!(s2 || i))
      return false;
    let r2, a2 = {};
    e === 16 || e === 24 ? (r2 = Math.floor((t - 32) / 2), a2.background = tl2[r2], t % 2 === 1 && (a2.background = a2.background + "_semi")) : t === 45 ? a2.background = "transparent" : (a2.foreground = "black", t === 47 && (a2.underline = true));
    let o = e <= 23 ? 1 : 2;
    return this.channels[o].setBkgData(a2), He2(e, t, this.cmdHistory), true;
  }
  reset() {
    for (let e = 0; e < Object.keys(this.channels).length; e++) {
      let t = this.channels[e];
      t && t.reset();
    }
    this.cmdHistory = Dr();
  }
  cueSplitAtTime(e) {
    for (let t = 0; t < this.channels.length; t++) {
      let s2 = this.channels[t];
      s2 && s2.cueSplitAtTime(e);
    }
  }
};
function He2(n12, e, t) {
  t.a = n12, t.b = e;
}
function br(n12, e, t) {
  return t.a === n12 && t.b === e;
}
function Dr() {
  return { a: null, b: null };
}
var We2 = class {
  constructor(e, t) {
    this.timelineController = void 0, this.cueRanges = [], this.trackName = void 0, this.startTime = null, this.endTime = null, this.screen = null, this.timelineController = e, this.trackName = t;
  }
  dispatchCue() {
    this.startTime !== null && (this.timelineController.addCues(this.trackName, this.startTime, this.endTime, this.screen, this.cueRanges), this.startTime = null);
  }
  newCue(e, t, s2) {
    (this.startTime === null || this.startTime > e) && (this.startTime = e), this.endTime = t, this.screen = s2, this.timelineController.createCaptionsTrack(this.trackName);
  }
  reset() {
    this.cueRanges = [], this.startTime = null;
  }
};
var Bi = function() {
  if (ze2 != null && ze2.VTTCue)
    return self.VTTCue;
  let n12 = ["", "lr", "rl"], e = ["start", "middle", "end", "left", "right"];
  function t(o, l2) {
    if (typeof l2 != "string" || !Array.isArray(o))
      return false;
    let c = l2.toLowerCase();
    return ~o.indexOf(c) ? c : false;
  }
  function s2(o) {
    return t(n12, o);
  }
  function i(o) {
    return t(e, o);
  }
  function r2(o, ...l2) {
    let c = 1;
    for (; c < arguments.length; c++) {
      let h2 = arguments[c];
      for (let u2 in h2)
        o[u2] = h2[u2];
    }
    return o;
  }
  function a2(o, l2, c) {
    let h2 = this, u2 = { enumerable: true };
    h2.hasBeenReset = false;
    let d3 = "", f3 = false, g2 = o, p3 = l2, T2 = c, E4 = null, x3 = "", y3 = true, R = "auto", S2 = "start", b = 50, L = "middle", C2 = 50, _ = "middle";
    Object.defineProperty(h2, "id", r2({}, u2, { get: function() {
      return d3;
    }, set: function(I) {
      d3 = "" + I;
    } })), Object.defineProperty(h2, "pauseOnExit", r2({}, u2, { get: function() {
      return f3;
    }, set: function(I) {
      f3 = !!I;
    } })), Object.defineProperty(h2, "startTime", r2({}, u2, { get: function() {
      return g2;
    }, set: function(I) {
      if (typeof I != "number")
        throw new TypeError("Start time must be set to a number.");
      g2 = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "endTime", r2({}, u2, { get: function() {
      return p3;
    }, set: function(I) {
      if (typeof I != "number")
        throw new TypeError("End time must be set to a number.");
      p3 = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "text", r2({}, u2, { get: function() {
      return T2;
    }, set: function(I) {
      T2 = "" + I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "region", r2({}, u2, { get: function() {
      return E4;
    }, set: function(I) {
      E4 = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "vertical", r2({}, u2, { get: function() {
      return x3;
    }, set: function(I) {
      let w = s2(I);
      if (w === false)
        throw new SyntaxError("An invalid or illegal string was specified.");
      x3 = w, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "snapToLines", r2({}, u2, { get: function() {
      return y3;
    }, set: function(I) {
      y3 = !!I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "line", r2({}, u2, { get: function() {
      return R;
    }, set: function(I) {
      if (typeof I != "number" && I !== "auto")
        throw new SyntaxError("An invalid number or illegal string was specified.");
      R = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "lineAlign", r2({}, u2, { get: function() {
      return S2;
    }, set: function(I) {
      let w = i(I);
      if (!w)
        throw new SyntaxError("An invalid or illegal string was specified.");
      S2 = w, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "position", r2({}, u2, { get: function() {
      return b;
    }, set: function(I) {
      if (I < 0 || I > 100)
        throw new Error("Position must be between 0 and 100.");
      b = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "positionAlign", r2({}, u2, { get: function() {
      return L;
    }, set: function(I) {
      let w = i(I);
      if (!w)
        throw new SyntaxError("An invalid or illegal string was specified.");
      L = w, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "size", r2({}, u2, { get: function() {
      return C2;
    }, set: function(I) {
      if (I < 0 || I > 100)
        throw new Error("Size must be between 0 and 100.");
      C2 = I, this.hasBeenReset = true;
    } })), Object.defineProperty(h2, "align", r2({}, u2, { get: function() {
      return _;
    }, set: function(I) {
      let w = i(I);
      if (!w)
        throw new SyntaxError("An invalid or illegal string was specified.");
      _ = w, this.hasBeenReset = true;
    } })), h2.displayState = void 0;
  }
  return a2.prototype.getCueAsHTML = function() {
    return self.WebVTT.convertCueToDOMTree(self, this.text);
  }, a2;
}();
var ui = class {
  decode(e, t) {
    if (!e)
      return "";
    if (typeof e != "string")
      throw new Error("Error - expected string data.");
    return decodeURIComponent(encodeURIComponent(e));
  }
};
function bn(n12) {
  function e(s2, i, r2, a2) {
    return (s2 | 0) * 3600 + (i | 0) * 60 + (r2 | 0) + parseFloat(a2 || 0);
  }
  let t = n12.match(/^(?:(\d+):)?(\d{2}):(\d{2})(\.\d+)?/);
  return t ? parseFloat(t[2]) > 59 ? e(t[2], t[3], 0, t[4]) : e(t[1], t[2], t[3], t[4]) : null;
}
var di = class {
  constructor() {
    this.values = /* @__PURE__ */ Object.create(null);
  }
  set(e, t) {
    !this.get(e) && t !== "" && (this.values[e] = t);
  }
  get(e, t, s2) {
    return s2 ? this.has(e) ? this.values[e] : t[s2] : this.has(e) ? this.values[e] : t;
  }
  has(e) {
    return e in this.values;
  }
  alt(e, t, s2) {
    for (let i = 0; i < s2.length; ++i)
      if (t === s2[i]) {
        this.set(e, t);
        break;
      }
  }
  integer(e, t) {
    /^-?\d+$/.test(t) && this.set(e, parseInt(t, 10));
  }
  percent(e, t) {
    if (/^([\d]{1,3})(\.[\d]*)?%$/.test(t)) {
      let s2 = parseFloat(t);
      if (s2 >= 0 && s2 <= 100)
        return this.set(e, s2), true;
    }
    return false;
  }
};
function Dn(n12, e, t, s2) {
  let i = s2 ? n12.split(s2) : [n12];
  for (let r2 in i) {
    if (typeof i[r2] != "string")
      continue;
    let a2 = i[r2].split(t);
    if (a2.length !== 2)
      continue;
    let o = a2[0], l2 = a2[1];
    e(o, l2);
  }
}
var fi = new Bi(0, 0, "");
var Tt = fi.align === "middle" ? "middle" : "center";
function sl(n12, e, t) {
  let s2 = n12;
  function i() {
    let o = bn(n12);
    if (o === null)
      throw new Error("Malformed timestamp: " + s2);
    return n12 = n12.replace(/^[^\sa-zA-Z-]+/, ""), o;
  }
  function r2(o, l2) {
    let c = new di();
    Dn(o, function(d3, f3) {
      let g2;
      switch (d3) {
        case "region":
          for (let p3 = t.length - 1; p3 >= 0; p3--)
            if (t[p3].id === f3) {
              c.set(d3, t[p3].region);
              break;
            }
          break;
        case "vertical":
          c.alt(d3, f3, ["rl", "lr"]);
          break;
        case "line":
          g2 = f3.split(","), c.integer(d3, g2[0]), c.percent(d3, g2[0]) && c.set("snapToLines", false), c.alt(d3, g2[0], ["auto"]), g2.length === 2 && c.alt("lineAlign", g2[1], ["start", Tt, "end"]);
          break;
        case "position":
          g2 = f3.split(","), c.percent(d3, g2[0]), g2.length === 2 && c.alt("positionAlign", g2[1], ["start", Tt, "end", "line-left", "line-right", "auto"]);
          break;
        case "size":
          c.percent(d3, f3);
          break;
        case "align":
          c.alt(d3, f3, ["start", Tt, "end", "left", "right"]);
          break;
      }
    }, /:/, /\s/), l2.region = c.get("region", null), l2.vertical = c.get("vertical", "");
    let h2 = c.get("line", "auto");
    h2 === "auto" && fi.line === -1 && (h2 = -1), l2.line = h2, l2.lineAlign = c.get("lineAlign", "start"), l2.snapToLines = c.get("snapToLines", true), l2.size = c.get("size", 100), l2.align = c.get("align", Tt);
    let u2 = c.get("position", "auto");
    u2 === "auto" && fi.position === 50 && (u2 = l2.align === "start" || l2.align === "left" ? 0 : l2.align === "end" || l2.align === "right" ? 100 : 50), l2.position = u2;
  }
  function a2() {
    n12 = n12.replace(/^\s+/, "");
  }
  if (a2(), e.startTime = i(), a2(), n12.slice(0, 3) !== "-->")
    throw new Error("Malformed time stamp (time stamps must be separated by '-->'): " + s2);
  n12 = n12.slice(3), a2(), e.endTime = i(), a2(), r2(n12, e);
}
function Cn(n12) {
  return n12.replace(/<br(?: \/)?>/gi, `
`);
}
var gi = class {
  constructor() {
    this.state = "INITIAL", this.buffer = "", this.decoder = new ui(), this.regionList = [], this.cue = null, this.oncue = void 0, this.onparsingerror = void 0, this.onflush = void 0;
  }
  parse(e) {
    let t = this;
    e && (t.buffer += t.decoder.decode(e, { stream: true }));
    function s2() {
      let r2 = t.buffer, a2 = 0;
      for (r2 = Cn(r2); a2 < r2.length && r2[a2] !== "\r" && r2[a2] !== `
`; )
        ++a2;
      let o = r2.slice(0, a2);
      return r2[a2] === "\r" && ++a2, r2[a2] === `
` && ++a2, t.buffer = r2.slice(a2), o;
    }
    function i(r2) {
      Dn(r2, function(a2, o) {
      }, /:/);
    }
    try {
      let r2 = "";
      if (t.state === "INITIAL") {
        if (!/\r\n|\n/.test(t.buffer))
          return this;
        r2 = s2();
        let o = r2.match(/^(ï»¿)?WEBVTT([ \t].*)?$/);
        if (!(o != null && o[0]))
          throw new Error("Malformed WebVTT signature.");
        t.state = "HEADER";
      }
      let a2 = false;
      for (; t.buffer; ) {
        if (!/\r\n|\n/.test(t.buffer))
          return this;
        switch (a2 ? a2 = false : r2 = s2(), t.state) {
          case "HEADER":
            /:/.test(r2) ? i(r2) : r2 || (t.state = "ID");
            continue;
          case "NOTE":
            r2 || (t.state = "ID");
            continue;
          case "ID":
            if (/^NOTE($|[ \t])/.test(r2)) {
              t.state = "NOTE";
              break;
            }
            if (!r2)
              continue;
            if (t.cue = new Bi(0, 0, ""), t.state = "CUE", r2.indexOf("-->") === -1) {
              t.cue.id = r2;
              continue;
            }
          case "CUE":
            if (!t.cue) {
              t.state = "BADCUE";
              continue;
            }
            try {
              sl(r2, t.cue, t.regionList);
            } catch {
              t.cue = null, t.state = "BADCUE";
              continue;
            }
            t.state = "CUETEXT";
            continue;
          case "CUETEXT":
            {
              let o = r2.indexOf("-->") !== -1;
              if (!r2 || o && (a2 = true)) {
                t.oncue && t.cue && t.oncue(t.cue), t.cue = null, t.state = "ID";
                continue;
              }
              if (t.cue === null)
                continue;
              t.cue.text && (t.cue.text += `
`), t.cue.text += r2;
            }
            continue;
          case "BADCUE":
            r2 || (t.state = "ID");
        }
      }
    } catch {
      t.state === "CUETEXT" && t.cue && t.oncue && t.oncue(t.cue), t.cue = null, t.state = t.state === "INITIAL" ? "BADWEBVTT" : "BADCUE";
    }
    return this;
  }
  flush() {
    let e = this;
    try {
      if ((e.cue || e.state === "HEADER") && (e.buffer += `

`, e.parse()), e.state === "INITIAL" || e.state === "BADWEBVTT")
        throw new Error("Malformed WebVTT signature.");
    } catch (t) {
      e.onparsingerror && e.onparsingerror(t);
    }
    return e.onflush && e.onflush(), this;
  }
};
var il = /\r\n|\n\r|\n|\r/g;
var fs = function(e, t, s2 = 0) {
  return e.slice(s2, s2 + t.length) === t;
};
var rl = function(e) {
  let t = parseInt(e.slice(-3)), s2 = parseInt(e.slice(-6, -4)), i = parseInt(e.slice(-9, -7)), r2 = e.length > 9 ? parseInt(e.substring(0, e.indexOf(":"))) : 0;
  if (!F(t) || !F(s2) || !F(i) || !F(r2))
    throw Error(`Malformed X-TIMESTAMP-MAP: Local:${e}`);
  return t += 1e3 * s2, t += 60 * 1e3 * i, t += 60 * 60 * 1e3 * r2, t;
};
var gs = function(e) {
  let t = 5381, s2 = e.length;
  for (; s2; )
    t = t * 33 ^ e.charCodeAt(--s2);
  return (t >>> 0).toString();
};
function $i(n12, e, t) {
  return gs(n12.toString()) + gs(e.toString()) + gs(t);
}
var nl2 = function(e, t, s2) {
  let i = e[t], r2 = e[i.prevCC];
  if (!r2 || !r2.new && i.new) {
    e.ccOffset = e.presentationOffset = i.start, i.new = false;
    return;
  }
  for (; (a2 = r2) != null && a2.new; ) {
    var a2;
    e.ccOffset += i.start - r2.start, i.new = false, i = r2, r2 = e[i.prevCC];
  }
  e.presentationOffset = s2;
};
function al(n12, e, t, s2, i, r2, a2) {
  let o = new gi(), l2 = Re2(new Uint8Array(n12)).trim().replace(il, `
`).split(`
`), c = [], h2 = e ? Ko(e.baseTime, e.timescale) : 0, u2 = "00:00.000", d3 = 0, f3 = 0, g2, p3 = true;
  o.oncue = function(T2) {
    let E4 = t[s2], x3 = t.ccOffset, y3 = (d3 - h2) / 9e4;
    if (E4 != null && E4.new && (f3 !== void 0 ? x3 = t.ccOffset = E4.start : nl2(t, s2, y3)), y3) {
      if (!e) {
        g2 = new Error("Missing initPTS for VTT MPEGTS");
        return;
      }
      x3 = y3 - t.presentationOffset;
    }
    let R = T2.endTime - T2.startTime, S2 = me3((T2.startTime + x3 - f3) * 9e4, i * 9e4) / 9e4;
    T2.startTime = Math.max(S2, 0), T2.endTime = Math.max(S2 + R, 0);
    let b = T2.text.trim();
    T2.text = decodeURIComponent(encodeURIComponent(b)), T2.id || (T2.id = $i(T2.startTime, T2.endTime, b)), T2.endTime > 0 && c.push(T2);
  }, o.onparsingerror = function(T2) {
    g2 = T2;
  }, o.onflush = function() {
    if (g2) {
      a2(g2);
      return;
    }
    r2(c);
  }, l2.forEach((T2) => {
    if (p3)
      if (fs(T2, "X-TIMESTAMP-MAP=")) {
        p3 = false, T2.slice(16).split(",").forEach((E4) => {
          fs(E4, "LOCAL:") ? u2 = E4.slice(6) : fs(E4, "MPEGTS:") && (d3 = parseInt(E4.slice(7)));
        });
        try {
          f3 = rl(u2) / 1e3;
        } catch (E4) {
          g2 = E4;
        }
        return;
      } else
        T2 === "" && (p3 = false);
    o.parse(T2 + `
`);
  }), o.flush();
}
var ms = "stpp.ttml.im1t";
var wn = /^(\d{2,}):(\d{2}):(\d{2}):(\d{2})\.?(\d+)?$/;
var _n = /^(\d*(?:\.\d*)?)(h|m|s|ms|f|t)$/;
var ol = { left: "start", center: "center", right: "end", start: "start", end: "end" };
function Cr(n12, e, t, s2) {
  let i = H2(new Uint8Array(n12), ["mdat"]);
  if (i.length === 0) {
    s2(new Error("Could not parse IMSC1 mdat"));
    return;
  }
  let r2 = i.map((o) => Re2(o)), a2 = Go(e.baseTime, 1, e.timescale);
  try {
    r2.forEach((o) => t(ll(o, a2)));
  } catch (o) {
    s2(o);
  }
}
function ll(n12, e) {
  let i = new DOMParser().parseFromString(n12, "text/xml").getElementsByTagName("tt")[0];
  if (!i)
    throw new Error("Invalid ttml");
  let r2 = { frameRate: 30, subFrameRate: 1, frameRateMultiplier: 0, tickRate: 0 }, a2 = Object.keys(r2).reduce((u2, d3) => (u2[d3] = i.getAttribute(`ttp:${d3}`) || r2[d3], u2), {}), o = i.getAttribute("xml:space") !== "preserve", l2 = wr(ps(i, "styling", "style")), c = wr(ps(i, "layout", "region")), h2 = ps(i, "body", "[begin]");
  return [].map.call(h2, (u2) => {
    let d3 = Pn(u2, o);
    if (!d3 || !u2.hasAttribute("begin"))
      return null;
    let f3 = Es(u2.getAttribute("begin"), a2), g2 = Es(u2.getAttribute("dur"), a2), p3 = Es(u2.getAttribute("end"), a2);
    if (f3 === null)
      throw _r(u2);
    if (p3 === null) {
      if (g2 === null)
        throw _r(u2);
      p3 = f3 + g2;
    }
    let T2 = new Bi(f3 - e, p3 - e, d3);
    T2.id = $i(T2.startTime, T2.endTime, T2.text);
    let E4 = c[u2.getAttribute("region")], x3 = l2[u2.getAttribute("style")], y3 = cl(E4, x3, l2), { textAlign: R } = y3;
    if (R) {
      let S2 = ol[R];
      S2 && (T2.lineAlign = S2), T2.align = R;
    }
    return te2(T2, y3), T2;
  }).filter((u2) => u2 !== null);
}
function ps(n12, e, t) {
  let s2 = n12.getElementsByTagName(e)[0];
  return s2 ? [].slice.call(s2.querySelectorAll(t)) : [];
}
function wr(n12) {
  return n12.reduce((e, t) => {
    let s2 = t.getAttribute("xml:id");
    return s2 && (e[s2] = t), e;
  }, {});
}
function Pn(n12, e) {
  return [].slice.call(n12.childNodes).reduce((t, s2, i) => {
    var r2;
    return s2.nodeName === "br" && i ? t + `
` : (r2 = s2.childNodes) != null && r2.length ? Pn(s2, e) : e ? t + s2.textContent.trim().replace(/\s+/g, " ") : t + s2.textContent;
  }, "");
}
function cl(n12, e, t) {
  let s2 = "http://www.w3.org/ns/ttml#styling", i = null, r2 = ["displayAlign", "textAlign", "color", "backgroundColor", "fontSize", "fontFamily"], a2 = n12 != null && n12.hasAttribute("style") ? n12.getAttribute("style") : null;
  return a2 && t.hasOwnProperty(a2) && (i = t[a2]), r2.reduce((o, l2) => {
    let c = Ts(e, s2, l2) || Ts(n12, s2, l2) || Ts(i, s2, l2);
    return c && (o[l2] = c), o;
  }, {});
}
function Ts(n12, e, t) {
  return n12 && n12.hasAttributeNS(e, t) ? n12.getAttributeNS(e, t) : null;
}
function _r(n12) {
  return new Error(`Could not parse ttml timestamp ${n12}`);
}
function Es(n12, e) {
  if (!n12)
    return null;
  let t = bn(n12);
  return t === null && (wn.test(n12) ? t = hl(n12, e) : _n.test(n12) && (t = ul(n12, e))), t;
}
function hl(n12, e) {
  let t = wn.exec(n12), s2 = (t[4] | 0) + (t[5] | 0) / e.subFrameRate;
  return (t[1] | 0) * 3600 + (t[2] | 0) * 60 + (t[3] | 0) + s2 / e.frameRate;
}
function ul(n12, e) {
  let t = _n.exec(n12), s2 = Number(t[1]);
  switch (t[2]) {
    case "h":
      return s2 * 3600;
    case "m":
      return s2 * 60;
    case "ms":
      return s2 * 1e3;
    case "f":
      return s2 / e.frameRate;
    case "t":
      return s2 / e.tickRate;
  }
  return s2;
}
var mi = class {
  constructor(e) {
    this.hls = void 0, this.media = null, this.config = void 0, this.enabled = true, this.Cues = void 0, this.textTracks = [], this.tracks = [], this.initPTS = [], this.unparsedVttFrags = [], this.captionsTracks = {}, this.nonNativeCaptionsTracks = {}, this.cea608Parser1 = void 0, this.cea608Parser2 = void 0, this.lastCc = -1, this.lastSn = -1, this.lastPartIndex = -1, this.prevCC = -1, this.vttCCs = kr(), this.captionsProperties = void 0, this.hls = e, this.config = e.config, this.Cues = e.config.cueHandler, this.captionsProperties = { textTrack1: { label: this.config.captionsTextTrack1Label, languageCode: this.config.captionsTextTrack1LanguageCode }, textTrack2: { label: this.config.captionsTextTrack2Label, languageCode: this.config.captionsTextTrack2LanguageCode }, textTrack3: { label: this.config.captionsTextTrack3Label, languageCode: this.config.captionsTextTrack3LanguageCode }, textTrack4: { label: this.config.captionsTextTrack4Label, languageCode: this.config.captionsTextTrack4LanguageCode } }, e.on(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.on(m2.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this), e.on(m2.FRAG_LOADING, this.onFragLoading, this), e.on(m2.FRAG_LOADED, this.onFragLoaded, this), e.on(m2.FRAG_PARSING_USERDATA, this.onFragParsingUserdata, this), e.on(m2.FRAG_DECRYPTED, this.onFragDecrypted, this), e.on(m2.INIT_PTS_FOUND, this.onInitPtsFound, this), e.on(m2.SUBTITLE_TRACKS_CLEARED, this.onSubtitleTracksCleared, this), e.on(m2.BUFFER_FLUSHING, this.onBufferFlushing, this);
  }
  destroy() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.off(m2.SUBTITLE_TRACKS_UPDATED, this.onSubtitleTracksUpdated, this), e.off(m2.FRAG_LOADING, this.onFragLoading, this), e.off(m2.FRAG_LOADED, this.onFragLoaded, this), e.off(m2.FRAG_PARSING_USERDATA, this.onFragParsingUserdata, this), e.off(m2.FRAG_DECRYPTED, this.onFragDecrypted, this), e.off(m2.INIT_PTS_FOUND, this.onInitPtsFound, this), e.off(m2.SUBTITLE_TRACKS_CLEARED, this.onSubtitleTracksCleared, this), e.off(m2.BUFFER_FLUSHING, this.onBufferFlushing, this), this.hls = this.config = null, this.cea608Parser1 = this.cea608Parser2 = void 0;
  }
  initCea608Parsers() {
    if (this.config.enableCEA708Captions && (!this.cea608Parser1 || !this.cea608Parser2)) {
      let e = new We2(this, "textTrack1"), t = new We2(this, "textTrack2"), s2 = new We2(this, "textTrack3"), i = new We2(this, "textTrack4");
      this.cea608Parser1 = new jt(1, e, t), this.cea608Parser2 = new jt(3, s2, i);
    }
  }
  addCues(e, t, s2, i, r2) {
    let a2 = false;
    for (let o = r2.length; o--; ) {
      let l2 = r2[o], c = dl(l2[0], l2[1], t, s2);
      if (c >= 0 && (l2[0] = Math.min(l2[0], t), l2[1] = Math.max(l2[1], s2), a2 = true, c / (s2 - t) > 0.5))
        return;
    }
    if (a2 || r2.push([t, s2]), this.config.renderTextTracksNatively) {
      let o = this.captionsTracks[e];
      this.Cues.newCue(o, t, s2, i);
    } else {
      let o = this.Cues.newCue(null, t, s2, i);
      this.hls.trigger(m2.CUES_PARSED, { type: "captions", cues: o, track: e });
    }
  }
  onInitPtsFound(e, { frag: t, id: s2, initPTS: i, timescale: r2 }) {
    let { unparsedVttFrags: a2 } = this;
    s2 === "main" && (this.initPTS[t.cc] = { baseTime: i, timescale: r2 }), a2.length && (this.unparsedVttFrags = [], a2.forEach((o) => {
      this.onFragLoaded(m2.FRAG_LOADED, o);
    }));
  }
  getExistingTrack(e, t) {
    let { media: s2 } = this;
    if (s2)
      for (let i = 0; i < s2.textTracks.length; i++) {
        let r2 = s2.textTracks[i];
        if (Pr(r2, { name: e, lang: t, attrs: {} }))
          return r2;
      }
    return null;
  }
  createCaptionsTrack(e) {
    this.config.renderTextTracksNatively ? this.createNativeTrack(e) : this.createNonNativeTrack(e);
  }
  createNativeTrack(e) {
    if (this.captionsTracks[e])
      return;
    let { captionsProperties: t, captionsTracks: s2, media: i } = this, { label: r2, languageCode: a2 } = t[e], o = this.getExistingTrack(r2, a2);
    if (o)
      s2[e] = o, Ve2(s2[e]), en(s2[e], i);
    else {
      let l2 = this.createTextTrack("captions", r2, a2);
      l2 && (l2[e] = true, s2[e] = l2);
    }
  }
  createNonNativeTrack(e) {
    if (this.nonNativeCaptionsTracks[e])
      return;
    let t = this.captionsProperties[e];
    if (!t)
      return;
    let s2 = t.label, i = { _id: e, label: s2, kind: "captions", default: t.media ? !!t.media.default : false, closedCaptions: t.media };
    this.nonNativeCaptionsTracks[e] = i, this.hls.trigger(m2.NON_NATIVE_TEXT_TRACKS_FOUND, { tracks: [i] });
  }
  createTextTrack(e, t, s2) {
    let i = this.media;
    if (i)
      return i.addTextTrack(e, t, s2);
  }
  onMediaAttaching(e, t) {
    this.media = t.media, this._cleanTracks();
  }
  onMediaDetaching() {
    let { captionsTracks: e } = this;
    Object.keys(e).forEach((t) => {
      Ve2(e[t]), delete e[t];
    }), this.nonNativeCaptionsTracks = {};
  }
  onManifestLoading() {
    this.lastCc = -1, this.lastSn = -1, this.lastPartIndex = -1, this.prevCC = -1, this.vttCCs = kr(), this._cleanTracks(), this.tracks = [], this.captionsTracks = {}, this.nonNativeCaptionsTracks = {}, this.textTracks = [], this.unparsedVttFrags = [], this.initPTS = [], this.cea608Parser1 && this.cea608Parser2 && (this.cea608Parser1.reset(), this.cea608Parser2.reset());
  }
  _cleanTracks() {
    let { media: e } = this;
    if (!e)
      return;
    let t = e.textTracks;
    if (t)
      for (let s2 = 0; s2 < t.length; s2++)
        Ve2(t[s2]);
  }
  onSubtitleTracksUpdated(e, t) {
    let s2 = t.subtitleTracks || [], i = s2.some((r2) => r2.textCodec === ms);
    if (this.config.enableWebVTT || i && this.config.enableIMSC1) {
      if (Rn(this.tracks, s2)) {
        this.tracks = s2;
        return;
      }
      if (this.textTracks = [], this.tracks = s2, this.config.renderTextTracksNatively) {
        let a2 = this.media, o = a2 ? xt(a2.textTracks) : null;
        if (this.tracks.forEach((l2, c) => {
          let h2;
          if (o) {
            let u2 = null;
            for (let d3 = 0; d3 < o.length; d3++)
              if (o[d3] && Pr(o[d3], l2)) {
                u2 = o[d3], o[d3] = null;
                break;
              }
            u2 && (h2 = u2);
          }
          if (h2)
            Ve2(h2);
          else {
            let u2 = kn(l2);
            h2 = this.createTextTrack(u2, l2.name, l2.lang), h2 && (h2.mode = "disabled");
          }
          h2 && this.textTracks.push(h2);
        }), o != null && o.length) {
          let l2 = o.filter((c) => c !== null).map((c) => c.label);
          l2.length && v2.warn(`Media element contains unused subtitle tracks: ${l2.join(", ")}. Replace media element for each source to clear TextTracks and captions menu.`);
        }
      } else if (this.tracks.length) {
        let a2 = this.tracks.map((o) => ({ label: o.name, kind: o.type.toLowerCase(), default: o.default, subtitleTrack: o }));
        this.hls.trigger(m2.NON_NATIVE_TEXT_TRACKS_FOUND, { tracks: a2 });
      }
    }
  }
  onManifestLoaded(e, t) {
    this.config.enableCEA708Captions && t.captions && t.captions.forEach((s2) => {
      let i = /(?:CC|SERVICE)([1-4])/.exec(s2.instreamId);
      if (!i)
        return;
      let r2 = `textTrack${i[1]}`, a2 = this.captionsProperties[r2];
      a2 && (a2.label = s2.name, s2.lang && (a2.languageCode = s2.lang), a2.media = s2);
    });
  }
  closedCaptionsForLevel(e) {
    let t = this.hls.levels[e.level];
    return t?.attrs["CLOSED-CAPTIONS"];
  }
  onFragLoading(e, t) {
    this.initCea608Parsers();
    let { cea608Parser1: s2, cea608Parser2: i, lastCc: r2, lastSn: a2, lastPartIndex: o } = this;
    if (!(!this.enabled || !s2 || !i) && t.frag.type === N2.MAIN) {
      var l2, c;
      let { cc: h2, sn: u2 } = t.frag, d3 = (l2 = t == null || (c = t.part) == null ? void 0 : c.index) != null ? l2 : -1;
      u2 === a2 + 1 || u2 === a2 && d3 === o + 1 || h2 === r2 || (s2.reset(), i.reset()), this.lastCc = h2, this.lastSn = u2, this.lastPartIndex = d3;
    }
  }
  onFragLoaded(e, t) {
    let { frag: s2, payload: i } = t;
    if (s2.type === N2.SUBTITLE)
      if (i.byteLength) {
        let r2 = s2.decryptdata, a2 = "stats" in t;
        if (r2 == null || !r2.encrypted || a2) {
          let o = this.tracks[s2.level], l2 = this.vttCCs;
          l2[s2.cc] || (l2[s2.cc] = { start: s2.start, prevCC: this.prevCC, new: true }, this.prevCC = s2.cc), o && o.textCodec === ms ? this._parseIMSC1(s2, i) : this._parseVTTs(t);
        }
      } else
        this.hls.trigger(m2.SUBTITLE_FRAG_PROCESSED, { success: false, frag: s2, error: new Error("Empty subtitle payload") });
  }
  _parseIMSC1(e, t) {
    let s2 = this.hls;
    Cr(t, this.initPTS[e.cc], (i) => {
      this._appendCues(i, e.level), s2.trigger(m2.SUBTITLE_FRAG_PROCESSED, { success: true, frag: e });
    }, (i) => {
      v2.log(`Failed to parse IMSC1: ${i}`), s2.trigger(m2.SUBTITLE_FRAG_PROCESSED, { success: false, frag: e, error: i });
    });
  }
  _parseVTTs(e) {
    var t;
    let { frag: s2, payload: i } = e, { initPTS: r2, unparsedVttFrags: a2 } = this, o = r2.length - 1;
    if (!r2[s2.cc] && o === -1) {
      a2.push(e);
      return;
    }
    let l2 = this.hls, c = (t = s2.initSegment) != null && t.data ? pe3(s2.initSegment.data, new Uint8Array(i)) : i;
    al(c, this.initPTS[s2.cc], this.vttCCs, s2.cc, s2.start, (h2) => {
      this._appendCues(h2, s2.level), l2.trigger(m2.SUBTITLE_FRAG_PROCESSED, { success: true, frag: s2 });
    }, (h2) => {
      let u2 = h2.message === "Missing initPTS for VTT MPEGTS";
      u2 ? a2.push(e) : this._fallbackToIMSC1(s2, i), v2.log(`Failed to parse VTT cue: ${h2}`), !(u2 && o > s2.cc) && l2.trigger(m2.SUBTITLE_FRAG_PROCESSED, { success: false, frag: s2, error: h2 });
    });
  }
  _fallbackToIMSC1(e, t) {
    let s2 = this.tracks[e.level];
    s2.textCodec || Cr(t, this.initPTS[e.cc], () => {
      s2.textCodec = ms, this._parseIMSC1(e, t);
    }, () => {
      s2.textCodec = "wvtt";
    });
  }
  _appendCues(e, t) {
    let s2 = this.hls;
    if (this.config.renderTextTracksNatively) {
      let i = this.textTracks[t];
      if (!i || i.mode === "disabled")
        return;
      e.forEach((r2) => tn(i, r2));
    } else {
      let i = this.tracks[t];
      if (!i)
        return;
      let r2 = i.default ? "default" : "subtitles" + t;
      s2.trigger(m2.CUES_PARSED, { type: "subtitles", cues: e, track: r2 });
    }
  }
  onFragDecrypted(e, t) {
    let { frag: s2 } = t;
    s2.type === N2.SUBTITLE && this.onFragLoaded(m2.FRAG_LOADED, t);
  }
  onSubtitleTracksCleared() {
    this.tracks = [], this.captionsTracks = {};
  }
  onFragParsingUserdata(e, t) {
    this.initCea608Parsers();
    let { cea608Parser1: s2, cea608Parser2: i } = this;
    if (!this.enabled || !s2 || !i)
      return;
    let { frag: r2, samples: a2 } = t;
    if (!(r2.type === N2.MAIN && this.closedCaptionsForLevel(r2) === "NONE"))
      for (let o = 0; o < a2.length; o++) {
        let l2 = a2[o].bytes;
        if (l2) {
          let c = this.extractCea608Data(l2);
          s2.addData(a2[o].pts, c[0]), i.addData(a2[o].pts, c[1]);
        }
      }
  }
  onBufferFlushing(e, { startOffset: t, endOffset: s2, endOffsetSubtitles: i, type: r2 }) {
    let { media: a2 } = this;
    if (!(!a2 || a2.currentTime < s2)) {
      if (!r2 || r2 === "video") {
        let { captionsTracks: o } = this;
        Object.keys(o).forEach((l2) => Is(o[l2], t, s2));
      }
      if (this.config.renderTextTracksNatively && t === 0 && i !== void 0) {
        let { textTracks: o } = this;
        Object.keys(o).forEach((l2) => Is(o[l2], t, i));
      }
    }
  }
  extractCea608Data(e) {
    let t = [[], []], s2 = e[0] & 31, i = 2;
    for (let r2 = 0; r2 < s2; r2++) {
      let a2 = e[i++], o = 127 & e[i++], l2 = 127 & e[i++];
      if (o === 0 && l2 === 0)
        continue;
      if ((4 & a2) !== 0) {
        let h2 = 3 & a2;
        (h2 === 0 || h2 === 1) && (t[h2].push(o), t[h2].push(l2));
      }
    }
    return t;
  }
};
function kn(n12) {
  return n12.characteristics && /transcribes-spoken-dialog/gi.test(n12.characteristics) && /describes-music-and-sound/gi.test(n12.characteristics) ? "captions" : "subtitles";
}
function Pr(n12, e) {
  return !!n12 && n12.kind === kn(e) && ei(e, n12);
}
function dl(n12, e, t, s2) {
  return Math.min(e, s2) - Math.max(n12, t);
}
function kr() {
  return { ccOffset: 0, presentationOffset: 0, 0: { start: 0, prevCC: -1, new: true } };
}
var pi = class n8 {
  constructor(e) {
    this.hls = void 0, this.autoLevelCapping = void 0, this.firstLevel = void 0, this.media = void 0, this.restrictedLevels = void 0, this.timer = void 0, this.clientRect = void 0, this.streamController = void 0, this.hls = e, this.autoLevelCapping = Number.POSITIVE_INFINITY, this.firstLevel = -1, this.media = null, this.restrictedLevels = [], this.timer = void 0, this.clientRect = null, this.registerListeners();
  }
  setStreamController(e) {
    this.streamController = e;
  }
  destroy() {
    this.hls && this.unregisterListener(), this.timer && this.stopCapping(), this.media = null, this.clientRect = null, this.hls = this.streamController = null;
  }
  registerListeners() {
    let { hls: e } = this;
    e.on(m2.FPS_DROP_LEVEL_CAPPING, this.onFpsDropLevelCapping, this), e.on(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.on(m2.BUFFER_CODECS, this.onBufferCodecs, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this);
  }
  unregisterListener() {
    let { hls: e } = this;
    e.off(m2.FPS_DROP_LEVEL_CAPPING, this.onFpsDropLevelCapping, this), e.off(m2.MEDIA_ATTACHING, this.onMediaAttaching, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.off(m2.BUFFER_CODECS, this.onBufferCodecs, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this);
  }
  onFpsDropLevelCapping(e, t) {
    let s2 = this.hls.levels[t.droppedLevel];
    this.isLevelAllowed(s2) && this.restrictedLevels.push({ bitrate: s2.bitrate, height: s2.height, width: s2.width });
  }
  onMediaAttaching(e, t) {
    this.media = t.media instanceof HTMLVideoElement ? t.media : null, this.clientRect = null, this.timer && this.hls.levels.length && this.detectPlayerSize();
  }
  onManifestParsed(e, t) {
    let s2 = this.hls;
    this.restrictedLevels = [], this.firstLevel = t.firstLevel, s2.config.capLevelToPlayerSize && t.video && this.startCapping();
  }
  onLevelsUpdated(e, t) {
    this.timer && F(this.autoLevelCapping) && this.detectPlayerSize();
  }
  onBufferCodecs(e, t) {
    this.hls.config.capLevelToPlayerSize && t.video && this.startCapping();
  }
  onMediaDetaching() {
    this.stopCapping();
  }
  detectPlayerSize() {
    if (this.media) {
      if (this.mediaHeight <= 0 || this.mediaWidth <= 0) {
        this.clientRect = null;
        return;
      }
      let e = this.hls.levels;
      if (e.length) {
        let t = this.hls, s2 = this.getMaxLevel(e.length - 1);
        s2 !== this.autoLevelCapping && v2.log(`Setting autoLevelCapping to ${s2}: ${e[s2].height}p@${e[s2].bitrate} for media ${this.mediaWidth}x${this.mediaHeight}`), t.autoLevelCapping = s2, t.autoLevelCapping > this.autoLevelCapping && this.streamController && this.streamController.nextLevelSwitch(), this.autoLevelCapping = t.autoLevelCapping;
      }
    }
  }
  getMaxLevel(e) {
    let t = this.hls.levels;
    if (!t.length)
      return -1;
    let s2 = t.filter((i, r2) => this.isLevelAllowed(i) && r2 <= e);
    return this.clientRect = null, n8.getMaxLevelByMediaSize(s2, this.mediaWidth, this.mediaHeight);
  }
  startCapping() {
    this.timer || (this.autoLevelCapping = Number.POSITIVE_INFINITY, self.clearInterval(this.timer), this.timer = self.setInterval(this.detectPlayerSize.bind(this), 1e3), this.detectPlayerSize());
  }
  stopCapping() {
    this.restrictedLevels = [], this.firstLevel = -1, this.autoLevelCapping = Number.POSITIVE_INFINITY, this.timer && (self.clearInterval(this.timer), this.timer = void 0);
  }
  getDimensions() {
    if (this.clientRect)
      return this.clientRect;
    let e = this.media, t = { width: 0, height: 0 };
    if (e) {
      let s2 = e.getBoundingClientRect();
      t.width = s2.width, t.height = s2.height, !t.width && !t.height && (t.width = s2.right - s2.left || e.width || 0, t.height = s2.bottom - s2.top || e.height || 0);
    }
    return this.clientRect = t, t;
  }
  get mediaWidth() {
    return this.getDimensions().width * this.contentScaleFactor;
  }
  get mediaHeight() {
    return this.getDimensions().height * this.contentScaleFactor;
  }
  get contentScaleFactor() {
    let e = 1;
    if (!this.hls.config.ignoreDevicePixelRatio)
      try {
        e = self.devicePixelRatio;
      } catch {
      }
    return e;
  }
  isLevelAllowed(e) {
    return !this.restrictedLevels.some((s2) => e.bitrate === s2.bitrate && e.width === s2.width && e.height === s2.height);
  }
  static getMaxLevelByMediaSize(e, t, s2) {
    if (!(e != null && e.length))
      return -1;
    let i = (o, l2) => l2 ? o.width !== l2.width || o.height !== l2.height : true, r2 = e.length - 1, a2 = Math.max(t, s2);
    for (let o = 0; o < e.length; o += 1) {
      let l2 = e[o];
      if ((l2.width >= a2 || l2.height >= a2) && i(l2, e[o + 1])) {
        r2 = o;
        break;
      }
    }
    return r2;
  }
};
var Ti = class {
  constructor(e) {
    this.hls = void 0, this.isVideoPlaybackQualityAvailable = false, this.timer = void 0, this.media = null, this.lastTime = void 0, this.lastDroppedFrames = 0, this.lastDecodedFrames = 0, this.streamController = void 0, this.hls = e, this.registerListeners();
  }
  setStreamController(e) {
    this.streamController = e;
  }
  registerListeners() {
    this.hls.on(m2.MEDIA_ATTACHING, this.onMediaAttaching, this);
  }
  unregisterListeners() {
    this.hls.off(m2.MEDIA_ATTACHING, this.onMediaAttaching, this);
  }
  destroy() {
    this.timer && clearInterval(this.timer), this.unregisterListeners(), this.isVideoPlaybackQualityAvailable = false, this.media = null;
  }
  onMediaAttaching(e, t) {
    let s2 = this.hls.config;
    if (s2.capLevelOnFPSDrop) {
      let i = t.media instanceof self.HTMLVideoElement ? t.media : null;
      this.media = i, i && typeof i.getVideoPlaybackQuality == "function" && (this.isVideoPlaybackQualityAvailable = true), self.clearInterval(this.timer), this.timer = self.setInterval(this.checkFPSInterval.bind(this), s2.fpsDroppedMonitoringPeriod);
    }
  }
  checkFPS(e, t, s2) {
    let i = performance.now();
    if (t) {
      if (this.lastTime) {
        let r2 = i - this.lastTime, a2 = s2 - this.lastDroppedFrames, o = t - this.lastDecodedFrames, l2 = 1e3 * a2 / r2, c = this.hls;
        if (c.trigger(m2.FPS_DROP, { currentDropped: a2, currentDecoded: o, totalDroppedFrames: s2 }), l2 > 0 && a2 > c.config.fpsDroppedMonitoringThreshold * o) {
          let h2 = c.currentLevel;
          v2.warn("drop FPS ratio greater than max allowed value for currentLevel: " + h2), h2 > 0 && (c.autoLevelCapping === -1 || c.autoLevelCapping >= h2) && (h2 = h2 - 1, c.trigger(m2.FPS_DROP_LEVEL_CAPPING, { level: h2, droppedLevel: c.currentLevel }), c.autoLevelCapping = h2, this.streamController.nextLevelSwitch());
        }
      }
      this.lastTime = i, this.lastDroppedFrames = s2, this.lastDecodedFrames = t;
    }
  }
  checkFPSInterval() {
    let e = this.media;
    if (e)
      if (this.isVideoPlaybackQualityAvailable) {
        let t = e.getVideoPlaybackQuality();
        this.checkFPS(e, t.totalVideoFrames, t.droppedVideoFrames);
      } else
        this.checkFPS(e, e.webkitDecodedFrameCount, e.webkitDroppedFrameCount);
  }
};
var Et = "[eme]";
var zt = class n9 {
  constructor(e) {
    this.hls = void 0, this.config = void 0, this.media = null, this.keyFormatPromise = null, this.keySystemAccessPromises = {}, this._requestLicenseFailureCount = 0, this.mediaKeySessions = [], this.keyIdToKeySessionPromise = {}, this.setMediaKeysQueue = n9.CDMCleanupPromise ? [n9.CDMCleanupPromise] : [], this.onMediaEncrypted = this._onMediaEncrypted.bind(this), this.onWaitingForKey = this._onWaitingForKey.bind(this), this.debug = v2.debug.bind(v2, Et), this.log = v2.log.bind(v2, Et), this.warn = v2.warn.bind(v2, Et), this.error = v2.error.bind(v2, Et), this.hls = e, this.config = e.config, this.registerListeners();
  }
  destroy() {
    this.unregisterListeners(), this.onMediaDetached();
    let e = this.config;
    e.requestMediaKeySystemAccessFunc = null, e.licenseXhrSetup = e.licenseResponseCallback = void 0, e.drmSystems = e.drmSystemOptions = {}, this.hls = this.onMediaEncrypted = this.onWaitingForKey = this.keyIdToKeySessionPromise = null, this.config = null;
  }
  registerListeners() {
    this.hls.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.on(m2.MEDIA_DETACHED, this.onMediaDetached, this), this.hls.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), this.hls.on(m2.MANIFEST_LOADED, this.onManifestLoaded, this);
  }
  unregisterListeners() {
    this.hls.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), this.hls.off(m2.MEDIA_DETACHED, this.onMediaDetached, this), this.hls.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), this.hls.off(m2.MANIFEST_LOADED, this.onManifestLoaded, this);
  }
  getLicenseServerUrl(e) {
    let { drmSystems: t, widevineLicenseUrl: s2 } = this.config, i = t[e];
    if (i)
      return i.licenseUrl;
    if (e === J.WIDEVINE && s2)
      return s2;
    throw new Error(`no license server URL configured for key-system "${e}"`);
  }
  getServerCertificateUrl(e) {
    let { drmSystems: t } = this.config, s2 = t[e];
    if (s2)
      return s2.serverCertificateUrl;
    this.log(`No Server Certificate in config.drmSystems["${e}"]`);
  }
  attemptKeySystemAccess(e) {
    let t = this.hls.levels, s2 = (a2, o, l2) => !!a2 && l2.indexOf(a2) === o, i = t.map((a2) => a2.audioCodec).filter(s2), r2 = t.map((a2) => a2.videoCodec).filter(s2);
    return i.length + r2.length === 0 && r2.push("avc1.42e01e"), new Promise((a2, o) => {
      let l2 = (c) => {
        let h2 = c.shift();
        this.getMediaKeysPromise(h2, i, r2).then((u2) => a2({ keySystem: h2, mediaKeys: u2 })).catch((u2) => {
          c.length ? l2(c) : u2 instanceof de3 ? o(u2) : o(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_NO_ACCESS, error: u2, fatal: true }, u2.message));
        });
      };
      l2(e);
    });
  }
  requestMediaKeySystemAccess(e, t) {
    let { requestMediaKeySystemAccessFunc: s2 } = this.config;
    if (typeof s2 != "function") {
      let i = `Configured requestMediaKeySystemAccess is not a function ${s2}`;
      return Gr === null && self.location.protocol === "http:" && (i = `navigator.requestMediaKeySystemAccess is not available over insecure protocol ${location.protocol}`), Promise.reject(new Error(i));
    }
    return s2(e, t);
  }
  getMediaKeysPromise(e, t, s2) {
    let i = na(e, t, s2, this.config.drmSystemOptions), r2 = this.keySystemAccessPromises[e], a2 = r2?.keySystemAccess;
    if (!a2) {
      this.log(`Requesting encrypted media "${e}" key-system access with config: ${JSON.stringify(i)}`), a2 = this.requestMediaKeySystemAccess(e, i);
      let o = this.keySystemAccessPromises[e] = { keySystemAccess: a2 };
      return a2.catch((l2) => {
        this.log(`Failed to obtain access to key-system "${e}": ${l2}`);
      }), a2.then((l2) => {
        this.log(`Access for key-system "${l2.keySystem}" obtained`);
        let c = this.fetchServerCertificate(e);
        return this.log(`Create media-keys for "${e}"`), o.mediaKeys = l2.createMediaKeys().then((h2) => (this.log(`Media-keys created for "${e}"`), c.then((u2) => u2 ? this.setMediaKeysServerCertificate(h2, e, u2) : h2))), o.mediaKeys.catch((h2) => {
          this.error(`Failed to create media-keys for "${e}"}: ${h2}`);
        }), o.mediaKeys;
      });
    }
    return a2.then(() => r2.mediaKeys);
  }
  createMediaKeySessionContext({ decryptdata: e, keySystem: t, mediaKeys: s2 }) {
    this.log(`Creating key-system session "${t}" keyId: ${ve3.hexDump(e.keyId || [])}`);
    let i = s2.createSession(), r2 = { decryptdata: e, keySystem: t, mediaKeys: s2, mediaKeysSession: i, keyStatus: "status-pending" };
    return this.mediaKeySessions.push(r2), r2;
  }
  renewKeySession(e) {
    let t = e.decryptdata;
    if (t.pssh) {
      let s2 = this.createMediaKeySessionContext(e), i = this.getKeyIdString(t), r2 = "cenc";
      this.keyIdToKeySessionPromise[i] = this.generateRequestWithPreferredKeySession(s2, r2, t.pssh, "expired");
    } else
      this.warn("Could not renew expired session. Missing pssh initData.");
    this.removeSession(e);
  }
  getKeyIdString(e) {
    if (!e)
      throw new Error("Could not read keyId of undefined decryptdata");
    if (e.keyId === null)
      throw new Error("keyId is null");
    return ve3.hexDump(e.keyId);
  }
  updateKeySession(e, t) {
    var s2;
    let i = e.mediaKeysSession;
    return this.log(`Updating key-session "${i.sessionId}" for keyID ${ve3.hexDump(((s2 = e.decryptdata) == null ? void 0 : s2.keyId) || [])}
      } (data length: ${t && t.byteLength})`), i.update(t);
  }
  selectKeySystemFormat(e) {
    let t = Object.keys(e.levelkeys || {});
    return this.keyFormatPromise || (this.log(`Selecting key-system from fragment (sn: ${e.sn} ${e.type}: ${e.level}) key formats ${t.join(", ")}`), this.keyFormatPromise = this.getKeyFormatPromise(t)), this.keyFormatPromise;
  }
  getKeyFormatPromise(e) {
    return new Promise((t, s2) => {
      let i = ts(this.config), r2 = e.map(Vi).filter((a2) => !!a2 && i.indexOf(a2) !== -1);
      return this.getKeySystemSelectionPromise(r2).then(({ keySystem: a2 }) => {
        let o = Wi(a2);
        o ? t(o) : s2(new Error(`Unable to find format for key-system "${a2}"`));
      }).catch(s2);
    });
  }
  loadKey(e) {
    let t = e.keyInfo.decryptdata, s2 = this.getKeyIdString(t), i = `(keyId: ${s2} format: "${t.keyFormat}" method: ${t.method} uri: ${t.uri})`;
    this.log(`Starting session for key ${i}`);
    let r2 = this.keyIdToKeySessionPromise[s2];
    return r2 || (r2 = this.keyIdToKeySessionPromise[s2] = this.getKeySystemForKeyPromise(t).then(({ keySystem: a2, mediaKeys: o }) => (this.throwIfDestroyed(), this.log(`Handle encrypted media sn: ${e.frag.sn} ${e.frag.type}: ${e.frag.level} using key ${i}`), this.attemptSetMediaKeys(a2, o).then(() => {
      this.throwIfDestroyed();
      let l2 = this.createMediaKeySessionContext({ keySystem: a2, mediaKeys: o, decryptdata: t });
      return this.generateRequestWithPreferredKeySession(l2, "cenc", t.pssh, "playlist-key");
    }))), r2.catch((a2) => this.handleError(a2))), r2;
  }
  throwIfDestroyed(e = "Invalid state") {
    if (!this.hls)
      throw new Error("invalid state");
  }
  handleError(e) {
    this.hls && (this.error(e.message), e instanceof de3 ? this.hls.trigger(m2.ERROR, e.data) : this.hls.trigger(m2.ERROR, { type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_NO_KEYS, error: e, fatal: true }));
  }
  getKeySystemForKeyPromise(e) {
    let t = this.getKeyIdString(e), s2 = this.keyIdToKeySessionPromise[t];
    if (!s2) {
      let i = Vi(e.keyFormat), r2 = i ? [i] : ts(this.config);
      return this.attemptKeySystemAccess(r2);
    }
    return s2;
  }
  getKeySystemSelectionPromise(e) {
    if (e.length || (e = ts(this.config)), e.length === 0)
      throw new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_NO_CONFIGURED_LICENSE, fatal: true }, `Missing key-system license configuration options ${JSON.stringify({ drmSystems: this.config.drmSystems })}`);
    return this.attemptKeySystemAccess(e);
  }
  _onMediaEncrypted(e) {
    let { initDataType: t, initData: s2 } = e;
    if (this.debug(`"${e.type}" event: init data type: "${t}"`), s2 === null)
      return;
    let i, r2;
    if (t === "sinf" && this.config.drmSystems[J.FAIRPLAY]) {
      let h2 = se2(new Uint8Array(s2));
      try {
        let u2 = _i(JSON.parse(h2).sinf), d3 = zr(new Uint8Array(u2));
        if (!d3)
          return;
        i = d3.subarray(8, 24), r2 = J.FAIRPLAY;
      } catch {
        this.warn('Failed to parse sinf "encrypted" event message initData');
        return;
      }
    } else {
      let h2 = wa(s2);
      if (h2 === null)
        return;
      h2.version === 0 && h2.systemId === $r.WIDEVINE && h2.data && (i = h2.data.subarray(8, 24)), r2 = ra(h2.systemId);
    }
    if (!r2 || !i)
      return;
    let a2 = ve3.hexDump(i), { keyIdToKeySessionPromise: o, mediaKeySessions: l2 } = this, c = o[a2];
    for (let h2 = 0; h2 < l2.length; h2++) {
      let u2 = l2[h2], d3 = u2.decryptdata;
      if (d3.pssh || !d3.keyId)
        continue;
      let f3 = ve3.hexDump(d3.keyId);
      if (a2 === f3 || d3.uri.replace(/-/g, "").indexOf(a2) !== -1) {
        c = o[f3], delete o[f3], d3.pssh = new Uint8Array(s2), d3.keyId = i, c = o[a2] = c.then(() => this.generateRequestWithPreferredKeySession(u2, t, s2, "encrypted-event-key-match"));
        break;
      }
    }
    c || (c = o[a2] = this.getKeySystemSelectionPromise([r2]).then(({ keySystem: h2, mediaKeys: u2 }) => {
      var d3;
      this.throwIfDestroyed();
      let f3 = new it("ISO-23001-7", a2, (d3 = Wi(h2)) != null ? d3 : "");
      return f3.pssh = new Uint8Array(s2), f3.keyId = i, this.attemptSetMediaKeys(h2, u2).then(() => {
        this.throwIfDestroyed();
        let g2 = this.createMediaKeySessionContext({ decryptdata: f3, keySystem: h2, mediaKeys: u2 });
        return this.generateRequestWithPreferredKeySession(g2, t, s2, "encrypted-event-no-match");
      });
    })), c.catch((h2) => this.handleError(h2));
  }
  _onWaitingForKey(e) {
    this.log(`"${e.type}" event`);
  }
  attemptSetMediaKeys(e, t) {
    let s2 = this.setMediaKeysQueue.slice();
    this.log(`Setting media-keys for "${e}"`);
    let i = Promise.all(s2).then(() => {
      if (!this.media)
        throw new Error("Attempted to set mediaKeys without media element attached");
      return this.media.setMediaKeys(t);
    });
    return this.setMediaKeysQueue.push(i), i.then(() => {
      this.log(`Media-keys set for "${e}"`), s2.push(i), this.setMediaKeysQueue = this.setMediaKeysQueue.filter((r2) => s2.indexOf(r2) === -1);
    });
  }
  generateRequestWithPreferredKeySession(e, t, s2, i) {
    var r2, a2;
    let o = (r2 = this.config.drmSystems) == null || (a2 = r2[e.keySystem]) == null ? void 0 : a2.generateRequest;
    if (o)
      try {
        let g2 = o.call(this.hls, t, s2, e);
        if (!g2)
          throw new Error("Invalid response from configured generateRequest filter");
        t = g2.initDataType, s2 = e.decryptdata.pssh = g2.initData ? new Uint8Array(g2.initData) : null;
      } catch (g2) {
        var l2;
        if (this.warn(g2.message), (l2 = this.hls) != null && l2.config.debug)
          throw g2;
      }
    if (s2 === null)
      return this.log(`Skipping key-session request for "${i}" (no initData)`), Promise.resolve(e);
    let c = this.getKeyIdString(e.decryptdata);
    this.log(`Generating key-session request for "${i}": ${c} (init data type: ${t} length: ${s2 ? s2.byteLength : null})`);
    let h2 = new Ui(), u2 = e._onmessage = (g2) => {
      let p3 = e.mediaKeysSession;
      if (!p3) {
        h2.emit("error", new Error("invalid state"));
        return;
      }
      let { messageType: T2, message: E4 } = g2;
      this.log(`"${T2}" message event for session "${p3.sessionId}" message size: ${E4.byteLength}`), T2 === "license-request" || T2 === "license-renewal" ? this.renewLicense(e, E4).catch((x3) => {
        this.handleError(x3), h2.emit("error", x3);
      }) : T2 === "license-release" ? e.keySystem === J.FAIRPLAY && (this.updateKeySession(e, As("acknowledged")), this.removeSession(e)) : this.warn(`unhandled media key message type "${T2}"`);
    }, d3 = e._onkeystatuseschange = (g2) => {
      if (!e.mediaKeysSession) {
        h2.emit("error", new Error("invalid state"));
        return;
      }
      this.onKeyStatusChange(e);
      let T2 = e.keyStatus;
      h2.emit("keyStatus", T2), T2 === "expired" && (this.warn(`${e.keySystem} expired for key ${c}`), this.renewKeySession(e));
    };
    e.mediaKeysSession.addEventListener("message", u2), e.mediaKeysSession.addEventListener("keystatuseschange", d3);
    let f3 = new Promise((g2, p3) => {
      h2.on("error", p3), h2.on("keyStatus", (T2) => {
        T2.startsWith("usable") ? g2() : T2 === "output-restricted" ? p3(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_STATUS_OUTPUT_RESTRICTED, fatal: false }, "HDCP level output restricted")) : T2 === "internal-error" ? p3(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_STATUS_INTERNAL_ERROR, fatal: true }, `key status changed to "${T2}"`)) : T2 === "expired" ? p3(new Error("key expired while generating request")) : this.warn(`unhandled key status change "${T2}"`);
      });
    });
    return e.mediaKeysSession.generateRequest(t, s2).then(() => {
      var g2;
      this.log(`Request generated for key-session "${(g2 = e.mediaKeysSession) == null ? void 0 : g2.sessionId}" keyId: ${c}`);
    }).catch((g2) => {
      throw new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_NO_SESSION, error: g2, fatal: false }, `Error generating key-session request: ${g2}`);
    }).then(() => f3).catch((g2) => {
      throw h2.removeAllListeners(), this.removeSession(e), g2;
    }).then(() => (h2.removeAllListeners(), e));
  }
  onKeyStatusChange(e) {
    e.mediaKeysSession.keyStatuses.forEach((t, s2) => {
      this.log(`key status change "${t}" for keyStatuses keyId: ${ve3.hexDump("buffer" in s2 ? new Uint8Array(s2.buffer, s2.byteOffset, s2.byteLength) : new Uint8Array(s2))} session keyId: ${ve3.hexDump(new Uint8Array(e.decryptdata.keyId || []))} uri: ${e.decryptdata.uri}`), e.keyStatus = t;
    });
  }
  fetchServerCertificate(e) {
    let t = this.config, s2 = t.loader, i = new s2(t), r2 = this.getServerCertificateUrl(e);
    return r2 ? (this.log(`Fetching server certificate for "${e}"`), new Promise((a2, o) => {
      let l2 = { responseType: "arraybuffer", url: r2 }, c = t.certLoadPolicy.default, h2 = { loadPolicy: c, timeout: c.maxLoadTimeMs, maxRetry: 0, retryDelay: 0, maxRetryDelay: 0 }, u2 = { onSuccess: (d3, f3, g2, p3) => {
        a2(d3.data);
      }, onError: (d3, f3, g2, p3) => {
        o(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_SERVER_CERTIFICATE_REQUEST_FAILED, fatal: true, networkDetails: g2, response: le3({ url: l2.url, data: void 0 }, d3) }, `"${e}" certificate request failed (${r2}). Status: ${d3.code} (${d3.text})`));
      }, onTimeout: (d3, f3, g2) => {
        o(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_SERVER_CERTIFICATE_REQUEST_FAILED, fatal: true, networkDetails: g2, response: { url: l2.url, data: void 0 } }, `"${e}" certificate request timed out (${r2})`));
      }, onAbort: (d3, f3, g2) => {
        o(new Error("aborted"));
      } };
      i.load(l2, h2, u2);
    })) : Promise.resolve();
  }
  setMediaKeysServerCertificate(e, t, s2) {
    return new Promise((i, r2) => {
      e.setServerCertificate(s2).then((a2) => {
        this.log(`setServerCertificate ${a2 ? "success" : "not supported by CDM"} (${s2?.byteLength}) on "${t}"`), i(e);
      }).catch((a2) => {
        r2(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_SERVER_CERTIFICATE_UPDATE_FAILED, error: a2, fatal: true }, a2.message));
      });
    });
  }
  renewLicense(e, t) {
    return this.requestLicense(e, new Uint8Array(t)).then((s2) => this.updateKeySession(e, new Uint8Array(s2)).catch((i) => {
      throw new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_SESSION_UPDATE_FAILED, error: i, fatal: true }, i.message);
    }));
  }
  unpackPlayReadyKeyMessage(e, t) {
    let s2 = String.fromCharCode.apply(null, new Uint16Array(t.buffer));
    if (!s2.includes("PlayReadyKeyMessage"))
      return e.setRequestHeader("Content-Type", "text/xml; charset=utf-8"), t;
    let i = new DOMParser().parseFromString(s2, "application/xml"), r2 = i.querySelectorAll("HttpHeader");
    if (r2.length > 0) {
      let h2;
      for (let u2 = 0, d3 = r2.length; u2 < d3; u2++) {
        var a2, o;
        h2 = r2[u2];
        let f3 = (a2 = h2.querySelector("name")) == null ? void 0 : a2.textContent, g2 = (o = h2.querySelector("value")) == null ? void 0 : o.textContent;
        f3 && g2 && e.setRequestHeader(f3, g2);
      }
    }
    let l2 = i.querySelector("Challenge"), c = l2?.textContent;
    if (!c)
      throw new Error("Cannot find <Challenge> in key message");
    return As(atob(c));
  }
  setupLicenseXHR(e, t, s2, i) {
    let r2 = this.config.licenseXhrSetup;
    return r2 ? Promise.resolve().then(() => {
      if (!s2.decryptdata)
        throw new Error("Key removed");
      return r2.call(this.hls, e, t, s2, i);
    }).catch((a2) => {
      if (!s2.decryptdata)
        throw a2;
      return e.open("POST", t, true), r2.call(this.hls, e, t, s2, i);
    }).then((a2) => (e.readyState || e.open("POST", t, true), { xhr: e, licenseChallenge: a2 || i })) : (e.open("POST", t, true), Promise.resolve({ xhr: e, licenseChallenge: i }));
  }
  requestLicense(e, t) {
    let s2 = this.config.keyLoadPolicy.default;
    return new Promise((i, r2) => {
      let a2 = this.getLicenseServerUrl(e.keySystem);
      this.log(`Sending license request to URL: ${a2}`);
      let o = new XMLHttpRequest();
      o.responseType = "arraybuffer", o.onreadystatechange = () => {
        if (!this.hls || !e.mediaKeysSession)
          return r2(new Error("invalid state"));
        if (o.readyState === 4)
          if (o.status === 200) {
            this._requestLicenseFailureCount = 0;
            let l2 = o.response;
            this.log(`License received ${l2 instanceof ArrayBuffer ? l2.byteLength : l2}`);
            let c = this.config.licenseResponseCallback;
            if (c)
              try {
                l2 = c.call(this.hls, o, a2, e);
              } catch (h2) {
                this.error(h2);
              }
            i(l2);
          } else {
            let l2 = s2.errorRetry, c = l2 ? l2.maxNumRetry : 0;
            if (this._requestLicenseFailureCount++, this._requestLicenseFailureCount > c || o.status >= 400 && o.status < 500)
              r2(new de3({ type: B2.KEY_SYSTEM_ERROR, details: A2.KEY_SYSTEM_LICENSE_REQUEST_FAILED, fatal: true, networkDetails: o, response: { url: a2, data: void 0, code: o.status, text: o.statusText } }, `License Request XHR failed (${a2}). Status: ${o.status} (${o.statusText})`));
            else {
              let h2 = c - this._requestLicenseFailureCount + 1;
              this.warn(`Retrying license request, ${h2} attempts left`), this.requestLicense(e, t).then(i, r2);
            }
          }
      }, e.licenseXhr && e.licenseXhr.readyState !== XMLHttpRequest.DONE && e.licenseXhr.abort(), e.licenseXhr = o, this.setupLicenseXHR(o, a2, e, t).then(({ xhr: l2, licenseChallenge: c }) => {
        e.keySystem == J.PLAYREADY && (c = this.unpackPlayReadyKeyMessage(l2, c)), l2.send(c);
      });
    });
  }
  onMediaAttached(e, t) {
    if (!this.config.emeEnabled)
      return;
    let s2 = t.media;
    this.media = s2, s2.addEventListener("encrypted", this.onMediaEncrypted), s2.addEventListener("waitingforkey", this.onWaitingForKey);
  }
  onMediaDetached() {
    let e = this.media, t = this.mediaKeySessions;
    e && (e.removeEventListener("encrypted", this.onMediaEncrypted), e.removeEventListener("waitingforkey", this.onWaitingForKey), this.media = null), this._requestLicenseFailureCount = 0, this.setMediaKeysQueue = [], this.mediaKeySessions = [], this.keyIdToKeySessionPromise = {}, it.clearKeyUriToKeyIdMap();
    let s2 = t.length;
    n9.CDMCleanupPromise = Promise.all(t.map((i) => this.removeSession(i)).concat(e?.setMediaKeys(null).catch((i) => {
      this.log(`Could not clear media keys: ${i}`);
    }))).then(() => {
      s2 && (this.log("finished closing key sessions and clearing media keys"), t.length = 0);
    }).catch((i) => {
      this.log(`Could not close sessions and clear media keys: ${i}`);
    });
  }
  onManifestLoading() {
    this.keyFormatPromise = null;
  }
  onManifestLoaded(e, { sessionKeys: t }) {
    if (!(!t || !this.config.emeEnabled) && !this.keyFormatPromise) {
      let s2 = t.reduce((i, r2) => (i.indexOf(r2.keyFormat) === -1 && i.push(r2.keyFormat), i), []);
      this.log(`Selecting key-system from session-keys ${s2.join(", ")}`), this.keyFormatPromise = this.getKeyFormatPromise(s2);
    }
  }
  removeSession(e) {
    let { mediaKeysSession: t, licenseXhr: s2 } = e;
    if (t) {
      this.log(`Remove licenses and keys and close session ${t.sessionId}`), e._onmessage && (t.removeEventListener("message", e._onmessage), e._onmessage = void 0), e._onkeystatuseschange && (t.removeEventListener("keystatuseschange", e._onkeystatuseschange), e._onkeystatuseschange = void 0), s2 && s2.readyState !== XMLHttpRequest.DONE && s2.abort(), e.mediaKeysSession = e.decryptdata = e.licenseXhr = void 0;
      let i = this.mediaKeySessions.indexOf(e);
      return i > -1 && this.mediaKeySessions.splice(i, 1), t.remove().catch((r2) => {
        this.log(`Could not remove session: ${r2}`);
      }).then(() => t.close()).catch((r2) => {
        this.log(`Could not close session: ${r2}`);
      });
    }
  }
};
zt.CDMCleanupPromise = void 0;
var de3 = class extends Error {
  constructor(e, t) {
    super(t), this.data = void 0, e.error || (e.error = new Error(t)), this.data = e, e.err = e.error;
  }
};
var he3;
(function(n12) {
  n12.MANIFEST = "m", n12.AUDIO = "a", n12.VIDEO = "v", n12.MUXED = "av", n12.INIT = "i", n12.CAPTION = "c", n12.TIMED_TEXT = "tt", n12.KEY = "k", n12.OTHER = "o";
})(he3 || (he3 = {}));
var Ei;
(function(n12) {
  n12.DASH = "d", n12.HLS = "h", n12.SMOOTH = "s", n12.OTHER = "o";
})(Ei || (Ei = {}));
var Oe2;
(function(n12) {
  n12.OBJECT = "CMCD-Object", n12.REQUEST = "CMCD-Request", n12.SESSION = "CMCD-Session", n12.STATUS = "CMCD-Status";
})(Oe2 || (Oe2 = {}));
var fl = { [Oe2.OBJECT]: ["br", "d", "ot", "tb"], [Oe2.REQUEST]: ["bl", "dl", "mtp", "nor", "nrr", "su"], [Oe2.SESSION]: ["cid", "pr", "sf", "sid", "st", "v"], [Oe2.STATUS]: ["bs", "rtp"] };
var ht = class n10 {
  constructor(e, t) {
    this.value = void 0, this.params = void 0, Array.isArray(e) && (e = e.map((s2) => s2 instanceof n10 ? s2 : new n10(s2))), this.value = e, this.params = t;
  }
};
var Xt = class {
  constructor(e) {
    this.description = void 0, this.description = e;
  }
};
var gl = "Dict";
function ml(n12) {
  return Array.isArray(n12) ? JSON.stringify(n12) : n12 instanceof Map ? "Map{}" : n12 instanceof Set ? "Set{}" : typeof n12 == "object" ? JSON.stringify(n12) : String(n12);
}
function pl(n12, e, t, s2) {
  return new Error(`failed to ${n12} "${ml(e)}" as ${t}`, { cause: s2 });
}
var Fr = "Bare Item";
var Tl = "Boolean";
var El = "Byte Sequence";
var yl = "Decimal";
var xl = "Integer";
function Sl(n12) {
  return n12 < -999999999999999 || 999999999999999 < n12;
}
var vl = /[\x00-\x1f\x7f]+/;
var Al = "Token";
var Ll = "Key";
function Ie3(n12, e, t) {
  return pl("serialize", n12, e, t);
}
function Rl(n12) {
  if (typeof n12 != "boolean")
    throw Ie3(n12, Tl);
  return n12 ? "?1" : "?0";
}
function Il(n12) {
  return btoa(String.fromCharCode(...n12));
}
function bl(n12) {
  if (ArrayBuffer.isView(n12) === false)
    throw Ie3(n12, El);
  return `:${Il(n12)}:`;
}
function Fn(n12) {
  if (Sl(n12))
    throw Ie3(n12, xl);
  return n12.toString();
}
function Dl(n12) {
  return `@${Fn(n12.getTime() / 1e3)}`;
}
function Mn(n12, e) {
  if (n12 < 0)
    return -Mn(-n12, e);
  let t = Math.pow(10, e);
  if (Math.abs(n12 * t % 1 - 0.5) < Number.EPSILON) {
    let i = Math.floor(n12 * t);
    return (i % 2 === 0 ? i : i + 1) / t;
  } else
    return Math.round(n12 * t) / t;
}
function Cl(n12) {
  let e = Mn(n12, 3);
  if (Math.floor(Math.abs(e)).toString().length > 12)
    throw Ie3(n12, yl);
  let t = e.toString();
  return t.includes(".") ? t : `${t}.0`;
}
var wl = "String";
function _l(n12) {
  if (vl.test(n12))
    throw Ie3(n12, wl);
  return `"${n12.replace(/\\/g, "\\\\").replace(/"/g, '\\"')}"`;
}
function Pl(n12) {
  return n12.description || n12.toString().slice(7, -1);
}
function Mr(n12) {
  let e = Pl(n12);
  if (/^([a-zA-Z*])([!#$%&'*+\-.^_`|~\w:/]*)$/.test(e) === false)
    throw Ie3(e, Al);
  return e;
}
function yi(n12) {
  switch (typeof n12) {
    case "number":
      if (!F(n12))
        throw Ie3(n12, Fr);
      return Number.isInteger(n12) ? Fn(n12) : Cl(n12);
    case "string":
      return _l(n12);
    case "symbol":
      return Mr(n12);
    case "boolean":
      return Rl(n12);
    case "object":
      if (n12 instanceof Date)
        return Dl(n12);
      if (n12 instanceof Uint8Array)
        return bl(n12);
      if (n12 instanceof Xt)
        return Mr(n12);
    default:
      throw Ie3(n12, Fr);
  }
}
function xi(n12) {
  if (/^[a-z*][a-z0-9\-_.*]*$/.test(n12) === false)
    throw Ie3(n12, Ll);
  return n12;
}
function Gi(n12) {
  return n12 == null ? "" : Object.entries(n12).map(([e, t]) => t === true ? `;${xi(e)}` : `;${xi(e)}=${yi(t)}`).join("");
}
function On(n12) {
  return n12 instanceof ht ? `${yi(n12.value)}${Gi(n12.params)}` : yi(n12);
}
function kl(n12) {
  return `(${n12.value.map(On).join(" ")})${Gi(n12.params)}`;
}
function Fl(n12, e = { whitespace: true }) {
  if (typeof n12 != "object")
    throw Ie3(n12, gl);
  let t = n12 instanceof Map ? n12.entries() : Object.entries(n12), s2 = e != null && e.whitespace ? " " : "";
  return Array.from(t).map(([i, r2]) => {
    r2 instanceof ht || (r2 = new ht(r2));
    let a2 = xi(i);
    return r2.value === true ? a2 += Gi(r2.params) : (a2 += "=", Array.isArray(r2.value) ? a2 += kl(r2) : a2 += On(r2)), a2;
  }).join(`,${s2}`);
}
function Ml(n12, e) {
  return Fl(n12, e);
}
var Ol = (n12) => n12 === "ot" || n12 === "sf" || n12 === "st";
var Nl = (n12) => typeof n12 == "number" ? F(n12) : n12 != null && n12 !== "" && n12 !== false;
function Ul(n12, e) {
  let t = new URL(n12), s2 = new URL(e);
  if (t.origin !== s2.origin)
    return n12;
  let i = t.pathname.split("/").slice(1), r2 = s2.pathname.split("/").slice(1, -1);
  for (; i[0] === r2[0]; )
    i.shift(), r2.shift();
  for (; r2.length; )
    r2.shift(), i.unshift("..");
  return i.join("/");
}
function Bl() {
  try {
    return crypto.randomUUID();
  } catch {
    try {
      let e = URL.createObjectURL(new Blob()), t = e.toString();
      return URL.revokeObjectURL(e), t.slice(t.lastIndexOf("/") + 1);
    } catch {
      let t = (/* @__PURE__ */ new Date()).getTime();
      return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, (i) => {
        let r2 = (t + Math.random() * 16) % 16 | 0;
        return t = Math.floor(t / 16), (i == "x" ? r2 : r2 & 3 | 8).toString(16);
      });
    }
  }
}
var Rt = (n12) => Math.round(n12);
var $l = (n12, e) => (e != null && e.baseUrl && (n12 = Ul(n12, e.baseUrl)), encodeURIComponent(n12));
var yt = (n12) => Rt(n12 / 100) * 100;
var Gl = { br: Rt, d: Rt, bl: yt, dl: yt, mtp: yt, nor: $l, rtp: yt, tb: Rt };
function Kl(n12, e) {
  let t = {};
  if (n12 == null || typeof n12 != "object")
    return t;
  let s2 = Object.keys(n12).sort(), i = te2({}, Gl, e?.formatters), r2 = e?.filter;
  return s2.forEach((a2) => {
    if (r2 != null && r2(a2))
      return;
    let o = n12[a2], l2 = i[a2];
    l2 && (o = l2(o, e)), !(a2 === "v" && o === 1) && (a2 == "pr" && o === 1 || Nl(o) && (Ol(a2) && typeof o == "string" && (o = new Xt(o)), t[a2] = o));
  }), t;
}
function Nn(n12, e = {}) {
  return n12 ? Ml(Kl(n12, e), te2({ whitespace: false }, e)) : "";
}
function Hl(n12, e = {}) {
  if (!n12)
    return {};
  let t = Object.entries(n12), s2 = Object.entries(fl).concat(Object.entries(e?.customHeaderMap || {})), i = t.reduce((r2, a2) => {
    var o, l2;
    let [c, h2] = a2, u2 = ((o = s2.find((d3) => d3[1].includes(c))) == null ? void 0 : o[0]) || Oe2.REQUEST;
    return (l2 = r2[u2]) != null || (r2[u2] = {}), r2[u2][c] = h2, r2;
  }, {});
  return Object.entries(i).reduce((r2, [a2, o]) => (r2[a2] = Nn(o, e), r2), {});
}
function Vl(n12, e, t) {
  return te2(n12, Hl(e, t));
}
var Wl = "CMCD";
function Yl(n12, e = {}) {
  if (!n12)
    return "";
  let t = Nn(n12, e);
  return `${Wl}=${encodeURIComponent(t)}`;
}
var Or = /CMCD=[^&#]+/;
function ql(n12, e, t) {
  let s2 = Yl(e, t);
  if (!s2)
    return n12;
  if (Or.test(n12))
    return n12.replace(Or, s2);
  let i = n12.includes("?") ? "&" : "?";
  return `${n12}${i}${s2}`;
}
var Si = class {
  constructor(e) {
    this.hls = void 0, this.config = void 0, this.media = void 0, this.sid = void 0, this.cid = void 0, this.useHeaders = false, this.includeKeys = void 0, this.initialized = false, this.starved = false, this.buffering = true, this.audioBuffer = void 0, this.videoBuffer = void 0, this.onWaiting = () => {
      this.initialized && (this.starved = true), this.buffering = true;
    }, this.onPlaying = () => {
      this.initialized || (this.initialized = true), this.buffering = false;
    }, this.applyPlaylistData = (i) => {
      try {
        this.apply(i, { ot: he3.MANIFEST, su: !this.initialized });
      } catch (r2) {
        v2.warn("Could not generate manifest CMCD data.", r2);
      }
    }, this.applyFragmentData = (i) => {
      try {
        let r2 = i.frag, a2 = this.hls.levels[r2.level], o = this.getObjectType(r2), l2 = { d: r2.duration * 1e3, ot: o };
        (o === he3.VIDEO || o === he3.AUDIO || o == he3.MUXED) && (l2.br = a2.bitrate / 1e3, l2.tb = this.getTopBandwidth(o) / 1e3, l2.bl = this.getBufferLength(o)), this.apply(i, l2);
      } catch (r2) {
        v2.warn("Could not generate segment CMCD data.", r2);
      }
    }, this.hls = e;
    let t = this.config = e.config, { cmcd: s2 } = t;
    s2 != null && (t.pLoader = this.createPlaylistLoader(), t.fLoader = this.createFragmentLoader(), this.sid = s2.sessionId || Bl(), this.cid = s2.contentId, this.useHeaders = s2.useHeaders === true, this.includeKeys = s2.includeKeys, this.registerListeners());
  }
  registerListeners() {
    let e = this.hls;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHED, this.onMediaDetached, this), e.on(m2.BUFFER_CREATED, this.onBufferCreated, this);
  }
  unregisterListeners() {
    let e = this.hls;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHED, this.onMediaDetached, this), e.off(m2.BUFFER_CREATED, this.onBufferCreated, this);
  }
  destroy() {
    this.unregisterListeners(), this.onMediaDetached(), this.hls = this.config = this.audioBuffer = this.videoBuffer = null, this.onWaiting = this.onPlaying = null;
  }
  onMediaAttached(e, t) {
    this.media = t.media, this.media.addEventListener("waiting", this.onWaiting), this.media.addEventListener("playing", this.onPlaying);
  }
  onMediaDetached() {
    this.media && (this.media.removeEventListener("waiting", this.onWaiting), this.media.removeEventListener("playing", this.onPlaying), this.media = null);
  }
  onBufferCreated(e, t) {
    var s2, i;
    this.audioBuffer = (s2 = t.tracks.audio) == null ? void 0 : s2.buffer, this.videoBuffer = (i = t.tracks.video) == null ? void 0 : i.buffer;
  }
  createData() {
    var e;
    return { v: 1, sf: Ei.HLS, sid: this.sid, cid: this.cid, pr: (e = this.media) == null ? void 0 : e.playbackRate, mtp: this.hls.bandwidthEstimate / 1e3 };
  }
  apply(e, t = {}) {
    te2(t, this.createData());
    let s2 = t.ot === he3.INIT || t.ot === he3.VIDEO || t.ot === he3.MUXED;
    this.starved && s2 && (t.bs = true, t.su = true, this.starved = false), t.su == null && (t.su = this.buffering);
    let { includeKeys: i } = this;
    i && (t = Object.keys(t).reduce((r2, a2) => (i.includes(a2) && (r2[a2] = t[a2]), r2), {})), this.useHeaders ? (e.headers || (e.headers = {}), Vl(e.headers, t)) : e.url = ql(e.url, t);
  }
  getObjectType(e) {
    let { type: t } = e;
    if (t === "subtitle")
      return he3.TIMED_TEXT;
    if (e.sn === "initSegment")
      return he3.INIT;
    if (t === "audio")
      return he3.AUDIO;
    if (t === "main")
      return this.hls.audioTracks.length ? he3.VIDEO : he3.MUXED;
  }
  getTopBandwidth(e) {
    let t = 0, s2, i = this.hls;
    if (e === he3.AUDIO)
      s2 = i.audioTracks;
    else {
      let r2 = i.maxAutoLevel, a2 = r2 > -1 ? r2 + 1 : i.levels.length;
      s2 = i.levels.slice(0, a2);
    }
    for (let r2 of s2)
      r2.bitrate > t && (t = r2.bitrate);
    return t > 0 ? t : NaN;
  }
  getBufferLength(e) {
    let t = this.hls.media, s2 = e === he3.AUDIO ? this.audioBuffer : this.videoBuffer;
    return !s2 || !t ? NaN : Q.bufferInfo(s2, t.currentTime, this.config.maxBufferHole).len * 1e3;
  }
  createPlaylistLoader() {
    let { pLoader: e } = this.config, t = this.applyPlaylistData, s2 = e || this.config.loader;
    return class {
      constructor(r2) {
        this.loader = void 0, this.loader = new s2(r2);
      }
      get stats() {
        return this.loader.stats;
      }
      get context() {
        return this.loader.context;
      }
      destroy() {
        this.loader.destroy();
      }
      abort() {
        this.loader.abort();
      }
      load(r2, a2, o) {
        t(r2), this.loader.load(r2, a2, o);
      }
    };
  }
  createFragmentLoader() {
    let { fLoader: e } = this.config, t = this.applyFragmentData, s2 = e || this.config.loader;
    return class {
      constructor(r2) {
        this.loader = void 0, this.loader = new s2(r2);
      }
      get stats() {
        return this.loader.stats;
      }
      get context() {
        return this.loader.context;
      }
      destroy() {
        this.loader.destroy();
      }
      abort() {
        this.loader.abort();
      }
      load(r2, a2, o) {
        t(r2), this.loader.load(r2, a2, o);
      }
    };
  }
};
var jl = 3e5;
var vi = class {
  constructor(e) {
    this.hls = void 0, this.log = void 0, this.loader = null, this.uri = null, this.pathwayId = ".", this.pathwayPriority = null, this.timeToLoad = 300, this.reloadTimer = -1, this.updated = 0, this.started = false, this.enabled = true, this.levels = null, this.audioTracks = null, this.subtitleTracks = null, this.penalizedPathways = {}, this.hls = e, this.log = v2.log.bind(v2, "[content-steering]:"), this.registerListeners();
  }
  registerListeners() {
    let e = this.hls;
    e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.ERROR, this.onError, this);
  }
  unregisterListeners() {
    let e = this.hls;
    e && (e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.ERROR, this.onError, this));
  }
  startLoad() {
    if (this.started = true, this.clearTimeout(), this.enabled && this.uri) {
      if (this.updated) {
        let e = this.timeToLoad * 1e3 - (performance.now() - this.updated);
        if (e > 0) {
          this.scheduleRefresh(this.uri, e);
          return;
        }
      }
      this.loadSteeringManifest(this.uri);
    }
  }
  stopLoad() {
    this.started = false, this.loader && (this.loader.destroy(), this.loader = null), this.clearTimeout();
  }
  clearTimeout() {
    this.reloadTimer !== -1 && (self.clearTimeout(this.reloadTimer), this.reloadTimer = -1);
  }
  destroy() {
    this.unregisterListeners(), this.stopLoad(), this.hls = null, this.levels = this.audioTracks = this.subtitleTracks = null;
  }
  removeLevel(e) {
    let t = this.levels;
    t && (this.levels = t.filter((s2) => s2 !== e));
  }
  onManifestLoading() {
    this.stopLoad(), this.enabled = true, this.timeToLoad = 300, this.updated = 0, this.uri = null, this.pathwayId = ".", this.levels = this.audioTracks = this.subtitleTracks = null;
  }
  onManifestLoaded(e, t) {
    let { contentSteering: s2 } = t;
    s2 !== null && (this.pathwayId = s2.pathwayId, this.uri = s2.uri, this.started && this.startLoad());
  }
  onManifestParsed(e, t) {
    this.audioTracks = t.audioTracks, this.subtitleTracks = t.subtitleTracks;
  }
  onError(e, t) {
    let { errorAction: s2 } = t;
    if (s2?.action === ae3.SendAlternateToPenaltyBox && s2.flags === Te2.MoveAllAlternatesMatchingHost) {
      let i = this.levels, r2 = this.pathwayPriority, a2 = this.pathwayId;
      if (t.context) {
        let { groupId: o, pathwayId: l2, type: c } = t.context;
        o && i ? a2 = this.getPathwayForGroupId(o, c, a2) : l2 && (a2 = l2);
      }
      a2 in this.penalizedPathways || (this.penalizedPathways[a2] = performance.now()), !r2 && i && (r2 = i.reduce((o, l2) => (o.indexOf(l2.pathwayId) === -1 && o.push(l2.pathwayId), o), [])), r2 && r2.length > 1 && (this.updatePathwayPriority(r2), s2.resolved = this.pathwayId !== a2), s2.resolved || v2.warn(`Could not resolve ${t.details} ("${t.error.message}") with content-steering for Pathway: ${a2} levels: ${i && i.length} priorities: ${JSON.stringify(r2)} penalized: ${JSON.stringify(this.penalizedPathways)}`);
    }
  }
  filterParsedLevels(e) {
    this.levels = e;
    let t = this.getLevelsForPathway(this.pathwayId);
    if (t.length === 0) {
      let s2 = e[0].pathwayId;
      this.log(`No levels found in Pathway ${this.pathwayId}. Setting initial Pathway to "${s2}"`), t = this.getLevelsForPathway(s2), this.pathwayId = s2;
    }
    return t.length !== e.length ? (this.log(`Found ${t.length}/${e.length} levels in Pathway "${this.pathwayId}"`), t) : e;
  }
  getLevelsForPathway(e) {
    return this.levels === null ? [] : this.levels.filter((t) => e === t.pathwayId);
  }
  updatePathwayPriority(e) {
    this.pathwayPriority = e;
    let t, s2 = this.penalizedPathways, i = performance.now();
    Object.keys(s2).forEach((r2) => {
      i - s2[r2] > jl && delete s2[r2];
    });
    for (let r2 = 0; r2 < e.length; r2++) {
      let a2 = e[r2];
      if (a2 in s2)
        continue;
      if (a2 === this.pathwayId)
        return;
      let o = this.hls.nextLoadLevel, l2 = this.hls.levels[o];
      if (t = this.getLevelsForPathway(a2), t.length > 0) {
        this.log(`Setting Pathway to "${a2}"`), this.pathwayId = a2, an2(t), this.hls.trigger(m2.LEVELS_UPDATED, { levels: t });
        let c = this.hls.levels[o];
        l2 && c && this.levels && (c.attrs["STABLE-VARIANT-ID"] !== l2.attrs["STABLE-VARIANT-ID"] && c.bitrate !== l2.bitrate && this.log(`Unstable Pathways change from bitrate ${l2.bitrate} to ${c.bitrate}`), this.hls.nextLoadLevel = o);
        break;
      }
    }
  }
  getPathwayForGroupId(e, t, s2) {
    let i = this.getLevelsForPathway(s2).concat(this.levels || []);
    for (let r2 = 0; r2 < i.length; r2++)
      if (t === W3.AUDIO_TRACK && i[r2].hasAudioGroup(e) || t === W3.SUBTITLE_TRACK && i[r2].hasSubtitleGroup(e))
        return i[r2].pathwayId;
    return s2;
  }
  clonePathways(e) {
    let t = this.levels;
    if (!t)
      return;
    let s2 = {}, i = {};
    e.forEach((r2) => {
      let { ID: a2, "BASE-ID": o, "URI-REPLACEMENT": l2 } = r2;
      if (t.some((h2) => h2.pathwayId === a2))
        return;
      let c = this.getLevelsForPathway(o).map((h2) => {
        let u2 = new Z2(h2.attrs);
        u2["PATHWAY-ID"] = a2;
        let d3 = u2.AUDIO && `${u2.AUDIO}_clone_${a2}`, f3 = u2.SUBTITLES && `${u2.SUBTITLES}_clone_${a2}`;
        d3 && (s2[u2.AUDIO] = d3, u2.AUDIO = d3), f3 && (i[u2.SUBTITLES] = f3, u2.SUBTITLES = f3);
        let g2 = Un(h2.uri, u2["STABLE-VARIANT-ID"], "PER-VARIANT-URIS", l2), p3 = new Pe3({ attrs: u2, audioCodec: h2.audioCodec, bitrate: h2.bitrate, height: h2.height, name: h2.name, url: g2, videoCodec: h2.videoCodec, width: h2.width });
        if (h2.audioGroups)
          for (let T2 = 1; T2 < h2.audioGroups.length; T2++)
            p3.addGroupId("audio", `${h2.audioGroups[T2]}_clone_${a2}`);
        if (h2.subtitleGroups)
          for (let T2 = 1; T2 < h2.subtitleGroups.length; T2++)
            p3.addGroupId("text", `${h2.subtitleGroups[T2]}_clone_${a2}`);
        return p3;
      });
      t.push(...c), Nr(this.audioTracks, s2, l2, a2), Nr(this.subtitleTracks, i, l2, a2);
    });
  }
  loadSteeringManifest(e) {
    let t = this.hls.config, s2 = t.loader;
    this.loader && this.loader.destroy(), this.loader = new s2(t);
    let i;
    try {
      i = new self.URL(e);
    } catch {
      this.enabled = false, this.log(`Failed to parse Steering Manifest URI: ${e}`);
      return;
    }
    if (i.protocol !== "data:") {
      let h2 = (this.hls.bandwidthEstimate || t.abrEwmaDefaultEstimate) | 0;
      i.searchParams.set("_HLS_pathway", this.pathwayId), i.searchParams.set("_HLS_throughput", "" + h2);
    }
    let r2 = { responseType: "json", url: i.href }, a2 = t.steeringManifestLoadPolicy.default, o = a2.errorRetry || a2.timeoutRetry || {}, l2 = { loadPolicy: a2, timeout: a2.maxLoadTimeMs, maxRetry: o.maxNumRetry || 0, retryDelay: o.retryDelayMs || 0, maxRetryDelay: o.maxRetryDelayMs || 0 }, c = { onSuccess: (h2, u2, d3, f3) => {
      this.log(`Loaded steering manifest: "${i}"`);
      let g2 = h2.data;
      if (g2.VERSION !== 1) {
        this.log(`Steering VERSION ${g2.VERSION} not supported!`);
        return;
      }
      this.updated = performance.now(), this.timeToLoad = g2.TTL;
      let { "RELOAD-URI": p3, "PATHWAY-CLONES": T2, "PATHWAY-PRIORITY": E4 } = g2;
      if (p3)
        try {
          this.uri = new self.URL(p3, i).href;
        } catch {
          this.enabled = false, this.log(`Failed to parse Steering Manifest RELOAD-URI: ${p3}`);
          return;
        }
      this.scheduleRefresh(this.uri || d3.url), T2 && this.clonePathways(T2);
      let x3 = { steeringManifest: g2, url: i.toString() };
      this.hls.trigger(m2.STEERING_MANIFEST_LOADED, x3), E4 && this.updatePathwayPriority(E4);
    }, onError: (h2, u2, d3, f3) => {
      if (this.log(`Error loading steering manifest: ${h2.code} ${h2.text} (${u2.url})`), this.stopLoad(), h2.code === 410) {
        this.enabled = false, this.log(`Steering manifest ${u2.url} no longer available`);
        return;
      }
      let g2 = this.timeToLoad * 1e3;
      if (h2.code === 429) {
        let p3 = this.loader;
        if (typeof p3?.getResponseHeader == "function") {
          let T2 = p3.getResponseHeader("Retry-After");
          T2 && (g2 = parseFloat(T2) * 1e3);
        }
        this.log(`Steering manifest ${u2.url} rate limited`);
        return;
      }
      this.scheduleRefresh(this.uri || u2.url, g2);
    }, onTimeout: (h2, u2, d3) => {
      this.log(`Timeout loading steering manifest (${u2.url})`), this.scheduleRefresh(this.uri || u2.url);
    } };
    this.log(`Requesting steering manifest: ${i}`), this.loader.load(r2, l2, c);
  }
  scheduleRefresh(e, t = this.timeToLoad * 1e3) {
    this.clearTimeout(), this.reloadTimer = self.setTimeout(() => {
      var s2;
      let i = (s2 = this.hls) == null ? void 0 : s2.media;
      if (i && !i.ended) {
        this.loadSteeringManifest(e);
        return;
      }
      this.scheduleRefresh(e, this.timeToLoad * 1e3);
    }, t);
  }
};
function Nr(n12, e, t, s2) {
  n12 && Object.keys(e).forEach((i) => {
    let r2 = n12.filter((a2) => a2.groupId === i).map((a2) => {
      let o = te2({}, a2);
      return o.details = void 0, o.attrs = new Z2(o.attrs), o.url = o.attrs.URI = Un(a2.url, a2.attrs["STABLE-RENDITION-ID"], "PER-RENDITION-URIS", t), o.groupId = o.attrs["GROUP-ID"] = e[i], o.attrs["PATHWAY-ID"] = s2, o;
    });
    n12.push(...r2);
  });
}
function Un(n12, e, t, s2) {
  let { HOST: i, PARAMS: r2, [t]: a2 } = s2, o;
  e && (o = a2?.[e], o && (n12 = o));
  let l2 = new self.URL(n12);
  return i && !o && (l2.host = i), r2 && Object.keys(r2).sort().forEach((c) => {
    c && l2.searchParams.set(c, r2[c]);
  }), l2.href;
}
var zl = /^age:\s*[\d.]+\s*$/im;
var Qt = class {
  constructor(e) {
    this.xhrSetup = void 0, this.requestTimeout = void 0, this.retryTimeout = void 0, this.retryDelay = void 0, this.config = null, this.callbacks = null, this.context = null, this.loader = null, this.stats = void 0, this.xhrSetup = e && e.xhrSetup || null, this.stats = new je2(), this.retryDelay = 0;
  }
  destroy() {
    this.callbacks = null, this.abortInternal(), this.loader = null, this.config = null, this.context = null, this.xhrSetup = null, this.stats = null;
  }
  abortInternal() {
    let e = this.loader;
    self.clearTimeout(this.requestTimeout), self.clearTimeout(this.retryTimeout), e && (e.onreadystatechange = null, e.onprogress = null, e.readyState !== 4 && (this.stats.aborted = true, e.abort()));
  }
  abort() {
    var e;
    this.abortInternal(), (e = this.callbacks) != null && e.onAbort && this.callbacks.onAbort(this.stats, this.context, this.loader);
  }
  load(e, t, s2) {
    if (this.stats.loading.start)
      throw new Error("Loader can only be used once.");
    this.stats.loading.start = self.performance.now(), this.context = e, this.config = t, this.callbacks = s2, this.loadInternal();
  }
  loadInternal() {
    let { config: e, context: t } = this;
    if (!e || !t)
      return;
    let s2 = this.loader = new self.XMLHttpRequest(), i = this.stats;
    i.loading.first = 0, i.loaded = 0, i.aborted = false;
    let r2 = this.xhrSetup;
    r2 ? Promise.resolve().then(() => {
      if (!this.stats.aborted)
        return r2(s2, t.url);
    }).catch((a2) => (s2.open("GET", t.url, true), r2(s2, t.url))).then(() => {
      this.stats.aborted || this.openAndSendXhr(s2, t, e);
    }).catch((a2) => {
      this.callbacks.onError({ code: s2.status, text: a2.message }, t, s2, i);
    }) : this.openAndSendXhr(s2, t, e);
  }
  openAndSendXhr(e, t, s2) {
    e.readyState || e.open("GET", t.url, true);
    let i = t.headers, { maxTimeToFirstByteMs: r2, maxLoadTimeMs: a2 } = s2.loadPolicy;
    if (i)
      for (let o in i)
        e.setRequestHeader(o, i[o]);
    t.rangeEnd && e.setRequestHeader("Range", "bytes=" + t.rangeStart + "-" + (t.rangeEnd - 1)), e.onreadystatechange = this.readystatechange.bind(this), e.onprogress = this.loadprogress.bind(this), e.responseType = t.responseType, self.clearTimeout(this.requestTimeout), s2.timeout = r2 && F(r2) ? r2 : a2, this.requestTimeout = self.setTimeout(this.loadtimeout.bind(this), s2.timeout), e.send();
  }
  readystatechange() {
    let { context: e, loader: t, stats: s2 } = this;
    if (!e || !t)
      return;
    let i = t.readyState, r2 = this.config;
    if (!s2.aborted && i >= 2 && (s2.loading.first === 0 && (s2.loading.first = Math.max(self.performance.now(), s2.loading.start), r2.timeout !== r2.loadPolicy.maxLoadTimeMs && (self.clearTimeout(this.requestTimeout), r2.timeout = r2.loadPolicy.maxLoadTimeMs, this.requestTimeout = self.setTimeout(this.loadtimeout.bind(this), r2.loadPolicy.maxLoadTimeMs - (s2.loading.first - s2.loading.start)))), i === 4)) {
      self.clearTimeout(this.requestTimeout), t.onreadystatechange = null, t.onprogress = null;
      let a2 = t.status, o = t.responseType !== "text";
      if (a2 >= 200 && a2 < 300 && (o && t.response || t.responseText !== null)) {
        s2.loading.end = Math.max(self.performance.now(), s2.loading.first);
        let l2 = o ? t.response : t.responseText, c = t.responseType === "arraybuffer" ? l2.byteLength : l2.length;
        if (s2.loaded = s2.total = c, s2.bwEstimate = s2.total * 8e3 / (s2.loading.end - s2.loading.first), !this.callbacks)
          return;
        let h2 = this.callbacks.onProgress;
        if (h2 && h2(s2, e, l2, t), !this.callbacks)
          return;
        let u2 = { url: t.responseURL, data: l2, code: a2 };
        this.callbacks.onSuccess(u2, s2, e, t);
      } else {
        let l2 = r2.loadPolicy.errorRetry, c = s2.retry, h2 = { url: e.url, data: void 0, code: a2 };
        Ot(l2, c, false, h2) ? this.retry(l2) : (v2.error(`${a2} while loading ${e.url}`), this.callbacks.onError({ code: a2, text: t.statusText }, e, t, s2));
      }
    }
  }
  loadtimeout() {
    var e;
    let t = (e = this.config) == null ? void 0 : e.loadPolicy.timeoutRetry, s2 = this.stats.retry;
    if (Ot(t, s2, true))
      this.retry(t);
    else {
      var i;
      v2.warn(`timeout while loading ${(i = this.context) == null ? void 0 : i.url}`);
      let r2 = this.callbacks;
      r2 && (this.abortInternal(), r2.onTimeout(this.stats, this.context, this.loader));
    }
  }
  retry(e) {
    let { context: t, stats: s2 } = this;
    this.retryDelay = Fi(e, s2.retry), s2.retry++, v2.warn(`${status ? "HTTP Status " + status : "Timeout"} while loading ${t?.url}, retrying ${s2.retry}/${e.maxNumRetry} in ${this.retryDelay}ms`), this.abortInternal(), this.loader = null, self.clearTimeout(this.retryTimeout), this.retryTimeout = self.setTimeout(this.loadInternal.bind(this), this.retryDelay);
  }
  loadprogress(e) {
    let t = this.stats;
    t.loaded = e.loaded, e.lengthComputable && (t.total = e.total);
  }
  getCacheAge() {
    let e = null;
    if (this.loader && zl.test(this.loader.getAllResponseHeaders())) {
      let t = this.loader.getResponseHeader("age");
      e = t ? parseFloat(t) : null;
    }
    return e;
  }
  getResponseHeader(e) {
    return this.loader && new RegExp(`^${e}:\\s*[\\d.]+\\s*$`, "im").test(this.loader.getAllResponseHeaders()) ? this.loader.getResponseHeader(e) : null;
  }
};
function Xl() {
  if (self.fetch && self.AbortController && self.ReadableStream && self.Request)
    try {
      return new self.ReadableStream({}), true;
    } catch {
    }
  return false;
}
var Ql = /(\d+)-(\d+)\/(\d+)/;
var Jt = class {
  constructor(e) {
    this.fetchSetup = void 0, this.requestTimeout = void 0, this.request = null, this.response = null, this.controller = void 0, this.context = null, this.config = null, this.callbacks = null, this.stats = void 0, this.loader = null, this.fetchSetup = e.fetchSetup || tc, this.controller = new self.AbortController(), this.stats = new je2();
  }
  destroy() {
    this.loader = this.callbacks = this.context = this.config = this.request = null, this.abortInternal(), this.response = null, this.fetchSetup = this.controller = this.stats = null;
  }
  abortInternal() {
    this.controller && !this.stats.loading.end && (this.stats.aborted = true, this.controller.abort());
  }
  abort() {
    var e;
    this.abortInternal(), (e = this.callbacks) != null && e.onAbort && this.callbacks.onAbort(this.stats, this.context, this.response);
  }
  load(e, t, s2) {
    let i = this.stats;
    if (i.loading.start)
      throw new Error("Loader can only be used once.");
    i.loading.start = self.performance.now();
    let r2 = Jl(e, this.controller.signal), a2 = s2.onProgress, o = e.responseType === "arraybuffer", l2 = o ? "byteLength" : "length", { maxTimeToFirstByteMs: c, maxLoadTimeMs: h2 } = t.loadPolicy;
    this.context = e, this.config = t, this.callbacks = s2, this.request = this.fetchSetup(e, r2), self.clearTimeout(this.requestTimeout), t.timeout = c && F(c) ? c : h2, this.requestTimeout = self.setTimeout(() => {
      this.abortInternal(), s2.onTimeout(i, e, this.response);
    }, t.timeout), self.fetch(this.request).then((u2) => {
      this.response = this.loader = u2;
      let d3 = Math.max(self.performance.now(), i.loading.start);
      if (self.clearTimeout(this.requestTimeout), t.timeout = h2, this.requestTimeout = self.setTimeout(() => {
        this.abortInternal(), s2.onTimeout(i, e, this.response);
      }, h2 - (d3 - i.loading.start)), !u2.ok) {
        let { status: f3, statusText: g2 } = u2;
        throw new Ai(g2 || "fetch, bad network response", f3, u2);
      }
      return i.loading.first = d3, i.total = ec(u2.headers) || i.total, a2 && F(t.highWaterMark) ? this.loadProgressively(u2, i, e, t.highWaterMark, a2) : o ? u2.arrayBuffer() : e.responseType === "json" ? u2.json() : u2.text();
    }).then((u2) => {
      let d3 = this.response;
      if (!d3)
        throw new Error("loader destroyed");
      self.clearTimeout(this.requestTimeout), i.loading.end = Math.max(self.performance.now(), i.loading.first);
      let f3 = u2[l2];
      f3 && (i.loaded = i.total = f3);
      let g2 = { url: d3.url, data: u2, code: d3.status };
      a2 && !F(t.highWaterMark) && a2(i, e, u2, d3), s2.onSuccess(g2, i, e, d3);
    }).catch((u2) => {
      if (self.clearTimeout(this.requestTimeout), i.aborted)
        return;
      let d3 = u2 && u2.code || 0, f3 = u2 ? u2.message : null;
      s2.onError({ code: d3, text: f3 }, e, u2 ? u2.details : null, i);
    });
  }
  getCacheAge() {
    let e = null;
    if (this.response) {
      let t = this.response.headers.get("age");
      e = t ? parseFloat(t) : null;
    }
    return e;
  }
  getResponseHeader(e) {
    return this.response ? this.response.headers.get(e) : null;
  }
  loadProgressively(e, t, s2, i = 0, r2) {
    let a2 = new Bt(), o = e.body.getReader(), l2 = () => o.read().then((c) => {
      if (c.done)
        return a2.dataLength && r2(t, s2, a2.flush(), e), Promise.resolve(new ArrayBuffer(0));
      let h2 = c.value, u2 = h2.length;
      return t.loaded += u2, u2 < i || a2.dataLength ? (a2.push(h2), a2.dataLength >= i && r2(t, s2, a2.flush(), e)) : r2(t, s2, h2, e), l2();
    }).catch(() => Promise.reject());
    return l2();
  }
};
function Jl(n12, e) {
  let t = { method: "GET", mode: "cors", credentials: "same-origin", signal: e, headers: new self.Headers(te2({}, n12.headers)) };
  return n12.rangeEnd && t.headers.set("Range", "bytes=" + n12.rangeStart + "-" + String(n12.rangeEnd - 1)), t;
}
function Zl(n12) {
  let e = Ql.exec(n12);
  if (e)
    return parseInt(e[2]) - parseInt(e[1]) + 1;
}
function ec(n12) {
  let e = n12.get("Content-Range");
  if (e) {
    let s2 = Zl(e);
    if (F(s2))
      return s2;
  }
  let t = n12.get("Content-Length");
  if (t)
    return parseInt(t);
}
function tc(n12, e) {
  return new self.Request(n12.url, e);
}
var Ai = class extends Error {
  constructor(e, t, s2) {
    super(e), this.code = void 0, this.details = void 0, this.code = t, this.details = s2;
  }
};
var sc = /\s/;
var ic = { newCue(n12, e, t, s2) {
  let i = [], r2, a2, o, l2, c, h2 = self.VTTCue || self.TextTrackCue;
  for (let d3 = 0; d3 < s2.rows.length; d3++)
    if (r2 = s2.rows[d3], o = true, l2 = 0, c = "", !r2.isEmpty()) {
      var u2;
      for (let p3 = 0; p3 < r2.chars.length; p3++)
        sc.test(r2.chars[p3].uchar) && o ? l2++ : (c += r2.chars[p3].uchar, o = false);
      r2.cueStartTime = e, e === t && (t += 1e-4), l2 >= 16 ? l2-- : l2++;
      let f3 = Cn(c.trim()), g2 = $i(e, t, f3);
      n12 != null && (u2 = n12.cues) != null && u2.getCueById(g2) || (a2 = new h2(e, t, f3), a2.id = g2, a2.line = d3 + 1, a2.align = "left", a2.position = 10 + Math.min(80, Math.floor(l2 * 8 / 32) * 10), i.push(a2));
    }
  return n12 && i.length && (i.sort((d3, f3) => d3.line === "auto" || f3.line === "auto" ? 0 : d3.line > 8 && f3.line > 8 ? f3.line - d3.line : d3.line - f3.line), i.forEach((d3) => tn(n12, d3))), i;
} };
var rc = { maxTimeToFirstByteMs: 8e3, maxLoadTimeMs: 2e4, timeoutRetry: null, errorRetry: null };
var Bn = le3(le3({ autoStartLoad: true, startPosition: -1, defaultAudioCodec: void 0, debug: false, capLevelOnFPSDrop: false, capLevelToPlayerSize: false, ignoreDevicePixelRatio: false, preferManagedMediaSource: true, initialLiveManifestSize: 1, maxBufferLength: 30, backBufferLength: 1 / 0, frontBufferFlushThreshold: 1 / 0, maxBufferSize: 60 * 1e3 * 1e3, maxBufferHole: 0.1, highBufferWatchdogPeriod: 2, nudgeOffset: 0.1, nudgeMaxRetry: 3, maxFragLookUpTolerance: 0.25, liveSyncDurationCount: 3, liveMaxLatencyDurationCount: 1 / 0, liveSyncDuration: void 0, liveMaxLatencyDuration: void 0, maxLiveSyncPlaybackRate: 1, liveDurationInfinity: false, liveBackBufferLength: null, maxMaxBufferLength: 600, enableWorker: true, workerPath: null, enableSoftwareAES: true, startLevel: void 0, startFragPrefetch: false, fpsDroppedMonitoringPeriod: 5e3, fpsDroppedMonitoringThreshold: 0.2, appendErrorMaxRetry: 3, loader: Qt, fLoader: void 0, pLoader: void 0, xhrSetup: void 0, licenseXhrSetup: void 0, licenseResponseCallback: void 0, abrController: Ms, bufferController: oi, capLevelController: pi, errorController: ks, fpsController: Ti, stretchShortVideoTrack: false, maxAudioFramesDrift: 1, forceKeyFrameOnDiscontinuity: true, abrEwmaFastLive: 3, abrEwmaSlowLive: 9, abrEwmaFastVoD: 3, abrEwmaSlowVoD: 9, abrEwmaDefaultEstimate: 5e5, abrEwmaDefaultEstimateMax: 5e6, abrBandWidthFactor: 0.95, abrBandWidthUpFactor: 0.7, abrMaxWithRealBitrate: false, maxStarvationDelay: 4, maxLoadingDelay: 4, minAutoBitrate: 0, emeEnabled: false, widevineLicenseUrl: void 0, drmSystems: {}, drmSystemOptions: {}, requestMediaKeySystemAccessFunc: Gr, testBandwidth: true, progressive: false, lowLatencyMode: true, cmcd: void 0, enableDateRangeMetadataCues: true, enableEmsgMetadataCues: true, enableID3MetadataCues: true, useMediaCapabilities: true, certLoadPolicy: { default: rc }, keyLoadPolicy: { default: { maxTimeToFirstByteMs: 8e3, maxLoadTimeMs: 2e4, timeoutRetry: { maxNumRetry: 1, retryDelayMs: 1e3, maxRetryDelayMs: 2e4, backoff: "linear" }, errorRetry: { maxNumRetry: 8, retryDelayMs: 1e3, maxRetryDelayMs: 2e4, backoff: "linear" } } }, manifestLoadPolicy: { default: { maxTimeToFirstByteMs: 1 / 0, maxLoadTimeMs: 2e4, timeoutRetry: { maxNumRetry: 2, retryDelayMs: 0, maxRetryDelayMs: 0 }, errorRetry: { maxNumRetry: 1, retryDelayMs: 1e3, maxRetryDelayMs: 8e3 } } }, playlistLoadPolicy: { default: { maxTimeToFirstByteMs: 1e4, maxLoadTimeMs: 2e4, timeoutRetry: { maxNumRetry: 2, retryDelayMs: 0, maxRetryDelayMs: 0 }, errorRetry: { maxNumRetry: 2, retryDelayMs: 1e3, maxRetryDelayMs: 8e3 } } }, fragLoadPolicy: { default: { maxTimeToFirstByteMs: 1e4, maxLoadTimeMs: 12e4, timeoutRetry: { maxNumRetry: 4, retryDelayMs: 0, maxRetryDelayMs: 0 }, errorRetry: { maxNumRetry: 6, retryDelayMs: 1e3, maxRetryDelayMs: 8e3 } } }, steeringManifestLoadPolicy: { default: { maxTimeToFirstByteMs: 1e4, maxLoadTimeMs: 2e4, timeoutRetry: { maxNumRetry: 2, retryDelayMs: 0, maxRetryDelayMs: 0 }, errorRetry: { maxNumRetry: 1, retryDelayMs: 1e3, maxRetryDelayMs: 8e3 } } }, manifestLoadingTimeOut: 1e4, manifestLoadingMaxRetry: 1, manifestLoadingRetryDelay: 1e3, manifestLoadingMaxRetryTimeout: 64e3, levelLoadingTimeOut: 1e4, levelLoadingMaxRetry: 4, levelLoadingRetryDelay: 1e3, levelLoadingMaxRetryTimeout: 64e3, fragLoadingTimeOut: 2e4, fragLoadingMaxRetry: 6, fragLoadingRetryDelay: 1e3, fragLoadingMaxRetryTimeout: 64e3 }, nc()), {}, { subtitleStreamController: ii, subtitleTrackController: ni, timelineController: mi, audioStreamController: ti, audioTrackController: si, emeController: zt, cmcdController: Si, contentSteeringController: vi });
function nc() {
  return { cueHandler: ic, enableWebVTT: true, enableIMSC1: true, enableCEA708Captions: true, captionsTextTrack1Label: "English", captionsTextTrack1LanguageCode: "en", captionsTextTrack2Label: "Spanish", captionsTextTrack2LanguageCode: "es", captionsTextTrack3Label: "Unknown CC", captionsTextTrack3LanguageCode: "", captionsTextTrack4Label: "Unknown CC", captionsTextTrack4LanguageCode: "", renderTextTracksNatively: true };
}
function ac(n12, e) {
  if ((e.liveSyncDurationCount || e.liveMaxLatencyDurationCount) && (e.liveSyncDuration || e.liveMaxLatencyDuration))
    throw new Error("Illegal hls.js config: don't mix up liveSyncDurationCount/liveMaxLatencyDurationCount and liveSyncDuration/liveMaxLatencyDuration");
  if (e.liveMaxLatencyDurationCount !== void 0 && (e.liveSyncDurationCount === void 0 || e.liveMaxLatencyDurationCount <= e.liveSyncDurationCount))
    throw new Error('Illegal hls.js config: "liveMaxLatencyDurationCount" must be greater than "liveSyncDurationCount"');
  if (e.liveMaxLatencyDuration !== void 0 && (e.liveSyncDuration === void 0 || e.liveMaxLatencyDuration <= e.liveSyncDuration))
    throw new Error('Illegal hls.js config: "liveMaxLatencyDuration" must be greater than "liveSyncDuration"');
  let t = Li(n12), s2 = ["manifest", "level", "frag"], i = ["TimeOut", "MaxRetry", "RetryDelay", "MaxRetryTimeout"];
  return s2.forEach((r2) => {
    let a2 = `${r2 === "level" ? "playlist" : r2}LoadPolicy`, o = e[a2] === void 0, l2 = [];
    i.forEach((c) => {
      let h2 = `${r2}Loading${c}`, u2 = e[h2];
      if (u2 !== void 0 && o) {
        l2.push(h2);
        let d3 = t[a2].default;
        switch (e[a2] = { default: d3 }, c) {
          case "TimeOut":
            d3.maxLoadTimeMs = u2, d3.maxTimeToFirstByteMs = u2;
            break;
          case "MaxRetry":
            d3.errorRetry.maxNumRetry = u2, d3.timeoutRetry.maxNumRetry = u2;
            break;
          case "RetryDelay":
            d3.errorRetry.retryDelayMs = u2, d3.timeoutRetry.retryDelayMs = u2;
            break;
          case "MaxRetryTimeout":
            d3.errorRetry.maxRetryDelayMs = u2, d3.timeoutRetry.maxRetryDelayMs = u2;
            break;
        }
      }
    }), l2.length && v2.warn(`hls.js config: "${l2.join('", "')}" setting(s) are deprecated, use "${a2}": ${JSON.stringify(e[a2])}`);
  }), le3(le3({}, t), e);
}
function Li(n12) {
  return n12 && typeof n12 == "object" ? Array.isArray(n12) ? n12.map(Li) : Object.keys(n12).reduce((e, t) => (e[t] = Li(n12[t]), e), {}) : n12;
}
function oc(n12) {
  let e = n12.loader;
  e !== Jt && e !== Qt ? (v2.log("[config]: Custom loader detected, cannot enable progressive streaming"), n12.progressive = false) : Xl() && (n12.loader = Jt, n12.progressive = true, n12.enableSoftwareAES = true, v2.log("[config]: Progressive streaming enabled, using FetchLoader"));
}
var ys;
var Ri = class extends nt {
  constructor(e, t) {
    super(e, "[level-controller]"), this._levels = [], this._firstLevel = -1, this._maxAutoLevel = -1, this._startLevel = void 0, this.currentLevel = null, this.currentLevelIndex = -1, this.manualLevelIndex = -1, this.steering = void 0, this.onParsedComplete = void 0, this.steering = t, this._registerListeners();
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.on(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.on(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.on(m2.ERROR, this.onError, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_LOADED, this.onManifestLoaded, this), e.off(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.off(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this), e.off(m2.ERROR, this.onError, this);
  }
  destroy() {
    this._unregisterListeners(), this.steering = null, this.resetLevels(), super.destroy();
  }
  stopLoad() {
    this._levels.forEach((t) => {
      t.loadError = 0, t.fragmentError = 0;
    }), super.stopLoad();
  }
  resetLevels() {
    this._startLevel = void 0, this.manualLevelIndex = -1, this.currentLevelIndex = -1, this.currentLevel = null, this._levels = [], this._maxAutoLevel = -1;
  }
  onManifestLoading(e, t) {
    this.resetLevels();
  }
  onManifestLoaded(e, t) {
    let s2 = this.hls.config.preferManagedMediaSource, i = [], r2 = {}, a2 = {}, o = false, l2 = false, c = false;
    t.levels.forEach((h2) => {
      var u2, d3;
      let f3 = h2.attrs, { audioCodec: g2, videoCodec: p3 } = h2;
      ((u2 = g2) == null ? void 0 : u2.indexOf("mp4a.40.34")) !== -1 && (ys || (ys = /chrome|firefox/i.test(navigator.userAgent)), ys && (h2.audioCodec = g2 = void 0)), g2 && (h2.audioCodec = g2 = Pt(g2, s2)), ((d3 = p3) == null ? void 0 : d3.indexOf("avc1")) === 0 && (p3 = h2.videoCodec = Na2(p3));
      let { width: T2, height: E4, unknownCodecs: x3 } = h2;
      if (o || (o = !!(T2 && E4)), l2 || (l2 = !!p3), c || (c = !!g2), x3 != null && x3.length || g2 && !ns(g2, "audio", s2) || p3 && !ns(p3, "video", s2))
        return;
      let { CODECS: y3, "FRAME-RATE": R, "HDCP-LEVEL": S2, "PATHWAY-ID": b, RESOLUTION: L, "VIDEO-RANGE": C2 } = f3, I = `${`${b || "."}-`}${h2.bitrate}-${L}-${R}-${y3}-${C2}-${S2}`;
      if (r2[I])
        if (r2[I].uri !== h2.url && !h2.attrs["PATHWAY-ID"]) {
          let w = a2[I] += 1;
          h2.attrs["PATHWAY-ID"] = new Array(w + 1).join(".");
          let K = new Pe3(h2);
          r2[I] = K, i.push(K);
        } else
          r2[I].addGroupId("audio", f3.AUDIO), r2[I].addGroupId("text", f3.SUBTITLES);
      else {
        let w = new Pe3(h2);
        r2[I] = w, a2[I] = 1, i.push(w);
      }
    }), this.filterAndSortMediaOptions(i, t, o, l2, c);
  }
  filterAndSortMediaOptions(e, t, s2, i, r2) {
    let a2 = [], o = [], l2 = e;
    if ((s2 || i) && r2 && (l2 = l2.filter(({ videoCodec: g2, videoRange: p3, width: T2, height: E4 }) => (!!g2 || !!(T2 && E4)) && qa(p3))), l2.length === 0) {
      Promise.resolve().then(() => {
        if (this.hls) {
          t.levels.length && this.warn(`One or more CODECS in variant not supported: ${JSON.stringify(t.levels[0].attrs)}`);
          let g2 = new Error("no level with compatible codecs found in manifest");
          this.hls.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.MANIFEST_INCOMPATIBLE_CODECS_ERROR, fatal: true, url: t.url, error: g2, reason: g2.message });
        }
      });
      return;
    }
    if (t.audioTracks) {
      let { preferManagedMediaSource: g2 } = this.hls.config;
      a2 = t.audioTracks.filter((p3) => !p3.audioCodec || ns(p3.audioCodec, "audio", g2)), Ur(a2);
    }
    t.subtitles && (o = t.subtitles, Ur(o));
    let c = l2.slice(0);
    l2.sort((g2, p3) => {
      if (g2.attrs["HDCP-LEVEL"] !== p3.attrs["HDCP-LEVEL"])
        return (g2.attrs["HDCP-LEVEL"] || "") > (p3.attrs["HDCP-LEVEL"] || "") ? 1 : -1;
      if (s2 && g2.height !== p3.height)
        return g2.height - p3.height;
      if (g2.frameRate !== p3.frameRate)
        return g2.frameRate - p3.frameRate;
      if (g2.videoRange !== p3.videoRange)
        return kt.indexOf(g2.videoRange) - kt.indexOf(p3.videoRange);
      if (g2.videoCodec !== p3.videoCodec) {
        let T2 = Xi(g2.videoCodec), E4 = Xi(p3.videoCodec);
        if (T2 !== E4)
          return E4 - T2;
      }
      if (g2.uri === p3.uri && g2.codecSet !== p3.codecSet) {
        let T2 = _t(g2.codecSet), E4 = _t(p3.codecSet);
        if (T2 !== E4)
          return E4 - T2;
      }
      return g2.averageBitrate !== p3.averageBitrate ? g2.averageBitrate - p3.averageBitrate : 0;
    });
    let h2 = c[0];
    if (this.steering && (l2 = this.steering.filterParsedLevels(l2), l2.length !== c.length)) {
      for (let g2 = 0; g2 < c.length; g2++)
        if (c[g2].pathwayId === l2[0].pathwayId) {
          h2 = c[g2];
          break;
        }
    }
    this._levels = l2;
    for (let g2 = 0; g2 < l2.length; g2++)
      if (l2[g2] === h2) {
        var u2;
        this._firstLevel = g2;
        let p3 = h2.bitrate, T2 = this.hls.bandwidthEstimate;
        if (this.log(`manifest loaded, ${l2.length} level(s) found, first bitrate: ${p3}`), ((u2 = this.hls.userConfig) == null ? void 0 : u2.abrEwmaDefaultEstimate) === void 0) {
          let E4 = Math.min(p3, this.hls.config.abrEwmaDefaultEstimateMax);
          E4 > T2 && T2 === Bn.abrEwmaDefaultEstimate && (this.hls.bandwidthEstimate = E4);
        }
        break;
      }
    let d3 = r2 && !i, f3 = { levels: l2, audioTracks: a2, subtitleTracks: o, sessionData: t.sessionData, sessionKeys: t.sessionKeys, firstLevel: this._firstLevel, stats: t.stats, audio: r2, video: i, altAudio: !d3 && a2.some((g2) => !!g2.url) };
    this.hls.trigger(m2.MANIFEST_PARSED, f3), (this.hls.config.autoStartLoad || this.hls.forceStartLoad) && this.hls.startLoad(this.hls.config.startPosition);
  }
  get levels() {
    return this._levels.length === 0 ? null : this._levels;
  }
  get level() {
    return this.currentLevelIndex;
  }
  set level(e) {
    let t = this._levels;
    if (t.length === 0)
      return;
    if (e < 0 || e >= t.length) {
      let h2 = new Error("invalid level idx"), u2 = e < 0;
      if (this.hls.trigger(m2.ERROR, { type: B2.OTHER_ERROR, details: A2.LEVEL_SWITCH_ERROR, level: e, fatal: u2, error: h2, reason: h2.message }), u2)
        return;
      e = Math.min(e, t.length - 1);
    }
    let s2 = this.currentLevelIndex, i = this.currentLevel, r2 = i ? i.attrs["PATHWAY-ID"] : void 0, a2 = t[e], o = a2.attrs["PATHWAY-ID"];
    if (this.currentLevelIndex = e, this.currentLevel = a2, s2 === e && a2.details && i && r2 === o)
      return;
    this.log(`Switching to level ${e} (${a2.height ? a2.height + "p " : ""}${a2.videoRange ? a2.videoRange + " " : ""}${a2.codecSet ? a2.codecSet + " " : ""}@${a2.bitrate})${o ? " with Pathway " + o : ""} from level ${s2}${r2 ? " with Pathway " + r2 : ""}`);
    let l2 = { level: e, attrs: a2.attrs, details: a2.details, bitrate: a2.bitrate, averageBitrate: a2.averageBitrate, maxBitrate: a2.maxBitrate, realBitrate: a2.realBitrate, width: a2.width, height: a2.height, codecSet: a2.codecSet, audioCodec: a2.audioCodec, videoCodec: a2.videoCodec, audioGroups: a2.audioGroups, subtitleGroups: a2.subtitleGroups, loaded: a2.loaded, loadError: a2.loadError, fragmentError: a2.fragmentError, name: a2.name, id: a2.id, uri: a2.uri, url: a2.url, urlId: 0, audioGroupIds: a2.audioGroupIds, textGroupIds: a2.textGroupIds };
    this.hls.trigger(m2.LEVEL_SWITCHING, l2);
    let c = a2.details;
    if (!c || c.live) {
      let h2 = this.switchParams(a2.uri, i?.details, c);
      this.loadPlaylist(h2);
    }
  }
  get manualLevel() {
    return this.manualLevelIndex;
  }
  set manualLevel(e) {
    this.manualLevelIndex = e, this._startLevel === void 0 && (this._startLevel = e), e !== -1 && (this.level = e);
  }
  get firstLevel() {
    return this._firstLevel;
  }
  set firstLevel(e) {
    this._firstLevel = e;
  }
  get startLevel() {
    if (this._startLevel === void 0) {
      let e = this.hls.config.startLevel;
      return e !== void 0 ? e : this.hls.firstAutoLevel;
    }
    return this._startLevel;
  }
  set startLevel(e) {
    this._startLevel = e;
  }
  onError(e, t) {
    t.fatal || !t.context || t.context.type === W3.LEVEL && t.context.level === this.level && this.checkRetry(t);
  }
  onFragBuffered(e, { frag: t }) {
    if (t !== void 0 && t.type === N2.MAIN) {
      let s2 = t.elementaryStreams;
      if (!Object.keys(s2).some((r2) => !!s2[r2]))
        return;
      let i = this._levels[t.level];
      i != null && i.loadError && (this.log(`Resetting level error count of ${i.loadError} on frag buffered`), i.loadError = 0);
    }
  }
  onLevelLoaded(e, t) {
    var s2;
    let { level: i, details: r2 } = t, a2 = this._levels[i];
    if (!a2) {
      var o;
      this.warn(`Invalid level index ${i}`), (o = t.deliveryDirectives) != null && o.skip && (r2.deltaUpdateFailed = true);
      return;
    }
    i === this.currentLevelIndex ? (a2.fragmentError === 0 && (a2.loadError = 0), this.playlistLoaded(i, t, a2.details)) : (s2 = t.deliveryDirectives) != null && s2.skip && (r2.deltaUpdateFailed = true);
  }
  loadPlaylist(e) {
    super.loadPlaylist();
    let t = this.currentLevelIndex, s2 = this.currentLevel;
    if (s2 && this.shouldLoadPlaylist(s2)) {
      let i = s2.uri;
      if (e)
        try {
          i = e.addDirectives(i);
        } catch (a2) {
          this.warn(`Could not construct new URL with HLS Delivery Directives: ${a2}`);
        }
      let r2 = s2.attrs["PATHWAY-ID"];
      this.log(`Loading level index ${t}${e?.msn !== void 0 ? " at sn " + e.msn + " part " + e.part : ""} with${r2 ? " Pathway " + r2 : ""} ${i}`), this.clearTimer(), this.hls.trigger(m2.LEVEL_LOADING, { url: i, level: t, pathwayId: s2.attrs["PATHWAY-ID"], id: 0, deliveryDirectives: e || null });
    }
  }
  get nextLoadLevel() {
    return this.manualLevelIndex !== -1 ? this.manualLevelIndex : this.hls.nextAutoLevel;
  }
  set nextLoadLevel(e) {
    this.level = e, this.manualLevelIndex === -1 && (this.hls.nextAutoLevel = e);
  }
  removeLevel(e) {
    var t;
    let s2 = this._levels.filter((i, r2) => r2 !== e ? true : (this.steering && this.steering.removeLevel(i), i === this.currentLevel && (this.currentLevel = null, this.currentLevelIndex = -1, i.details && i.details.fragments.forEach((a2) => a2.level = -1)), false));
    an2(s2), this._levels = s2, this.currentLevelIndex > -1 && (t = this.currentLevel) != null && t.details && (this.currentLevelIndex = this.currentLevel.details.fragments[0].level), this.hls.trigger(m2.LEVELS_UPDATED, { levels: s2 });
  }
  onLevelsUpdated(e, { levels: t }) {
    this._levels = t;
  }
  checkMaxAutoUpdated() {
    let { autoLevelCapping: e, maxAutoLevel: t, maxHdcpLevel: s2 } = this.hls;
    this._maxAutoLevel !== t && (this._maxAutoLevel = t, this.hls.trigger(m2.MAX_AUTO_LEVEL_UPDATED, { autoLevelCapping: e, levels: this.levels, maxAutoLevel: t, minAutoLevel: this.hls.minAutoLevel, maxHdcpLevel: s2 }));
  }
};
function Ur(n12) {
  let e = {};
  n12.forEach((t) => {
    let s2 = t.groupId || "";
    t.id = e[s2] = e[s2] || 0, e[s2]++;
  });
}
var Ii = class {
  constructor(e) {
    this.config = void 0, this.keyUriToKeyInfo = {}, this.emeController = null, this.config = e;
  }
  abort(e) {
    for (let s2 in this.keyUriToKeyInfo) {
      let i = this.keyUriToKeyInfo[s2].loader;
      if (i) {
        var t;
        if (e && e !== ((t = i.context) == null ? void 0 : t.frag.type))
          return;
        i.abort();
      }
    }
  }
  detach() {
    for (let e in this.keyUriToKeyInfo) {
      let t = this.keyUriToKeyInfo[e];
      (t.mediaKeySessionContext || t.decryptdata.isCommonEncryption) && delete this.keyUriToKeyInfo[e];
    }
  }
  destroy() {
    this.detach();
    for (let e in this.keyUriToKeyInfo) {
      let t = this.keyUriToKeyInfo[e].loader;
      t && t.destroy();
    }
    this.keyUriToKeyInfo = {};
  }
  createKeyLoadError(e, t = A2.KEY_LOAD_ERROR, s2, i, r2) {
    return new ye3({ type: B2.NETWORK_ERROR, details: t, fatal: false, frag: e, response: r2, error: s2, networkDetails: i });
  }
  loadClear(e, t) {
    if (this.emeController && this.config.emeEnabled) {
      let { sn: s2, cc: i } = e;
      for (let r2 = 0; r2 < t.length; r2++) {
        let a2 = t[r2];
        if (i <= a2.cc && (s2 === "initSegment" || a2.sn === "initSegment" || s2 < a2.sn)) {
          this.emeController.selectKeySystemFormat(a2).then((o) => {
            a2.setKeyFormat(o);
          });
          break;
        }
      }
    }
  }
  load(e) {
    return !e.decryptdata && e.encrypted && this.emeController ? this.emeController.selectKeySystemFormat(e).then((t) => this.loadInternal(e, t)) : this.loadInternal(e);
  }
  loadInternal(e, t) {
    var s2, i;
    t && e.setKeyFormat(t);
    let r2 = e.decryptdata;
    if (!r2) {
      let c = new Error(t ? `Expected frag.decryptdata to be defined after setting format ${t}` : "Missing decryption data on fragment in onKeyLoading");
      return Promise.reject(this.createKeyLoadError(e, A2.KEY_LOAD_ERROR, c));
    }
    let a2 = r2.uri;
    if (!a2)
      return Promise.reject(this.createKeyLoadError(e, A2.KEY_LOAD_ERROR, new Error(`Invalid key URI: "${a2}"`)));
    let o = this.keyUriToKeyInfo[a2];
    if ((s2 = o) != null && s2.decryptdata.key)
      return r2.key = o.decryptdata.key, Promise.resolve({ frag: e, keyInfo: o });
    if ((i = o) != null && i.keyLoadPromise) {
      var l2;
      switch ((l2 = o.mediaKeySessionContext) == null ? void 0 : l2.keyStatus) {
        case void 0:
        case "status-pending":
        case "usable":
        case "usable-in-future":
          return o.keyLoadPromise.then((c) => (r2.key = c.keyInfo.decryptdata.key, { frag: e, keyInfo: o }));
      }
    }
    switch (o = this.keyUriToKeyInfo[a2] = { decryptdata: r2, keyLoadPromise: null, loader: null, mediaKeySessionContext: null }, r2.method) {
      case "ISO-23001-7":
      case "SAMPLE-AES":
      case "SAMPLE-AES-CENC":
      case "SAMPLE-AES-CTR":
        return r2.keyFormat === "identity" ? this.loadKeyHTTP(o, e) : this.loadKeyEME(o, e);
      case "AES-128":
        return this.loadKeyHTTP(o, e);
      default:
        return Promise.reject(this.createKeyLoadError(e, A2.KEY_LOAD_ERROR, new Error(`Key supplied with unsupported METHOD: "${r2.method}"`)));
    }
  }
  loadKeyEME(e, t) {
    let s2 = { frag: t, keyInfo: e };
    if (this.emeController && this.config.emeEnabled) {
      let i = this.emeController.loadKey(s2);
      if (i)
        return (e.keyLoadPromise = i.then((r2) => (e.mediaKeySessionContext = r2, s2))).catch((r2) => {
          throw e.keyLoadPromise = null, r2;
        });
    }
    return Promise.resolve(s2);
  }
  loadKeyHTTP(e, t) {
    let s2 = this.config, i = s2.loader, r2 = new i(s2);
    return t.keyLoader = e.loader = r2, e.keyLoadPromise = new Promise((a2, o) => {
      let l2 = { keyInfo: e, frag: t, responseType: "arraybuffer", url: e.decryptdata.uri }, c = s2.keyLoadPolicy.default, h2 = { loadPolicy: c, timeout: c.maxLoadTimeMs, maxRetry: 0, retryDelay: 0, maxRetryDelay: 0 }, u2 = { onSuccess: (d3, f3, g2, p3) => {
        let { frag: T2, keyInfo: E4, url: x3 } = g2;
        if (!T2.decryptdata || E4 !== this.keyUriToKeyInfo[x3])
          return o(this.createKeyLoadError(T2, A2.KEY_LOAD_ERROR, new Error("after key load, decryptdata unset or changed"), p3));
        E4.decryptdata.key = T2.decryptdata.key = new Uint8Array(d3.data), T2.keyLoader = null, E4.loader = null, a2({ frag: T2, keyInfo: E4 });
      }, onError: (d3, f3, g2, p3) => {
        this.resetLoader(f3), o(this.createKeyLoadError(t, A2.KEY_LOAD_ERROR, new Error(`HTTP Error ${d3.code} loading key ${d3.text}`), g2, le3({ url: l2.url, data: void 0 }, d3)));
      }, onTimeout: (d3, f3, g2) => {
        this.resetLoader(f3), o(this.createKeyLoadError(t, A2.KEY_LOAD_TIMEOUT, new Error("key loading timed out"), g2));
      }, onAbort: (d3, f3, g2) => {
        this.resetLoader(f3), o(this.createKeyLoadError(t, A2.INTERNAL_ABORTED, new Error("key loading aborted"), g2));
      } };
      r2.load(l2, h2, u2);
    });
  }
  resetLoader(e) {
    let { frag: t, keyInfo: s2, url: i } = e, r2 = s2.loader;
    t.keyLoader === r2 && (t.keyLoader = null, s2.loader = null), delete this.keyUriToKeyInfo[i], r2 && r2.destroy();
  }
};
function $n() {
  return self.SourceBuffer || self.WebKitSourceBuffer;
}
function Gn() {
  if (!Ue2())
    return false;
  let e = $n();
  return !e || e.prototype && typeof e.prototype.appendBuffer == "function" && typeof e.prototype.remove == "function";
}
function lc() {
  if (!Gn())
    return false;
  let n12 = Ue2();
  return typeof n12?.isTypeSupported == "function" && (["avc1.42E01E,mp4a.40.2", "av01.0.01M.08", "vp09.00.50.08"].some((e) => n12.isTypeSupported(rt(e, "video"))) || ["mp4a.40.2", "fLaC"].some((e) => n12.isTypeSupported(rt(e, "audio"))));
}
function cc() {
  var n12;
  let e = $n();
  return typeof (e == null || (n12 = e.prototype) == null ? void 0 : n12.changeType) == "function";
}
var hc = 250;
var It = 2;
var uc = 0.1;
var dc = 0.05;
var bi = class {
  constructor(e, t, s2, i) {
    this.config = void 0, this.media = null, this.fragmentTracker = void 0, this.hls = void 0, this.nudgeRetry = 0, this.stallReported = false, this.stalled = null, this.moved = false, this.seeking = false, this.config = e, this.media = t, this.fragmentTracker = s2, this.hls = i;
  }
  destroy() {
    this.media = null, this.hls = this.fragmentTracker = null;
  }
  poll(e, t) {
    let { config: s2, media: i, stalled: r2 } = this;
    if (i === null)
      return;
    let { currentTime: a2, seeking: o } = i, l2 = this.seeking && !o, c = !this.seeking && o;
    if (this.seeking = o, a2 !== e) {
      if (this.moved = true, o || (this.nudgeRetry = 0), r2 !== null) {
        if (this.stallReported) {
          let T2 = self.performance.now() - r2;
          v2.warn(`playback not stuck anymore @${a2}, after ${Math.round(T2)}ms`), this.stallReported = false;
        }
        this.stalled = null;
      }
      return;
    }
    if (c || l2) {
      this.stalled = null;
      return;
    }
    if (i.paused && !o || i.ended || i.playbackRate === 0 || !Q.getBuffered(i).length) {
      this.nudgeRetry = 0;
      return;
    }
    let h2 = Q.bufferInfo(i, a2, 0), u2 = h2.nextStart || 0;
    if (o) {
      let T2 = h2.len > It, E4 = !u2 || t && t.start <= a2 || u2 - a2 > It && !this.fragmentTracker.getPartialFragment(a2);
      if (T2 || E4)
        return;
      this.moved = false;
    }
    if (!this.moved && this.stalled !== null) {
      var d3;
      if (!(h2.len > 0) && !u2)
        return;
      let E4 = Math.max(u2, h2.start || 0) - a2, x3 = this.hls.levels ? this.hls.levels[this.hls.currentLevel] : null, R = (x3 == null || (d3 = x3.details) == null ? void 0 : d3.live) ? x3.details.targetduration * 2 : It, S2 = this.fragmentTracker.getPartialFragment(a2);
      if (E4 > 0 && (E4 <= R || S2)) {
        i.paused || this._trySkipBufferHole(S2);
        return;
      }
    }
    let f3 = self.performance.now();
    if (r2 === null) {
      this.stalled = f3;
      return;
    }
    let g2 = f3 - r2;
    if (!o && g2 >= hc && (this._reportStall(h2), !this.media))
      return;
    let p3 = Q.bufferInfo(i, a2, s2.maxBufferHole);
    this._tryFixBufferStall(p3, g2);
  }
  _tryFixBufferStall(e, t) {
    let { config: s2, fragmentTracker: i, media: r2 } = this;
    if (r2 === null)
      return;
    let a2 = r2.currentTime, o = i.getPartialFragment(a2);
    o && (this._trySkipBufferHole(o) || !this.media) || (e.len > s2.maxBufferHole || e.nextStart && e.nextStart - a2 < s2.maxBufferHole) && t > s2.highBufferWatchdogPeriod * 1e3 && (v2.warn("Trying to nudge playhead over buffer-hole"), this.stalled = null, this._tryNudgeBuffer());
  }
  _reportStall(e) {
    let { hls: t, media: s2, stallReported: i } = this;
    if (!i && s2) {
      this.stallReported = true;
      let r2 = new Error(`Playback stalling at @${s2.currentTime} due to low buffer (${JSON.stringify(e)})`);
      v2.warn(r2.message), t.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_STALLED_ERROR, fatal: false, error: r2, buffer: e.len });
    }
  }
  _trySkipBufferHole(e) {
    let { config: t, hls: s2, media: i } = this;
    if (i === null)
      return 0;
    let r2 = i.currentTime, a2 = Q.bufferInfo(i, r2, 0), o = r2 < a2.start ? a2.start : a2.nextStart;
    if (o) {
      let l2 = a2.len <= t.maxBufferHole, c = a2.len > 0 && a2.len < 1 && i.readyState < 3, h2 = o - r2;
      if (h2 > 0 && (l2 || c)) {
        if (h2 > t.maxBufferHole) {
          let { fragmentTracker: d3 } = this, f3 = false;
          if (r2 === 0) {
            let g2 = d3.getAppendedFrag(0, N2.MAIN);
            g2 && o < g2.end && (f3 = true);
          }
          if (!f3) {
            let g2 = e || d3.getAppendedFrag(r2, N2.MAIN);
            if (g2) {
              let p3 = false, T2 = g2.end;
              for (; T2 < o; ) {
                let E4 = d3.getPartialFragment(T2);
                if (E4)
                  T2 += E4.duration;
                else {
                  p3 = true;
                  break;
                }
              }
              if (p3)
                return 0;
            }
          }
        }
        let u2 = Math.max(o + dc, r2 + uc);
        if (v2.warn(`skipping hole, adjusting currentTime from ${r2} to ${u2}`), this.moved = true, this.stalled = null, i.currentTime = u2, e && !e.gap) {
          let d3 = new Error(`fragment loaded with buffer holes, seeking from ${r2} to ${u2}`);
          s2.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_SEEK_OVER_HOLE, fatal: false, error: d3, reason: d3.message, frag: e });
        }
        return u2;
      }
    }
    return 0;
  }
  _tryNudgeBuffer() {
    let { config: e, hls: t, media: s2, nudgeRetry: i } = this;
    if (s2 === null)
      return;
    let r2 = s2.currentTime;
    if (this.nudgeRetry++, i < e.nudgeMaxRetry) {
      let a2 = r2 + (i + 1) * e.nudgeOffset, o = new Error(`Nudging 'currentTime' from ${r2} to ${a2}`);
      v2.warn(o.message), s2.currentTime = a2, t.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_NUDGE_ON_STALL, error: o, fatal: false });
    } else {
      let a2 = new Error(`Playhead still not moving while enough data buffered @${r2} after ${e.nudgeMaxRetry} nudges`);
      v2.error(a2.message), t.trigger(m2.ERROR, { type: B2.MEDIA_ERROR, details: A2.BUFFER_STALLED_ERROR, error: a2, fatal: true });
    }
  }
};
var fc = 100;
var Di = class extends lt {
  constructor(e, t, s2) {
    super(e, t, s2, "[stream-controller]", N2.MAIN), this.audioCodecSwap = false, this.gapController = null, this.level = -1, this._forceStartLoad = false, this.altAudio = false, this.audioOnly = false, this.fragPlaying = null, this.onvplaying = null, this.onvseeked = null, this.fragLastKbps = 0, this.couldBacktrack = false, this.backtrackFragment = null, this.audioCodecSwitch = false, this.videoBuffer = null, this._registerListeners();
  }
  _registerListeners() {
    let { hls: e } = this;
    e.on(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.on(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.on(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.on(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.on(m2.LEVEL_LOADING, this.onLevelLoading, this), e.on(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.on(m2.FRAG_LOAD_EMERGENCY_ABORTED, this.onFragLoadEmergencyAborted, this), e.on(m2.ERROR, this.onError, this), e.on(m2.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this), e.on(m2.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this), e.on(m2.BUFFER_CREATED, this.onBufferCreated, this), e.on(m2.BUFFER_FLUSHED, this.onBufferFlushed, this), e.on(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.on(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  _unregisterListeners() {
    let { hls: e } = this;
    e.off(m2.MEDIA_ATTACHED, this.onMediaAttached, this), e.off(m2.MEDIA_DETACHING, this.onMediaDetaching, this), e.off(m2.MANIFEST_LOADING, this.onManifestLoading, this), e.off(m2.MANIFEST_PARSED, this.onManifestParsed, this), e.off(m2.LEVEL_LOADED, this.onLevelLoaded, this), e.off(m2.FRAG_LOAD_EMERGENCY_ABORTED, this.onFragLoadEmergencyAborted, this), e.off(m2.ERROR, this.onError, this), e.off(m2.AUDIO_TRACK_SWITCHING, this.onAudioTrackSwitching, this), e.off(m2.AUDIO_TRACK_SWITCHED, this.onAudioTrackSwitched, this), e.off(m2.BUFFER_CREATED, this.onBufferCreated, this), e.off(m2.BUFFER_FLUSHED, this.onBufferFlushed, this), e.off(m2.LEVELS_UPDATED, this.onLevelsUpdated, this), e.off(m2.FRAG_BUFFERED, this.onFragBuffered, this);
  }
  onHandlerDestroying() {
    this._unregisterListeners(), super.onHandlerDestroying();
  }
  startLoad(e) {
    if (this.levels) {
      let { lastCurrentTime: t, hls: s2 } = this;
      if (this.stopLoad(), this.setInterval(fc), this.level = -1, !this.startFragRequested) {
        let i = s2.startLevel;
        i === -1 && (s2.config.testBandwidth && this.levels.length > 1 ? (i = 0, this.bitrateTest = true) : i = s2.firstAutoLevel), s2.nextLoadLevel = i, this.level = s2.loadLevel, this.loadedmetadata = false;
      }
      t > 0 && e === -1 && (this.log(`Override startPosition with lastCurrentTime @${t.toFixed(3)}`), e = t), this.state = D.IDLE, this.nextLoadPosition = this.startPosition = this.lastCurrentTime = e, this.tick();
    } else
      this._forceStartLoad = true, this.state = D.STOPPED;
  }
  stopLoad() {
    this._forceStartLoad = false, super.stopLoad();
  }
  doTick() {
    switch (this.state) {
      case D.WAITING_LEVEL: {
        let { levels: t, level: s2 } = this, i = t?.[s2], r2 = i?.details;
        if (r2 && (!r2.live || this.levelLastLoaded === i)) {
          if (this.waitForCdnTuneIn(r2))
            break;
          this.state = D.IDLE;
          break;
        } else if (this.hls.nextLoadLevel !== this.level) {
          this.state = D.IDLE;
          break;
        }
        break;
      }
      case D.FRAG_LOADING_WAITING_RETRY:
        {
          var e;
          let t = self.performance.now(), s2 = this.retryDate;
          if (!s2 || t >= s2 || (e = this.media) != null && e.seeking) {
            let { levels: i, level: r2 } = this, a2 = i?.[r2];
            this.resetStartWhenNotLoaded(a2 || null), this.state = D.IDLE;
          }
        }
        break;
    }
    this.state === D.IDLE && this.doTickIdle(), this.onTickEnd();
  }
  onTickEnd() {
    super.onTickEnd(), this.checkBuffer(), this.checkFragmentChanged();
  }
  doTickIdle() {
    let { hls: e, levelLastLoaded: t, levels: s2, media: i } = this;
    if (t === null || !i && (this.startFragRequested || !e.config.startFragPrefetch) || this.altAudio && this.audioOnly)
      return;
    let r2 = e.nextLoadLevel;
    if (!(s2 != null && s2[r2]))
      return;
    let a2 = s2[r2], o = this.getMainFwdBufferInfo();
    if (o === null)
      return;
    let l2 = this.getLevelDetails();
    if (l2 && this._streamEnded(o, l2)) {
      let p3 = {};
      this.altAudio && (p3.type = "video"), this.hls.trigger(m2.BUFFER_EOS, p3), this.state = D.ENDED;
      return;
    }
    e.loadLevel !== r2 && e.manualLevel === -1 && this.log(`Adapting to level ${r2} from level ${this.level}`), this.level = e.nextLoadLevel = r2;
    let c = a2.details;
    if (!c || this.state === D.WAITING_LEVEL || c.live && this.levelLastLoaded !== a2) {
      this.level = r2, this.state = D.WAITING_LEVEL;
      return;
    }
    let h2 = o.len, u2 = this.getMaxBufferLength(a2.maxBitrate);
    if (h2 >= u2)
      return;
    this.backtrackFragment && this.backtrackFragment.start > o.end && (this.backtrackFragment = null);
    let d3 = this.backtrackFragment ? this.backtrackFragment.start : o.end, f3 = this.getNextFragment(d3, c);
    if (this.couldBacktrack && !this.fragPrevious && f3 && f3.sn !== "initSegment" && this.fragmentTracker.getState(f3) !== oe2.OK) {
      var g2;
      let T2 = ((g2 = this.backtrackFragment) != null ? g2 : f3).sn - c.startSN, E4 = c.fragments[T2 - 1];
      E4 && f3.cc === E4.cc && (f3 = E4, this.fragmentTracker.removeFragment(E4));
    } else
      this.backtrackFragment && o.len && (this.backtrackFragment = null);
    if (f3 && this.isLoopLoading(f3, d3)) {
      if (!f3.gap) {
        let T2 = this.audioOnly && !this.altAudio ? z2.AUDIO : z2.VIDEO, E4 = (T2 === z2.VIDEO ? this.videoBuffer : this.mediaBuffer) || this.media;
        E4 && this.afterBufferFlushed(E4, T2, N2.MAIN);
      }
      f3 = this.getNextFragmentLoopLoading(f3, c, o, N2.MAIN, u2);
    }
    f3 && (f3.initSegment && !f3.initSegment.data && !this.bitrateTest && (f3 = f3.initSegment), this.loadFragment(f3, a2, d3));
  }
  loadFragment(e, t, s2) {
    let i = this.fragmentTracker.getState(e);
    this.fragCurrent = e, i === oe2.NOT_LOADED || i === oe2.PARTIAL ? e.sn === "initSegment" ? this._loadInitSegment(e, t) : this.bitrateTest ? (this.log(`Fragment ${e.sn} of level ${e.level} is being downloaded to test bitrate and will not be buffered`), this._loadBitrateTestFrag(e, t)) : (this.startFragRequested = true, super.loadFragment(e, t, s2)) : this.clearTrackerIfNeeded(e);
  }
  getBufferedFrag(e) {
    return this.fragmentTracker.getBufferedFrag(e, N2.MAIN);
  }
  followingBufferedFrag(e) {
    return e ? this.getBufferedFrag(e.end + 0.5) : null;
  }
  immediateLevelSwitch() {
    this.abortCurrentFrag(), this.flushMainBuffer(0, Number.POSITIVE_INFINITY);
  }
  nextLevelSwitch() {
    let { levels: e, media: t } = this;
    if (t != null && t.readyState) {
      let s2, i = this.getAppendedFrag(t.currentTime);
      i && i.start > 1 && this.flushMainBuffer(0, i.start - 1);
      let r2 = this.getLevelDetails();
      if (r2 != null && r2.live) {
        let o = this.getMainFwdBufferInfo();
        if (!o || o.len < r2.targetduration * 2)
          return;
      }
      if (!t.paused && e) {
        let o = this.hls.nextLoadLevel, l2 = e[o], c = this.fragLastKbps;
        c && this.fragCurrent ? s2 = this.fragCurrent.duration * l2.maxBitrate / (1e3 * c) + 1 : s2 = 0;
      } else
        s2 = 0;
      let a2 = this.getBufferedFrag(t.currentTime + s2);
      if (a2) {
        let o = this.followingBufferedFrag(a2);
        if (o) {
          this.abortCurrentFrag();
          let l2 = o.maxStartPTS ? o.maxStartPTS : o.start, c = o.duration, h2 = Math.max(a2.end, l2 + Math.min(Math.max(c - this.config.maxFragLookUpTolerance, c * (this.couldBacktrack ? 0.5 : 0.125)), c * (this.couldBacktrack ? 0.75 : 0.25)));
          this.flushMainBuffer(h2, Number.POSITIVE_INFINITY);
        }
      }
    }
  }
  abortCurrentFrag() {
    let e = this.fragCurrent;
    switch (this.fragCurrent = null, this.backtrackFragment = null, e && (e.abortRequests(), this.fragmentTracker.removeFragment(e)), this.state) {
      case D.KEY_LOADING:
      case D.FRAG_LOADING:
      case D.FRAG_LOADING_WAITING_RETRY:
      case D.PARSING:
      case D.PARSED:
        this.state = D.IDLE;
        break;
    }
    this.nextLoadPosition = this.getLoadPosition();
  }
  flushMainBuffer(e, t) {
    super.flushMainBuffer(e, t, this.altAudio ? "video" : null);
  }
  onMediaAttached(e, t) {
    super.onMediaAttached(e, t);
    let s2 = t.media;
    this.onvplaying = this.onMediaPlaying.bind(this), this.onvseeked = this.onMediaSeeked.bind(this), s2.addEventListener("playing", this.onvplaying), s2.addEventListener("seeked", this.onvseeked), this.gapController = new bi(this.config, s2, this.fragmentTracker, this.hls);
  }
  onMediaDetaching() {
    let { media: e } = this;
    e && this.onvplaying && this.onvseeked && (e.removeEventListener("playing", this.onvplaying), e.removeEventListener("seeked", this.onvseeked), this.onvplaying = this.onvseeked = null, this.videoBuffer = null), this.fragPlaying = null, this.gapController && (this.gapController.destroy(), this.gapController = null), super.onMediaDetaching();
  }
  onMediaPlaying() {
    this.tick();
  }
  onMediaSeeked() {
    let e = this.media, t = e ? e.currentTime : null;
    F(t) && this.log(`Media seeked to ${t.toFixed(3)}`);
    let s2 = this.getMainFwdBufferInfo();
    if (s2 === null || s2.len === 0) {
      this.warn(`Main forward buffer length on "seeked" event ${s2 ? s2.len : "empty"})`);
      return;
    }
    this.tick();
  }
  onManifestLoading() {
    this.log("Trigger BUFFER_RESET"), this.hls.trigger(m2.BUFFER_RESET, void 0), this.fragmentTracker.removeAllFragments(), this.couldBacktrack = false, this.startPosition = this.lastCurrentTime = this.fragLastKbps = 0, this.levels = this.fragPlaying = this.backtrackFragment = this.levelLastLoaded = null, this.altAudio = this.audioOnly = this.startFragRequested = false;
  }
  onManifestParsed(e, t) {
    let s2 = false, i = false;
    t.levels.forEach((r2) => {
      let a2 = r2.audioCodec;
      a2 && (s2 = s2 || a2.indexOf("mp4a.40.2") !== -1, i = i || a2.indexOf("mp4a.40.5") !== -1);
    }), this.audioCodecSwitch = s2 && i && !cc(), this.audioCodecSwitch && this.log("Both AAC/HE-AAC audio found in levels; declaring level codec as HE-AAC"), this.levels = t.levels, this.startFragRequested = false;
  }
  onLevelLoading(e, t) {
    let { levels: s2 } = this;
    if (!s2 || this.state !== D.IDLE)
      return;
    let i = s2[t.level];
    (!i.details || i.details.live && this.levelLastLoaded !== i || this.waitForCdnTuneIn(i.details)) && (this.state = D.WAITING_LEVEL);
  }
  onLevelLoaded(e, t) {
    var s2;
    let { levels: i } = this, r2 = t.level, a2 = t.details, o = a2.totalduration;
    if (!i) {
      this.warn(`Levels were reset while loading level ${r2}`);
      return;
    }
    this.log(`Level ${r2} loaded [${a2.startSN},${a2.endSN}]${a2.lastPartSn ? `[part-${a2.lastPartSn}-${a2.lastPartIndex}]` : ""}, cc [${a2.startCC}, ${a2.endCC}] duration:${o}`);
    let l2 = i[r2], c = this.fragCurrent;
    c && (this.state === D.FRAG_LOADING || this.state === D.FRAG_LOADING_WAITING_RETRY) && c.level !== t.level && c.loader && this.abortCurrentFrag();
    let h2 = 0;
    if (a2.live || (s2 = l2.details) != null && s2.live) {
      var u2;
      if (this.checkLiveUpdate(a2), a2.deltaUpdateFailed)
        return;
      h2 = this.alignPlaylists(a2, l2.details, (u2 = this.levelLastLoaded) == null ? void 0 : u2.details);
    }
    if (l2.details = a2, this.levelLastLoaded = l2, this.hls.trigger(m2.LEVEL_UPDATED, { details: a2, level: r2 }), this.state === D.WAITING_LEVEL) {
      if (this.waitForCdnTuneIn(a2))
        return;
      this.state = D.IDLE;
    }
    this.startFragRequested ? a2.live && this.synchronizeToLiveEdge(a2) : this.setStartPosition(a2, h2), this.tick();
  }
  _handleFragmentLoadProgress(e) {
    var t;
    let { frag: s2, part: i, payload: r2 } = e, { levels: a2 } = this;
    if (!a2) {
      this.warn(`Levels were reset while fragment load was in progress. Fragment ${s2.sn} of level ${s2.level} will not be buffered`);
      return;
    }
    let o = a2[s2.level], l2 = o.details;
    if (!l2) {
      this.warn(`Dropping fragment ${s2.sn} of level ${s2.level} after level details were reset`), this.fragmentTracker.removeFragment(s2);
      return;
    }
    let c = o.videoCodec, h2 = l2.PTSKnown || !l2.live, u2 = (t = s2.initSegment) == null ? void 0 : t.data, d3 = this._getAudioCodec(o), f3 = this.transmuxer = this.transmuxer || new Wt(this.hls, N2.MAIN, this._handleTransmuxComplete.bind(this), this._handleTransmuxerFlush.bind(this)), g2 = i ? i.index : -1, p3 = g2 !== -1, T2 = new at(s2.level, s2.sn, s2.stats.chunkCount, r2.byteLength, g2, p3), E4 = this.initPTS[s2.cc];
    f3.push(r2, u2, d3, c, s2, i, l2.totalduration, h2, T2, E4);
  }
  onAudioTrackSwitching(e, t) {
    let s2 = this.altAudio;
    if (!!!t.url) {
      if (this.mediaBuffer !== this.media) {
        this.log("Switching on main audio, use media.buffered to schedule main fragment loading"), this.mediaBuffer = this.media;
        let a2 = this.fragCurrent;
        a2 && (this.log("Switching to main audio track, cancel main fragment load"), a2.abortRequests(), this.fragmentTracker.removeFragment(a2)), this.resetTransmuxer(), this.resetLoadingState();
      } else
        this.audioOnly && this.resetTransmuxer();
      let r2 = this.hls;
      s2 && (r2.trigger(m2.BUFFER_FLUSHING, { startOffset: 0, endOffset: Number.POSITIVE_INFINITY, type: null }), this.fragmentTracker.removeAllFragments()), r2.trigger(m2.AUDIO_TRACK_SWITCHED, t);
    }
  }
  onAudioTrackSwitched(e, t) {
    let s2 = t.id, i = !!this.hls.audioTracks[s2].url;
    if (i) {
      let r2 = this.videoBuffer;
      r2 && this.mediaBuffer !== r2 && (this.log("Switching on alternate audio, use video.buffered to schedule main fragment loading"), this.mediaBuffer = r2);
    }
    this.altAudio = i, this.tick();
  }
  onBufferCreated(e, t) {
    let s2 = t.tracks, i, r2, a2 = false;
    for (let o in s2) {
      let l2 = s2[o];
      if (l2.id === "main") {
        if (r2 = o, i = l2, o === "video") {
          let c = s2[o];
          c && (this.videoBuffer = c.buffer);
        }
      } else
        a2 = true;
    }
    a2 && i ? (this.log(`Alternate track found, use ${r2}.buffered to schedule main fragment loading`), this.mediaBuffer = i.buffer) : this.mediaBuffer = this.media;
  }
  onFragBuffered(e, t) {
    let { frag: s2, part: i } = t;
    if (s2 && s2.type !== N2.MAIN)
      return;
    if (this.fragContextChanged(s2)) {
      this.warn(`Fragment ${s2.sn}${i ? " p: " + i.index : ""} of level ${s2.level} finished buffering, but was aborted. state: ${this.state}`), this.state === D.PARSED && (this.state = D.IDLE);
      return;
    }
    let r2 = i ? i.stats : s2.stats;
    this.fragLastKbps = Math.round(8 * r2.total / (r2.buffering.end - r2.loading.first)), s2.sn !== "initSegment" && (this.fragPrevious = s2), this.fragBufferedComplete(s2, i);
  }
  onError(e, t) {
    var s2;
    if (t.fatal) {
      this.state = D.ERROR;
      return;
    }
    switch (t.details) {
      case A2.FRAG_GAP:
      case A2.FRAG_PARSING_ERROR:
      case A2.FRAG_DECRYPT_ERROR:
      case A2.FRAG_LOAD_ERROR:
      case A2.FRAG_LOAD_TIMEOUT:
      case A2.KEY_LOAD_ERROR:
      case A2.KEY_LOAD_TIMEOUT:
        this.onFragmentOrKeyLoadError(N2.MAIN, t);
        break;
      case A2.LEVEL_LOAD_ERROR:
      case A2.LEVEL_LOAD_TIMEOUT:
      case A2.LEVEL_PARSING_ERROR:
        !t.levelRetry && this.state === D.WAITING_LEVEL && ((s2 = t.context) == null ? void 0 : s2.type) === W3.LEVEL && (this.state = D.IDLE);
        break;
      case A2.BUFFER_APPEND_ERROR:
      case A2.BUFFER_FULL_ERROR:
        if (!t.parent || t.parent !== "main")
          return;
        if (t.details === A2.BUFFER_APPEND_ERROR) {
          this.resetLoadingState();
          return;
        }
        this.reduceLengthAndFlushBuffer(t) && this.flushMainBuffer(0, Number.POSITIVE_INFINITY);
        break;
      case A2.INTERNAL_EXCEPTION:
        this.recoverWorkerError(t);
        break;
    }
  }
  checkBuffer() {
    let { media: e, gapController: t } = this;
    if (!(!e || !t || !e.readyState)) {
      if (this.loadedmetadata || !Q.getBuffered(e).length) {
        let s2 = this.state !== D.IDLE ? this.fragCurrent : null;
        t.poll(this.lastCurrentTime, s2);
      }
      this.lastCurrentTime = e.currentTime;
    }
  }
  onFragLoadEmergencyAborted() {
    this.state = D.IDLE, this.loadedmetadata || (this.startFragRequested = false, this.nextLoadPosition = this.startPosition), this.tickImmediate();
  }
  onBufferFlushed(e, { type: t }) {
    if (t !== z2.AUDIO || this.audioOnly && !this.altAudio) {
      let s2 = (t === z2.VIDEO ? this.videoBuffer : this.mediaBuffer) || this.media;
      this.afterBufferFlushed(s2, t, N2.MAIN), this.tick();
    }
  }
  onLevelsUpdated(e, t) {
    this.level > -1 && this.fragCurrent && (this.level = this.fragCurrent.level), this.levels = t.levels;
  }
  swapAudioCodec() {
    this.audioCodecSwap = !this.audioCodecSwap;
  }
  seekToStartPos() {
    let { media: e } = this;
    if (!e)
      return;
    let t = e.currentTime, s2 = this.startPosition;
    if (s2 >= 0 && t < s2) {
      if (e.seeking) {
        this.log(`could not seek to ${s2}, already seeking at ${t}`);
        return;
      }
      let i = Q.getBuffered(e), a2 = (i.length ? i.start(0) : 0) - s2;
      a2 > 0 && (a2 < this.config.maxBufferHole || a2 < this.config.maxFragLookUpTolerance) && (this.log(`adjusting start position by ${a2} to match buffer start`), s2 += a2, this.startPosition = s2), this.log(`seek to target start position ${s2} from current time ${t}`), e.currentTime = s2;
    }
  }
  _getAudioCodec(e) {
    let t = this.config.defaultAudioCodec || e.audioCodec;
    return this.audioCodecSwap && t && (this.log("Swapping audio codec"), t.indexOf("mp4a.40.5") !== -1 ? t = "mp4a.40.2" : t = "mp4a.40.5"), t;
  }
  _loadBitrateTestFrag(e, t) {
    e.bitrateTest = true, this._doFragLoad(e, t).then((s2) => {
      let { hls: i } = this;
      if (!s2 || this.fragContextChanged(e))
        return;
      t.fragmentError = 0, this.state = D.IDLE, this.startFragRequested = false, this.bitrateTest = false;
      let r2 = e.stats;
      r2.parsing.start = r2.parsing.end = r2.buffering.start = r2.buffering.end = self.performance.now(), i.trigger(m2.FRAG_LOADED, s2), e.bitrateTest = false;
    });
  }
  _handleTransmuxComplete(e) {
    var t;
    let s2 = "main", { hls: i } = this, { remuxResult: r2, chunkMeta: a2 } = e, o = this.getCurrentContext(a2);
    if (!o) {
      this.resetWhenMissingContext(a2);
      return;
    }
    let { frag: l2, part: c, level: h2 } = o, { video: u2, text: d3, id3: f3, initSegment: g2 } = r2, { details: p3 } = h2, T2 = this.altAudio ? void 0 : r2.audio;
    if (this.fragContextChanged(l2)) {
      this.fragmentTracker.removeFragment(l2);
      return;
    }
    if (this.state = D.PARSING, g2) {
      if (g2 != null && g2.tracks) {
        let y3 = l2.initSegment || l2;
        this._bufferInitSegment(h2, g2.tracks, y3, a2), i.trigger(m2.FRAG_PARSING_INIT_SEGMENT, { frag: y3, id: s2, tracks: g2.tracks });
      }
      let E4 = g2.initPTS, x3 = g2.timescale;
      F(E4) && (this.initPTS[l2.cc] = { baseTime: E4, timescale: x3 }, i.trigger(m2.INIT_PTS_FOUND, { frag: l2, id: s2, initPTS: E4, timescale: x3 }));
    }
    if (u2 && p3 && l2.sn !== "initSegment") {
      let E4 = p3.fragments[l2.sn - 1 - p3.startSN], x3 = l2.sn === p3.startSN, y3 = !E4 || l2.cc > E4.cc;
      if (r2.independent !== false) {
        let { startPTS: R, endPTS: S2, startDTS: b, endDTS: L } = u2;
        if (c)
          c.elementaryStreams[u2.type] = { startPTS: R, endPTS: S2, startDTS: b, endDTS: L };
        else if (u2.firstKeyFrame && u2.independent && a2.id === 1 && !y3 && (this.couldBacktrack = true), u2.dropped && u2.independent) {
          let C2 = this.getMainFwdBufferInfo(), _ = (C2 ? C2.end : this.getLoadPosition()) + this.config.maxBufferHole, I = u2.firstKeyFramePTS ? u2.firstKeyFramePTS : R;
          if (!x3 && _ < I - this.config.maxBufferHole && !y3) {
            this.backtrack(l2);
            return;
          } else
            y3 && (l2.gap = true);
          l2.setElementaryStreamInfo(u2.type, l2.start, S2, l2.start, L, true);
        } else
          x3 && R > It && (l2.gap = true);
        l2.setElementaryStreamInfo(u2.type, R, S2, b, L), this.backtrackFragment && (this.backtrackFragment = l2), this.bufferFragmentData(u2, l2, c, a2, x3 || y3);
      } else if (x3 || y3)
        l2.gap = true;
      else {
        this.backtrack(l2);
        return;
      }
    }
    if (T2) {
      let { startPTS: E4, endPTS: x3, startDTS: y3, endDTS: R } = T2;
      c && (c.elementaryStreams[z2.AUDIO] = { startPTS: E4, endPTS: x3, startDTS: y3, endDTS: R }), l2.setElementaryStreamInfo(z2.AUDIO, E4, x3, y3, R), this.bufferFragmentData(T2, l2, c, a2);
    }
    if (p3 && f3 != null && (t = f3.samples) != null && t.length) {
      let E4 = { id: s2, frag: l2, details: p3, samples: f3.samples };
      i.trigger(m2.FRAG_PARSING_METADATA, E4);
    }
    if (p3 && d3) {
      let E4 = { id: s2, frag: l2, details: p3, samples: d3.samples };
      i.trigger(m2.FRAG_PARSING_USERDATA, E4);
    }
  }
  _bufferInitSegment(e, t, s2, i) {
    if (this.state !== D.PARSING)
      return;
    this.audioOnly = !!t.audio && !t.video, this.altAudio && !this.audioOnly && delete t.audio;
    let { audio: r2, video: a2, audiovideo: o } = t;
    if (r2) {
      let l2 = e.audioCodec, c = navigator.userAgent.toLowerCase();
      this.audioCodecSwitch && (l2 && (l2.indexOf("mp4a.40.5") !== -1 ? l2 = "mp4a.40.2" : l2 = "mp4a.40.5"), r2.metadata.channelCount !== 1 && c.indexOf("firefox") === -1 && (l2 = "mp4a.40.5")), l2 && l2.indexOf("mp4a.40.5") !== -1 && c.indexOf("android") !== -1 && r2.container !== "audio/mpeg" && (l2 = "mp4a.40.2", this.log(`Android: force audio codec to ${l2}`)), e.audioCodec && e.audioCodec !== l2 && this.log(`Swapping manifest audio codec "${e.audioCodec}" for "${l2}"`), r2.levelCodec = l2, r2.id = "main", this.log(`Init audio buffer, container:${r2.container}, codecs[selected/level/parsed]=[${l2 || ""}/${e.audioCodec || ""}/${r2.codec}]`);
    }
    a2 && (a2.levelCodec = e.videoCodec, a2.id = "main", this.log(`Init video buffer, container:${a2.container}, codecs[level/parsed]=[${e.videoCodec || ""}/${a2.codec}]`)), o && this.log(`Init audiovideo buffer, container:${o.container}, codecs[level/parsed]=[${e.codecs}/${o.codec}]`), this.hls.trigger(m2.BUFFER_CODECS, t), Object.keys(t).forEach((l2) => {
      let h2 = t[l2].initSegment;
      h2 != null && h2.byteLength && this.hls.trigger(m2.BUFFER_APPENDING, { type: l2, data: h2, frag: s2, part: null, chunkMeta: i, parent: s2.type });
    }), this.tickImmediate();
  }
  getMainFwdBufferInfo() {
    return this.getFwdBufferInfo(this.mediaBuffer ? this.mediaBuffer : this.media, N2.MAIN);
  }
  backtrack(e) {
    this.couldBacktrack = true, this.backtrackFragment = e, this.resetTransmuxer(), this.flushBufferGap(e), this.fragmentTracker.removeFragment(e), this.fragPrevious = null, this.nextLoadPosition = e.start, this.state = D.IDLE;
  }
  checkFragmentChanged() {
    let e = this.media, t = null;
    if (e && e.readyState > 1 && e.seeking === false) {
      let s2 = e.currentTime;
      if (Q.isBuffered(e, s2) ? t = this.getAppendedFrag(s2) : Q.isBuffered(e, s2 + 0.1) && (t = this.getAppendedFrag(s2 + 0.1)), t) {
        this.backtrackFragment = null;
        let i = this.fragPlaying, r2 = t.level;
        (!i || t.sn !== i.sn || i.level !== r2) && (this.fragPlaying = t, this.hls.trigger(m2.FRAG_CHANGED, { frag: t }), (!i || i.level !== r2) && this.hls.trigger(m2.LEVEL_SWITCHED, { level: r2 }));
      }
    }
  }
  get nextLevel() {
    let e = this.nextBufferedFrag;
    return e ? e.level : -1;
  }
  get currentFrag() {
    let e = this.media;
    return e ? this.fragPlaying || this.getAppendedFrag(e.currentTime) : null;
  }
  get currentProgramDateTime() {
    let e = this.media;
    if (e) {
      let t = e.currentTime, s2 = this.currentFrag;
      if (s2 && F(t) && F(s2.programDateTime)) {
        let i = s2.programDateTime + (t - s2.start) * 1e3;
        return new Date(i);
      }
    }
    return null;
  }
  get currentLevel() {
    let e = this.currentFrag;
    return e ? e.level : -1;
  }
  get nextBufferedFrag() {
    let e = this.currentFrag;
    return e ? this.followingBufferedFrag(e) : null;
  }
  get forceStartLoad() {
    return this._forceStartLoad;
  }
};
var Ci = class n11 {
  static get version() {
    return "1.5.8";
  }
  static isMSESupported() {
    return Gn();
  }
  static isSupported() {
    return lc();
  }
  static getMediaSource() {
    return Ue2();
  }
  static get Events() {
    return m2;
  }
  static get ErrorTypes() {
    return B2;
  }
  static get ErrorDetails() {
    return A2;
  }
  static get DefaultConfig() {
    return n11.defaultConfig ? n11.defaultConfig : Bn;
  }
  static set DefaultConfig(e) {
    n11.defaultConfig = e;
  }
  constructor(e = {}) {
    this.config = void 0, this.userConfig = void 0, this.coreComponents = void 0, this.networkControllers = void 0, this.started = false, this._emitter = new Ui(), this._autoLevelCapping = -1, this._maxHdcpLevel = null, this.abrController = void 0, this.bufferController = void 0, this.capLevelController = void 0, this.latencyController = void 0, this.levelController = void 0, this.streamController = void 0, this.audioTrackController = void 0, this.subtitleTrackController = void 0, this.emeController = void 0, this.cmcdController = void 0, this._media = null, this.url = null, this.triggeringException = void 0, Xn(e.debug || false, "Hls instance");
    let t = this.config = ac(n11.DefaultConfig, e);
    this.userConfig = e, t.progressive && oc(t);
    let { abrController: s2, bufferController: i, capLevelController: r2, errorController: a2, fpsController: o } = t, l2 = new a2(this), c = this.abrController = new s2(this), h2 = this.bufferController = new i(this), u2 = this.capLevelController = new r2(this), d3 = new o(this), f3 = new Rs(this), g2 = new Ds(this), p3 = t.contentSteeringController, T2 = p3 ? new p3(this) : null, E4 = this.levelController = new Ri(this, T2), x3 = new Ns(this), y3 = new Ii(this.config), R = this.streamController = new Di(this, x3, y3);
    u2.setStreamController(R), d3.setStreamController(R);
    let S2 = [f3, E4, R];
    T2 && S2.splice(1, 0, T2), this.networkControllers = S2;
    let b = [c, h2, u2, d3, g2, x3];
    this.audioTrackController = this.createController(t.audioTrackController, S2);
    let L = t.audioStreamController;
    L && S2.push(new L(this, x3, y3)), this.subtitleTrackController = this.createController(t.subtitleTrackController, S2);
    let C2 = t.subtitleStreamController;
    C2 && S2.push(new C2(this, x3, y3)), this.createController(t.timelineController, b), y3.emeController = this.emeController = this.createController(t.emeController, b), this.cmcdController = this.createController(t.cmcdController, b), this.latencyController = this.createController(Cs, b), this.coreComponents = b, S2.push(l2);
    let _ = l2.onErrorOut;
    typeof _ == "function" && this.on(m2.ERROR, _, l2);
  }
  createController(e, t) {
    if (e) {
      let s2 = new e(this);
      return t && t.push(s2), s2;
    }
    return null;
  }
  on(e, t, s2 = this) {
    this._emitter.on(e, t, s2);
  }
  once(e, t, s2 = this) {
    this._emitter.once(e, t, s2);
  }
  removeAllListeners(e) {
    this._emitter.removeAllListeners(e);
  }
  off(e, t, s2 = this, i) {
    this._emitter.off(e, t, s2, i);
  }
  listeners(e) {
    return this._emitter.listeners(e);
  }
  emit(e, t, s2) {
    return this._emitter.emit(e, t, s2);
  }
  trigger(e, t) {
    if (this.config.debug)
      return this.emit(e, e, t);
    try {
      return this.emit(e, e, t);
    } catch (s2) {
      if (v2.error("An internal error happened while handling event " + e + '. Error message: "' + s2.message + '". Here is a stacktrace:', s2), !this.triggeringException) {
        this.triggeringException = true;
        let i = e === m2.ERROR;
        this.trigger(m2.ERROR, { type: B2.OTHER_ERROR, details: A2.INTERNAL_EXCEPTION, fatal: i, event: e, error: s2 }), this.triggeringException = false;
      }
    }
    return false;
  }
  listenerCount(e) {
    return this._emitter.listenerCount(e);
  }
  destroy() {
    v2.log("destroy"), this.trigger(m2.DESTROYING, void 0), this.detachMedia(), this.removeAllListeners(), this._autoLevelCapping = -1, this.url = null, this.networkControllers.forEach((t) => t.destroy()), this.networkControllers.length = 0, this.coreComponents.forEach((t) => t.destroy()), this.coreComponents.length = 0;
    let e = this.config;
    e.xhrSetup = e.fetchSetup = void 0, this.userConfig = null;
  }
  attachMedia(e) {
    v2.log("attachMedia"), this._media = e, this.trigger(m2.MEDIA_ATTACHING, { media: e });
  }
  detachMedia() {
    v2.log("detachMedia"), this.trigger(m2.MEDIA_DETACHING, void 0), this._media = null;
  }
  loadSource(e) {
    this.stopLoad();
    let t = this.media, s2 = this.url, i = this.url = wi.buildAbsoluteURL(self.location.href, e, { alwaysNormalize: true });
    this._autoLevelCapping = -1, this._maxHdcpLevel = null, v2.log(`loadSource:${i}`), t && s2 && (s2 !== i || this.bufferController.hasSourceTypes()) && (this.detachMedia(), this.attachMedia(t)), this.trigger(m2.MANIFEST_LOADING, { url: e });
  }
  startLoad(e = -1) {
    v2.log(`startLoad(${e})`), this.started = true, this.networkControllers.forEach((t) => {
      t.startLoad(e);
    });
  }
  stopLoad() {
    v2.log("stopLoad"), this.started = false, this.networkControllers.forEach((e) => {
      e.stopLoad();
    });
  }
  resumeBuffering() {
    this.started && this.networkControllers.forEach((e) => {
      "fragmentLoader" in e && e.startLoad(-1);
    });
  }
  pauseBuffering() {
    this.networkControllers.forEach((e) => {
      "fragmentLoader" in e && e.stopLoad();
    });
  }
  swapAudioCodec() {
    v2.log("swapAudioCodec"), this.streamController.swapAudioCodec();
  }
  recoverMediaError() {
    v2.log("recoverMediaError");
    let e = this._media;
    this.detachMedia(), e && this.attachMedia(e);
  }
  removeLevel(e) {
    this.levelController.removeLevel(e);
  }
  get levels() {
    let e = this.levelController.levels;
    return e || [];
  }
  get currentLevel() {
    return this.streamController.currentLevel;
  }
  set currentLevel(e) {
    v2.log(`set currentLevel:${e}`), this.levelController.manualLevel = e, this.streamController.immediateLevelSwitch();
  }
  get nextLevel() {
    return this.streamController.nextLevel;
  }
  set nextLevel(e) {
    v2.log(`set nextLevel:${e}`), this.levelController.manualLevel = e, this.streamController.nextLevelSwitch();
  }
  get loadLevel() {
    return this.levelController.level;
  }
  set loadLevel(e) {
    v2.log(`set loadLevel:${e}`), this.levelController.manualLevel = e;
  }
  get nextLoadLevel() {
    return this.levelController.nextLoadLevel;
  }
  set nextLoadLevel(e) {
    this.levelController.nextLoadLevel = e;
  }
  get firstLevel() {
    return Math.max(this.levelController.firstLevel, this.minAutoLevel);
  }
  set firstLevel(e) {
    v2.log(`set firstLevel:${e}`), this.levelController.firstLevel = e;
  }
  get startLevel() {
    let e = this.levelController.startLevel;
    return e === -1 && this.abrController.forcedAutoLevel > -1 ? this.abrController.forcedAutoLevel : e;
  }
  set startLevel(e) {
    v2.log(`set startLevel:${e}`), e !== -1 && (e = Math.max(e, this.minAutoLevel)), this.levelController.startLevel = e;
  }
  get capLevelToPlayerSize() {
    return this.config.capLevelToPlayerSize;
  }
  set capLevelToPlayerSize(e) {
    let t = !!e;
    t !== this.config.capLevelToPlayerSize && (t ? this.capLevelController.startCapping() : (this.capLevelController.stopCapping(), this.autoLevelCapping = -1, this.streamController.nextLevelSwitch()), this.config.capLevelToPlayerSize = t);
  }
  get autoLevelCapping() {
    return this._autoLevelCapping;
  }
  get bandwidthEstimate() {
    let { bwEstimator: e } = this.abrController;
    return e ? e.getEstimate() : NaN;
  }
  set bandwidthEstimate(e) {
    this.abrController.resetEstimator(e);
  }
  get ttfbEstimate() {
    let { bwEstimator: e } = this.abrController;
    return e ? e.getEstimateTTFB() : NaN;
  }
  set autoLevelCapping(e) {
    this._autoLevelCapping !== e && (v2.log(`set autoLevelCapping:${e}`), this._autoLevelCapping = e, this.levelController.checkMaxAutoUpdated());
  }
  get maxHdcpLevel() {
    return this._maxHdcpLevel;
  }
  set maxHdcpLevel(e) {
    Ya(e) && this._maxHdcpLevel !== e && (this._maxHdcpLevel = e, this.levelController.checkMaxAutoUpdated());
  }
  get autoLevelEnabled() {
    return this.levelController.manualLevel === -1;
  }
  get manualLevel() {
    return this.levelController.manualLevel;
  }
  get minAutoLevel() {
    let { levels: e, config: { minAutoBitrate: t } } = this;
    if (!e)
      return 0;
    let s2 = e.length;
    for (let i = 0; i < s2; i++)
      if (e[i].maxBitrate >= t)
        return i;
    return 0;
  }
  get maxAutoLevel() {
    let { levels: e, autoLevelCapping: t, maxHdcpLevel: s2 } = this, i;
    if (t === -1 && e != null && e.length ? i = e.length - 1 : i = t, s2)
      for (let r2 = i; r2--; ) {
        let a2 = e[r2].attrs["HDCP-LEVEL"];
        if (a2 && a2 <= s2)
          return r2;
      }
    return i;
  }
  get firstAutoLevel() {
    return this.abrController.firstAutoLevel;
  }
  get nextAutoLevel() {
    return this.abrController.nextAutoLevel;
  }
  set nextAutoLevel(e) {
    this.abrController.nextAutoLevel = e;
  }
  get playingDate() {
    return this.streamController.currentProgramDateTime;
  }
  get mainForwardBufferInfo() {
    return this.streamController.getMainFwdBufferInfo();
  }
  setAudioOption(e) {
    var t;
    return (t = this.audioTrackController) == null ? void 0 : t.setAudioOption(e);
  }
  setSubtitleOption(e) {
    var t;
    return (t = this.subtitleTrackController) == null || t.setSubtitleOption(e), null;
  }
  get allAudioTracks() {
    let e = this.audioTrackController;
    return e ? e.allAudioTracks : [];
  }
  get audioTracks() {
    let e = this.audioTrackController;
    return e ? e.audioTracks : [];
  }
  get audioTrack() {
    let e = this.audioTrackController;
    return e ? e.audioTrack : -1;
  }
  set audioTrack(e) {
    let t = this.audioTrackController;
    t && (t.audioTrack = e);
  }
  get allSubtitleTracks() {
    let e = this.subtitleTrackController;
    return e ? e.allSubtitleTracks : [];
  }
  get subtitleTracks() {
    let e = this.subtitleTrackController;
    return e ? e.subtitleTracks : [];
  }
  get subtitleTrack() {
    let e = this.subtitleTrackController;
    return e ? e.subtitleTrack : -1;
  }
  get media() {
    return this._media;
  }
  set subtitleTrack(e) {
    let t = this.subtitleTrackController;
    t && (t.subtitleTrack = e);
  }
  get subtitleDisplay() {
    let e = this.subtitleTrackController;
    return e ? e.subtitleDisplay : false;
  }
  set subtitleDisplay(e) {
    let t = this.subtitleTrackController;
    t && (t.subtitleDisplay = e);
  }
  get lowLatencyMode() {
    return this.config.lowLatencyMode;
  }
  set lowLatencyMode(e) {
    this.config.lowLatencyMode = e;
  }
  get liveSyncPosition() {
    return this.latencyController.liveSyncPosition;
  }
  get latency() {
    return this.latencyController.latency;
  }
  get maxLatency() {
    return this.latencyController.maxLatency;
  }
  get targetLatency() {
    return this.latencyController.targetLatency;
  }
  get drift() {
    return this.latencyController.drift;
  }
  get forceStartLoad() {
    return this.streamController.forceStartLoad;
  }
};
Ci.defaultConfig = void 0;

// NiconicomeWeb/src/shared/AttemptResult.ts
var AttemptResultWidthDataImpl = class _AttemptResultWidthDataImpl {
  constructor(isSucceeded, data, message) {
    this.IsSucceeded = isSucceeded;
    this.Data = data;
    this.Message = message;
  }
  IsSucceeded;
  Data;
  Message;
  static Succeeded(data) {
    return new _AttemptResultWidthDataImpl(true, data, null);
  }
  static Fail(message) {
    return new _AttemptResultWidthDataImpl(false, null, message);
  }
};
var AttemptResultImpl = class _AttemptResultImpl {
  constructor(isSucceeded, message) {
    this.IsSucceeded = isSucceeded;
    this.Message = message;
  }
  IsSucceeded;
  Message;
  static Succeeded() {
    return new _AttemptResultImpl(true, null);
  }
  static Fail(message) {
    return new _AttemptResultImpl(false, message);
  }
};

// NiconicomeWeb/src/watch/ui/video/videoElement.ts
var VideoImpl = class {
  _videoElement;
  _hls;
  _source;
  _listners = /* @__PURE__ */ new Map();
  constructor(videoElement) {
    this._videoElement = videoElement;
  }
  get duration() {
    return this._videoElement.duration;
  }
  get currentTime() {
    return this._videoElement.currentTime;
  }
  set currentTime(time) {
    this._videoElement.currentTime = time;
  }
  get paused() {
    return this._videoElement.paused;
  }
  get buffered() {
    return this._videoElement.buffered;
  }
  get repeat() {
    return this._videoElement.loop;
  }
  set repeat(repeat) {
    this._videoElement.loop = repeat;
  }
  async Initialize(source, videoElement) {
    if (!Ci.isSupported()) {
      return AttemptResultImpl.Fail("HLS is not supported");
    }
    if (videoElement) {
      this._videoElement = videoElement;
    }
    this._source = source;
    if (this._hls) {
      this._hls.destroy();
    }
    this._hls = new Ci();
    if (!this._source.media.isDMS) {
      const res = await fetch(source.media.createUrl);
      if (!res.ok) {
        return AttemptResultImpl.Fail("Failed to fetch DMS URL");
      }
      Logger.log("HLS\u30E1\u30C7\u30A3\u30A2\u30D5\u30A1\u30A4\u30EB\u3092\u751F\u6210\u3057\u307E\u3057\u305F\u3002");
      this._hls.loadSource(source.media.contentUrl);
      this._hls.attachMedia(this._videoElement);
    } else {
      this._hls.loadSource(source.media.contentUrl);
      this._hls.attachMedia(this._videoElement);
    }
    return AttemptResultImpl.Succeeded();
  }
  on(event, listener) {
    if (!this._listners.has(event))
      this._listners.set(event, []);
    this._listners.get(event).push(listener);
    this._videoElement.addEventListener(event, listener);
  }
  off(event, listener) {
    if (this._listners.has(event)) {
      this._listners.set(
        event,
        this._listners.get(event).filter((l2) => l2 !== listener)
      );
    }
    this._videoElement.removeEventListener(event, listener);
  }
  play() {
    this._videoElement.play();
  }
  pause() {
    this._videoElement.pause();
  }
  listened(ev, listener) {
    const listeners = this._listners.get(ev);
    if (!listeners)
      return false;
    return listeners.filter((l2) => l2 === listener).length > 0;
  }
};

// NiconicomeWeb/src/watch/ui/comment/drawer/utils/DataUtils.ts
var DataUtils = class {
  splitter(value, splitBy = ",") {
    if (value) {
      return value.split(splitBy);
    } else {
      return [];
    }
    ;
  }
  //0埋め
  padding(num, digit = 2, paddString = "0") {
    if (num < 10 ** digit) {
      const origin = num.toString();
      let padded = origin;
      for (let i = 0; i < digit - origin.length; i++) {
        padded = paddString + padded;
      }
      return padded;
    } else {
      return num.toString();
    }
  }
  /**
   * ネストされた配列を返します。
   * @param len 配列長
   */
  createNestedArray(len) {
    const arr = [];
    for (let i = 0; i < len; i++) {
      arr.push([]);
    }
    return arr;
  }
  createArray(len) {
    const arr = [];
    arr.length = len;
    return arr;
  }
  /**
   * VPOSから時間を計算します
   * @param vpos VPOS
   */
  calcVPOS(vpos) {
    const second = Math.floor(vpos / 100);
    const minute = Math.floor(second / 60);
    const extra = Math.floor(second % 60);
    return { ms: { min: minute, sec: extra }, sec: second };
  }
};

// NiconicomeWeb/src/watch/ui/comment/drawer/module/config.ts
var config_default = {
  fps: 60,
  duration: 3e3,
  lineHeight: 1.2,
  bigLines: 8,
  mediumLines: 11,
  smallLines: 16,
  fix: {
    smal: 38,
    big: 16,
    medium: 25
  },
  defaultColor: "#fff",
  defaultLayer: "base",
  defaultFont: "MS PGothic",
  opacity: 1
};

// NiconicomeWeb/src/watch/ui/comment/drawer/module/Comment.ts
var datautl = new DataUtils();
var CommentBase = class {
  ctx;
  //canvas
  customAttr;
  //カスタム属性
  duration;
  //表示時間
  canvasSize;
  //canvasサイズ
  canvasWidthFlash;
  //canvas横幅
  width;
  //幅
  text;
  //本文
  textForRender;
  //描写用
  originalText;
  //整形前のコメント
  color;
  //色
  borderColor;
  //縁路
  commentNumber;
  //コメ番
  lines;
  //同種のコメントの行数
  selfLines;
  //コメントの行数
  life = 0;
  //残りコマ数
  delta = 0;
  //1コマ当たりのX座標の変量
  left = 0;
  //X座標
  top = 0;
  //Y座標(原点は左上)
  reveal = 0;
  //コメントが画面右から完全に露出するまでのコマ数
  touch = 0;
  //コメントの左端が画面左に到達するまでのコマ数
  fontSize;
  //フォントサイズ
  fontName;
  //フォント名
  overallSize;
  //オブジェクト高さ
  fontSizeString;
  //small/big/mediumのいずれか
  type;
  //タイプ
  alive;
  //フラグ
  fixed = false;
  //固定フラグ
  vpos;
  //vpos
  opacity;
  //透過度
  full;
  textLength;
  //コメントの長さ
  maxLengthIndex;
  //一番長いコメントが格納されているインデックス
  onDisposed;
  constructor(param) {
    this.type = param.option.mode;
    this.customAttr = param.option.customAttr;
    this.originalText = param.text;
    this.text = this._formatComment(param.text, this.type);
    this.textForRender = this._formatRenderComment(this.text);
    this.textLength = Math.max(...this.text.map((text) => text.length));
    this.maxLengthIndex = this.text.map((comment) => comment.length).indexOf(
      this.textLength
    );
    this.commentNumber = param.commentNumber;
    this.color = param.option.color;
    this.borderColor = param.option.borderColor;
    this.lines = this._getLines(param.option.commentSize, param.lines);
    this.selfLines = this.text.length;
    this.ctx = param.ctx;
    this.fontName = param.option.fontName;
    this.opacity = param.option.opacity;
    this.full = param.option.full;
    this.canvasSize = param.canvasSize;
    this.canvasWidthFlash = param.canvasWidthFlash;
    if (this.text.length >= this.lines) {
      this.fixed = true;
    }
    this.vpos = param.option.vpos;
    this.duration = param.option.duration || config_default.duration;
    this.fontSizeString = param.option.commentSize;
    [this.fontSize, this.width] = this._getFont(
      this.text[this.maxLengthIndex],
      this.canvasSize,
      this.canvasWidthFlash,
      param.option.commentSize,
      param.fontSize,
      param.fixedFontSize,
      this.type
    );
    this.fontSizeString = param.option.commentSize;
    this.overallSize = this.fontSize * this.selfLines;
    this.alive = true;
    this.onDisposed = param.onDisposed;
    this._set();
  }
  //属性取得
  getProp(key) {
    return this.customAttr.get(key);
  }
  /**
   * 更新処理
   */
  tick(vpos) {
    if (vpos >= this.vpos + this.duration * 2) {
      this.alive = false;
      return;
    }
    if (this.vpos > vpos + this.duration) {
      this.alive = false;
      return;
    }
    this.life--;
    if (this.type === "naka") {
      this.left -= this.delta;
      this.reveal--;
      this.touch--;
    }
    if (this.life < 0) {
      this.kill();
    }
  }
  /**
   * コメントの生存フラグを強制的に降ろします
   */
  kill() {
    this.alive = false;
    this.onDisposed();
  }
  //セット
  _set() {
    this.life = 0;
    this.left = 0;
    this.top = 0;
    this.delta = 0;
    this.reveal = 0;
    this.touch = 0;
    switch (this.type) {
      case "naka":
        this._setNaka();
        break;
      case "shita":
      case "ue":
        this._setShitaUe();
    }
  }
  //nakaコメント設定
  _setNaka() {
    this.life = config_default.fps * this.duration / 1e3;
    this.left = this.canvasSize.width;
    this.delta = (this.canvasSize.width + this.width) / this.life;
    this.reveal = this.width / this.delta;
    this.touch = this.canvasSize.width / this.delta;
  }
  /**
   * shit・ueコメントを設定する
   * @param width canvasの横幅
   * @param duration コメント表示時間
   */
  _setShitaUe() {
    this.life = config_default.fps * this.duration / 1e3;
    if (this.fixed) {
      this.left = (this.canvasSize.width - this.width) / 2;
    } else {
      this.left = this.canvasSize.width / 2;
    }
  }
  //fontを決定する
  _getFont(text, canvasSize, flashWidth, commentSize, fontSize, fixedFontSize, type) {
    let originalFont = this._getSize(commentSize, fontSize);
    let font = originalFont;
    if (originalFont * this.selfLines > canvasSize.height / 3) {
      font = this._getSize(commentSize, fixedFontSize);
    }
    this.ctx.font = `${font}px "Yu Gothic"`;
    let comWidth = this.ctx.measureText(text).width;
    const widthOverflow = comWidth > this.canvasSize.width && type !== "naka";
    if (widthOverflow && !this.full) {
      font = this._modSize(originalFont, comWidth, this.canvasWidthFlash);
      this.ctx.font = `${font}px "Yu Gothic"`;
      comWidth = this.ctx.measureText(text).width;
    } else if (widthOverflow && this.full) {
      font = this._modSize(originalFont, comWidth, this.canvasSize.width);
      this.ctx.font = `${font}px "Yu Gothic"`;
      comWidth = this.ctx.measureText(text).width;
    }
    return [font, comWidth];
  }
  //フォントサイズ修正
  _modSize(font, width, canvasWidth) {
    return font * canvasWidth / width;
  }
  //fontSize取得
  _getSize(commentSize, fontSize) {
    switch (commentSize) {
      case "big":
        return fontSize.big;
      case "small":
        return fontSize.small;
      default:
        return fontSize.medium;
    }
  }
  /**
   * 行数を求める
   * @param size big/small/mediumのいずれか
   * @param allLines commentLines型のオブジェクト(それぞれのサイズについて行数を表す)
   */
  _getLines(size, allLines) {
    switch (size) {
      case "big":
        return allLines.big;
      case "medium":
        return allLines.medium;
      case "small":
        return allLines.small;
    }
  }
  /**
   * コメントを整形します
   * @param origin コメント
   * @param commentPos コメントのタイプ
   */
  _formatComment(origin, commentPos) {
    let formated = datautl.splitter(origin, "\n");
    formated = this._deleteBlank(formated);
    formated = this._deleteFirstAndLastBlank(formated);
    formated = this._sortByType(formated, commentPos);
    return formated;
  }
  /**
   * 描写用コメントを整形します
   * @param origin コメント
   * @param commentPos コメントのタイプ
   */
  _formatRenderComment(origin) {
    let formated = [...origin];
    formated = this._deleteBlankLineFromEnd(origin);
    return formated;
  }
  /**
   * コメントから3回以上連続する改行を削除します。
   * @param comments コメントのリスト
   */
  _deleteBlank(commentList) {
    let count = 0;
    const deleted = [];
    for (const text of commentList) {
      const isBlank = !text;
      if (isBlank) {
        count++;
      } else {
        count = 0;
      }
      if (count < 3) {
        deleted.push(text);
      }
    }
    return deleted;
  }
  /**
   * 最初と最後の空白行を削除します。
   * @param comments コメントのリスト
   */
  _deleteFirstAndLastBlank(comments) {
    const deleted = [...comments];
    for (const comment of comments) {
      if (!comment) {
        deleted.shift();
      } else {
        break;
      }
    }
    const reversed = [...deleted].reverse();
    for (const comment of reversed) {
      if (!comment) {
        deleted.pop();
      } else {
        break;
      }
    }
    return deleted;
  }
  /**
   * コメントの位置によってソートします
   * @param comments コメントのリスト
   * @param type コメントの位置
   */
  _sortByType(comments, type) {
    return type === "shita" ? comments.reverse() : comments;
  }
  /**
   * 空行を削除します
   * @param comments コメント
   */
  _deleteBlankLineFromEnd(comments) {
    const formated = [...comments];
    const reversed = [...comments].reverse();
    for (const line of reversed) {
      const isBlank = /^\s+$/.test(line) || line.length === 0;
      if (isBlank) {
        formated.pop();
      } else {
        break;
      }
    }
    return formated;
  }
};
var Size = class {
  height;
  width;
  constructor(size) {
    this.height = size.height;
    this.width = size.width;
  }
};
var Layer = class {
  ctx;
  //context
  canvasSize;
  //横幅
  canvasWidthFlash;
  //flash板横幅
  lines;
  //行数
  fonrSize;
  //fontsize
  fixedFonrSize;
  //fontsize
  duration;
  naka;
  //nakaコメント配列
  shita;
  //shitaコメント配列
  ue;
  //ueコメント配列
  maxlines;
  //最大行数(smallコメと同じになる)
  comments;
  //総合コメント
  //初期化
  constructor(ctx, canvasSize, lines, commentSize, commentSizeFixed, duration) {
    this.ctx = ctx;
    this.canvasSize = canvasSize;
    this.canvasWidthFlash = canvasSize.height / 3 * 4;
    this.lines = lines;
    this.fonrSize = commentSize;
    this.fixedFonrSize = commentSizeFixed;
    this.duration = duration;
    this.comments = [];
    this.maxlines = lines.small;
    this.naka = new NakaLine(lines.small, canvasSize);
    this.shita = [];
    this.ue = [];
  }
  /**
   * レイヤーにコメントを追加する
   * @param text コメント本文
   * @param commentNumber コメ番
   * @param customAttr カスタム属性
   * @param type naka/shita/ueのいずれか
   * @param size big/medium/largeのいずれか
   * @param vpos VPOS
   */
  add(text, commentNumber, customAttr, type = "naka", size = "medium", callBack, vpos) {
    if (this.comments.length > 40)
      return;
    const options = {
      mode: type,
      color: customAttr.get("color") || "#fff",
      borderColor: customAttr.get("bcolor") || "#000",
      duration: this.duration,
      customAttr,
      commentSize: size,
      vpos,
      fontName: customAttr.get("fontName"),
      opacity: customAttr.get("opacity"),
      full: customAttr.get("full")
    };
    const param = {
      text,
      ctx: this.ctx,
      canvasSize: this.canvasSize,
      canvasWidthFlash: this.canvasWidthFlash,
      fontSize: this.fonrSize,
      fixedFontSize: this.fixedFonrSize,
      lines: this.lines,
      option: options,
      commentNumber,
      onDisposed: callBack.onDispased
    };
    const commentObj = new CommentBase(param);
    switch (commentObj.type) {
      case "naka":
        this.naka.add(commentObj);
        break;
      case "shita":
        this._appendShita(commentObj);
        break;
      case "ue":
        this._appendUe(commentObj);
        break;
    }
  }
  /**
   * 更新処理
   * @param counter ループ回数
   */
  tick(options) {
    const currentVpos = options ? options.vpos ? options.vpos : 0 : 0;
    const doRender = options ? options.render ? true : false : true;
    if (this.comments.length) {
      this.comments.forEach((comment) => {
        if (doRender)
          this._render(comment);
        comment.tick(currentVpos);
      });
    }
    if (doRender) {
      this.naka.getList().forEach((comment) => {
        this._render(comment);
      });
    }
    this.naka.tick(currentVpos);
    this._clean();
  }
  /**
   * 描写処理
   * @param comment コメント
   */
  _render(comment) {
    this.ctx.textBaseline = "top";
    switch (true) {
      case (comment.type === "naka" || comment.fixed):
        this.ctx.textAlign = "left";
        break;
      default:
        this.ctx.textAlign = "center";
        break;
    }
    this.ctx.fillStyle = `${comment.color}`;
    this.ctx.shadowColor = comment.borderColor;
    this.ctx.shadowOffsetX = 1.5;
    this.ctx.shadowOffsetY = 1.5;
    this.ctx.globalAlpha = comment.opacity;
    this.ctx.font = `bold ${comment.fontSize}px "${comment.fontName}"`;
    const deltaMinusOrPlus = comment.type === "shita" ? -1 : 1;
    const delta = deltaMinusOrPlus * comment.fontSize;
    for (let i = 0; i < comment.textForRender.length; i++) {
      this.ctx.fillText(
        comment.textForRender[i],
        comment.left,
        comment.top + delta * i
      );
    }
  }
  /**
   * コメントを削除する
   * @param  option 最初のコメントまたは、最後のコメントのみを削除することが出来ます。
   */
  clear(option) {
    switch (option) {
      case void 0:
        this.comments.forEach((comment) => comment.kill());
        this.comments = [];
        this.naka = new NakaLine(this.lines.small, this.canvasSize);
        this.ue = [];
        this.shita = [];
        break;
      case "first":
        if (this.comments.length)
          this.comments[0].kill();
        break;
      case "last":
        if (this.comments.length) {
          this.comments[this.comments.length - 1].kill();
        }
    }
  }
  /**
   * コメント配列からフラグが降りているコメントを削除
   */
  _clean() {
    this.comments = this.comments.filter((commet) => commet.alive);
    this.naka.clean();
  }
  /**
   * shitaコメントを追加する
   * @param comment コメントオブジェクト
   */
  _appendShita(comment) {
    let bottom = this.canvasSize.height;
    for (let i = 0; i < 40; i++) {
      if (this.shita[i] && this.shita[i].alive && !comment.fixed) {
        bottom -= this.shita[i].overallSize;
      }
      switch (true) {
        case this.shita.length === 0:
        case this.shita.length <= i:
        case !this.shita[i].alive:
          break;
        case comment.fixed:
          break;
        case bottom - comment.overallSize < 0:
          break;
        default:
          continue;
      }
      if (comment.fixed) {
        comment.top = this.canvasSize.height - comment.fontSize;
      } else if (bottom - comment.overallSize < 0) {
        comment.top = Math.random() * (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = bottom - comment.overallSize;
      }
      this.shita[i] = comment;
      this.comments.push(comment);
      break;
    }
  }
  /**
   * ueコメントを追加する
   * @param comment コメントオブジェクト
   */
  _appendUe(comment) {
    let top = 0;
    for (let i = 0; i < 40; i++) {
      if (this.ue[i] && this.ue[i].alive && !comment.fixed) {
        top += this.ue[i].overallSize;
      }
      switch (true) {
        case this.ue.length === 0:
        case this.ue.length <= i:
        case !this.ue[i].alive:
        case comment.fixed:
        case top + comment.overallSize > this.canvasSize.height:
          break;
        default:
          continue;
      }
      if (comment.fixed) {
        comment.top = 0;
      } else if (top + comment.overallSize > this.canvasSize.height) {
        comment.top = Math.random() * (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = top;
      }
      this.ue[i] = comment;
      this.comments.push(comment);
      break;
    }
  }
  /**
   * 指定した位置に存在するコメントの配列を取得します
   * @param x X座標
   * @param y Y座標
   */
  get(x3, y3) {
    const comments = [];
    const naka = this.naka.get(x3, y3);
    if (naka !== void 0) {
      comments.push(naka);
    }
    const ueshita = this.comments.filter((comment) => {
      const half = comment.width / 2;
      const isX = x3 > comment.left - half && x3 < comment.left + half;
      const isY = y3 > comment.top && y3 < comment.top + comment.overallSize;
      return isX && isY;
    });
    comments.push(...ueshita);
    return comments;
  }
};
var NakaLine = class {
  lastCommentStream;
  comments;
  canvasSize;
  allines;
  //全ての行数
  constructor(smallLines, size) {
    this.comments = [];
    this.lastCommentStream = [];
    this.allines = smallLines;
    this.canvasSize = size;
  }
  /**
   * nakaコメントを追加する
   * @param comment コメント
   */
  add(comment) {
    let top = 0;
    for (let i = 0; i < this.allines; i++) {
      const line = this.lastCommentStream[i];
      switch (true) {
        case line === void 0:
          break;
        case comment.fixed:
          break;
        case (line.reveal < 0 && line.life < comment.touch):
          break;
        case top + comment.overallSize > this.canvasSize.height:
          break;
        default:
          top += line.overallSize;
          continue;
      }
      if (comment.fixed) {
        comment.top = 0;
      } else if (top + comment.overallSize > this.canvasSize.height) {
        comment.top = Math.random() * (this.canvasSize.height - comment.overallSize);
      } else {
        comment.top = top;
      }
      this.comments.push(comment);
      this.lastCommentStream[i] = comment;
      break;
    }
  }
  /**
   * 流れたコメントを削除する
   */
  clean() {
    this.comments = this.comments.filter((comment) => comment.alive);
    this.lastCommentStream = this.lastCommentStream.filter((comment) => comment.alive);
  }
  /**
   * 更新処理
   */
  tick(vpos) {
    this.comments.forEach((comment) => {
      comment.tick(vpos);
    });
  }
  /**
   * コメントのリストを取得する
   */
  getList() {
    return this.comments;
  }
  /**
   * コメントを取得する
   * @param x X座標
   * @param y Y座標
   */
  get(x3, y3) {
    return this.comments.find((comment) => {
      const isX = x3 > comment.left && x3 < comment.left + comment.width;
      const isY = y3 > comment.top && y3 < comment.top + comment.overallSize;
      if (isX && isY) {
        return true;
      }
    });
  }
};

// NiconicomeWeb/src/watch/ui/comment/drawer/commentDrawer.ts
var NicommentJS = class {
  /**
   * canvasコンテキスト
   */
  ctx;
  /**
   * canvasサイズ
   */
  canvasSize;
  /**
   * メタ情報
   */
  meta;
  /**
   * コメントのサイズごとの行数
   */
  lines;
  /**
   * 固定コメントのサイズごとの行数
   */
  fixedLines;
  /**
   * フォントサイズ
   */
  fonrSize;
  /**
   * 固定コメントのフォントサイズ
   */
  fixedFonrSize;
  /**
   * コメント表示時間
   */
  duration;
  /**
   * 自動更新フラグ
   */
  autoTickDisabled;
  /**
   * メインレイヤー
   */
  mainLayerName;
  /**
   * 実行フラグ
   */
  run;
  /**
   * 再生フラグ
   */
  isPlay;
  /**
   * レイヤー
   */
  layers;
  /**
   * トータルコメ数
   */
  total;
  /**
   * デバッグモード
   */
  isDebug;
  /**
   * @param id canvasのID
   * @param width canvasの幅
   * @param height canvasの高さ
   * @param option オプション
   */
  constructor(id, width, height, option) {
    this.isDebug = option?.debug ?? false;
    this.checkArgs(id, width, height);
    this.ctx = this._getContext(id, width, height);
    this.canvasSize = new Size({ height, width });
    this.meta = new Meta();
    this.duration = option ? option.duration ? option.duration : config_default.duration : config_default.duration;
    this.mainLayerName = option ? option.layerName ? option.layerName : config_default.defaultLayer : config_default.defaultLayer;
    this.lines = {
      big: option ? option.bigLines ? option.bigLines : config_default.bigLines : config_default.bigLines,
      medium: option ? option.mediumLines ? option.mediumLines : config_default.mediumLines : config_default.mediumLines,
      small: option ? option.smallLines ? option.smallLines : config_default.smallLines : config_default.smallLines
    };
    this.fixedLines = {
      big: config_default.fix.big,
      medium: config_default.fix.medium,
      small: config_default.fix.smal
    };
    this.fonrSize = this._getFontSize(height, this.lines);
    this.fixedFonrSize = this._getFontSize(height, this.fixedLines);
    this.mainLayerName = option ? option.layerName ? option.layerName : config_default.defaultLayer : config_default.defaultLayer;
    this.layers = /* @__PURE__ */ new Map();
    this.addLayer(this.mainLayerName);
    this.total = 0;
    this.run = true;
    this.isPlay = true;
    this.autoTickDisabled = option ? option.autoTickDisabled ? option.autoTickDisabled : false : false;
    if (option?.debug) {
      Logger2.write(`canvasID:${id}, width:${width}px, height:${height}px`);
      Logger2.write("\u521D\u671F\u5316\u304C\u5B8C\u4E86\u3057\u307E\u3057\u305F\u3002");
    }
    if (!this.autoTickDisabled) {
      if (option?.debug)
        Logger2.write("\u66F4\u65B0\u51E6\u7406\u3092\u958B\u59CB\u3057\u307E\u3059\u3002");
      this.tick();
    }
  }
  /**
   * 引数チェック
   * @param id id
   * @param width width
   * @param height height
   */
  checkArgs(id, width, height) {
    if (!id)
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.ID);
    if (!width) {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.WIDTH);
    }
    if (!height) {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NOT_EXIST.HEIGHT);
    }
    if (typeof width !== "number") {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NaN.WIDTH(width));
    }
    if (typeof height !== "number") {
      throw new Error(NicoExceptions.__INIT__.ARGUMENTS.NaN.HEIGHT(height));
    }
  }
  /**
   * コメントを追加する
   * @param text 表示するコメント
   * @param option オプション。;
   */
  send(text, option) {
    let customAttr = /* @__PURE__ */ new Map();
    const layer = option ? option.layer ? option.layer : this.mainLayerName : this.mainLayerName;
    const comType = option ? option.type ? option.type : "naka" : "naka";
    const comSize = option ? option.size ? option.size : "medium" : "medium";
    const color = option ? option.color ? option.color : "#fff" : "#fff";
    const bcolor = this.getBcolor(color);
    const vpos = option ? option.vpos ? option.vpos : 0 : 0;
    const fontName = option ? option.fontName ? option.fontName : config_default.defaultFont : config_default.defaultFont;
    const opacity = option ? option.opacity ? option.opacity : config_default.opacity : config_default.opacity;
    const full = option ? option.full ? option.full : false : false;
    const onDisposed = option ? option.onDisposed ? option.onDisposed : () => {
    } : () => {
    };
    this.total++;
    if (option !== void 0) {
      if (option.customAttr !== void 0) {
        customAttr = this._getAttr(option.customAttr);
      }
    }
    customAttr.set("color", color);
    customAttr.set("bcolor", bcolor);
    customAttr.set("fontName", fontName);
    customAttr.set("opacity", opacity);
    customAttr.set("full", full);
    const layerObj = this.layers.get(layer);
    if (layerObj !== void 0) {
      layerObj.add(text, this.total, customAttr, comType, comSize, {
        onDispased: onDisposed
      }, vpos);
    } else {
      throw new Error(NicoExceptions.LAYER.LAYER_DOES_NOT_EXIST(layer));
    }
    if (this.isDebug) {
      Logger2.write(`\u30B3\u30E1\u30F3\u30C8\u3092\u8FFD\u52A0\u3057\u307E\u3057\u305F\u3002(${text})`);
    }
  }
  /**
   * 一時停止
   */
  pause() {
    if (this.isDebug) {
      Logger2.write("\u4E00\u6642\u505C\u6B62");
    }
    this.isPlay = false;
  }
  /**
   * 再生
   */
  play() {
    if (this.isDebug) {
      Logger2.write("\u518D\u751F");
    }
    this.isPlay = true;
  }
  /**
   * コメントを削除します
   * @param layer レイヤー
   */
  clear(layer) {
    if (layer) {
      const layerObj = this.layers.get(layer);
      if (layerObj !== void 0) {
        layerObj.clear();
      } else {
        throw new Error(NicoExceptions.LAYER.LAYER_DOES_NOT_EXIST(layer));
      }
    } else {
      this.layers.forEach((layer2) => {
        layer2.clear();
      });
    }
  }
  /**
   * 全ての処理を終了します
   */
  dispose() {
    this.run = false;
    this.isPlay = false;
    this.layers.forEach((layer) => {
      layer.clear();
    });
    this.ctx.clearRect(0, 0, this.canvasSize.width, this.canvasSize.height);
    this.layers.clear();
    if (this.isDebug) {
      Logger2.write("\u5168\u3066\u306E\u51E6\u7406\u3092\u7D42\u4E86\u3057\u307E\u3057\u305F\u3002");
    }
  }
  /**
   * 属性を取得します
   * @param customAttr カスタム属性
   */
  _getAttr(customAttr) {
    const mapobj = /* @__PURE__ */ new Map();
    for (const [key, value] of Object.entries(customAttr)) {
      mapobj.set(key, value);
    }
    return mapobj;
  }
  _getFontSize(height, lines) {
    const big = height / lines.big;
    const medium = height / lines.medium;
    const small = height / lines.small;
    return { big, medium, small };
  }
  /**
   * canvasコンテキストを取得します
   * @param id ID
   * @param width 横幅
   * @param height 高さ
   */
  _getContext(id, width, height) {
    const elm = document.getElementById(
      id
    );
    if (!elm) {
      throw new Error(NicoExceptions.__INIT__.ELEMENT.NOT_EXIST(id));
    } else {
      elm.height = height;
      elm.width = width;
      elm.style.width = `${width}px`;
      elm.style.height = `${height}px`;
      const ctx = elm.getContext("2d");
      if (ctx !== null) {
        return ctx;
      } else {
        throw new Error();
      }
    }
  }
  /**
   * メインループ
   */
  tick(options) {
    if (this.isPlay) {
      const doRender = options ? options.render ? true : false : true;
      const currentVpos = options ? options.vpos ? options.vpos : 0 : 0;
      if (doRender) {
        this.ctx.clearRect(0, 0, this.canvasSize.width, this.canvasSize.height);
      }
      this.layers.forEach((layer) => {
        layer.tick({
          vpos: currentVpos,
          render: doRender
        });
      });
      this.meta.loop();
    }
    if (this.run && !this.autoTickDisabled) {
      requestAnimationFrame(() => {
        this.tick(options);
      });
    }
  }
  /**
   * 縁取り色を取得します
   * @param color 色
   */
  getBcolor(color) {
    switch (color) {
      case "black":
      case "#000":
      case "#000000":
        return "#fff";
      default:
        return "#000";
    }
  }
  /**
   * 指定した位置に存在するコメントを取得します
   * @param x X座標
   * @param y Y座標
   */
  get(x3, y3) {
    const comments = [];
    this.layers.forEach((layer) => {
      comments.push(...layer.get(x3, y3));
    });
    return comments;
  }
  /**
   * レイヤーを追加します
   * @param layerName レイヤー名
   */
  addLayer(layerName) {
    if (this.layers.has(layerName)) {
      throw new Error(NicoExceptions.LAYER.DUPLICATION(layerName));
    } else {
      this.layers.set(
        layerName,
        new Layer(
          this.ctx,
          this.canvasSize,
          this.lines,
          this.fonrSize,
          this.fixedFonrSize,
          this.duration
        )
      );
    }
    if (this.isDebug) {
      Logger2.write(`\u30EC\u30A4\u30E4\u30FC "${layerName}" \u3092\u8FFD\u52A0\u3057\u307E\u3057\u305F\u3002`);
    }
  }
  /**
   * レイヤーを削除します
   * @param layerName レイヤー名
   */
  removeLayer(layerName) {
    if (this.layers.has(layerName)) {
      throw new Error(NicoExceptions.LAYER.DUPLICATION(layerName));
    } else {
      this.layers.delete(layerName);
    }
    if (this.isDebug) {
      Logger2.write(`\u30EC\u30A4\u30E4\u30FC "${layerName}" \u3092\u524A\u9664\u3057\u307E\u3057\u305F\u3002`);
    }
  }
};
var Meta = class {
  /**
   * ループ回数
   */
  count;
  /**
   * 初期化
   */
  constructor() {
    this.count = 0;
  }
  /**
   * カウントを増やします
   */
  loop() {
    this.count++;
  }
  /**
   * カウントを取得
   */
  getCount() {
    return this.count;
  }
};
var Logger2 = class {
  /**
   * ログを出力する
   * @param log 本文
   */
  static write(log) {
    console.log(`[NicommentJS]${log}`);
  }
};
var NicoExceptions = {
  /**
   * 初期化エラー
   */
  __INIT__: {
    /**
     * 引数エラー
     */
    ARGUMENTS: {
      /**
       * 必要な引数が存在しない
       */
      NOT_EXIST: {
        /**
         * 高さ
         */
        HEIGHT: '[ERR]argument "height" must be specified.',
        /**
         * 横幅
         */
        WIDTH: '[ERR]argument "width" must be specified.',
        /**
         * ID
         */
        ID: '[ERR]argument "id" must be specified.'
      },
      /**
       * 引数の値が不適切である
       */
      NaN: {
        /**
         * 高さが数字でない
         */
        HEIGHT: (value) => `[ERR]${value} is not a number. "height" must be a number.`,
        /**
         * 横幅が数字でない
         */
        WIDTH: (value) => `[ERR]${value} is not a number. "width" must be a number.`
      }
    },
    /**
     * 要素が存在しない・canvasでない
     */
    ELEMENT: {
      /**
       * 要素が存在しない
       */
      NOT_EXIST: (id) => {
        return `[ERR]Canvas Element which id is "${id}" was not found.`;
      },
      /**
       * 要素がHTMLCanvasでない
       */
      NOT_A_CANVAS_ELEMENT: (id) => {
        return `[ERR]Element which id is "${id}" is not a canvasHTML5 Element.`;
      }
    }
  },
  /**
   * コメント追加時のエラー
   */
  SEND: {},
  /**
   * レイヤー関係のエラー
   */
  LAYER: {
    /**
     * 重複
     */
    DUPLICATION: (name) => {
      return `[ERR]The layer name ${name} already exists.`;
    },
    /**
     * レイヤーが存在しない
     */
    LAYER_DOES_NOT_EXIST: (name) => {
      return `[ERR]A layer name is ${name} does not exist.`;
    }
  }
};

// NiconicomeWeb/src/watch/ui/comment/manager/commentManager.ts
var CommentManagerImpl = class {
  constructor(video, commentDrawer) {
    this._video = video;
    this._commentDrawer = commentDrawer;
    this.initialize();
  }
  _commentDrawer;
  _video;
  _comments = [];
  _isPlaying = false;
  _isMainloopRunning = false;
  _isDisposed = false;
  _currentID = "";
  get currentID() {
    return this._currentID;
  }
  _commentAddedListener;
  initialize() {
    this._video.on("play", this.onPlay.bind(this));
    this._video.on("pause", this.onPause.bind(this));
    this._video.on("seeked", this.onSeek.bind(this));
  }
  load(comments, duration, niconicoID) {
    this._currentID = niconicoID;
    this._isPlaying = !this._video.paused;
    this._comments = [];
    const blockCount = Math.ceil(duration / 10) + 1;
    for (let i = 0; i < blockCount; i++) {
      this._comments.push([]);
    }
    comments.forEach((comment) => {
      const blockIndex = Math.floor(comment.vposMS / 1e3 / 10);
      if (blockIndex + 1 >= blockCount)
        return;
      if (this._comments[blockIndex] === void 0) {
        this._comments[blockIndex] = [];
      }
      this._comments[blockIndex].push(comment);
    });
  }
  start() {
    if (this._isMainloopRunning)
      return;
    this._isPlaying = true;
    this._isMainloopRunning = true;
    this._commentDrawer.play();
    this.mainloop();
  }
  dispose() {
    this._isDisposed = true;
    this._isPlaying = false;
    this._commentDrawer.dispose();
    this._video.off("play", this.onPlay);
    this._video.off("pause", this.onPause);
    this._video.off("seeked", this.onSeek);
  }
  on(event, listener) {
    switch (event) {
      case "commentAdded":
        this._commentAddedListener = listener;
        break;
    }
  }
  mainloop() {
    if (this._isDisposed)
      return;
    if (!this._isPlaying)
      return;
    const currentTime = this._video.currentTime;
    const blockIndex = Math.floor(currentTime / 10);
    const vpos = currentTime * 1e3;
    this._comments[blockIndex].forEach((comment) => {
      if (!comment.isAdded && comment.vposMS < vpos + 500 && comment.vposMS > vpos - 500) {
        comment.isAdded = true;
        this._commentDrawer.send(comment.body, {
          vpos: comment.vposMS,
          color: comment.color,
          type: comment.type
        });
        if (this._commentAddedListener) {
          this._commentAddedListener(comment);
        }
      }
    });
    this._commentDrawer.tick({ vpos, render: true });
    requestAnimationFrame(() => {
      if (this._isDisposed || !this._isPlaying)
        return;
      this.mainloop();
    });
  }
  onPlay() {
    this.start();
  }
  onPause() {
    this._isPlaying = false;
    this._isMainloopRunning = false;
    this._commentDrawer.pause();
  }
  onSeek() {
    this._comments.forEach((block) => {
      block.forEach((comment) => {
        comment.isAdded = false;
      });
    });
  }
};

// NiconicomeWeb/src/watch/ui/comment/fetch/commentFetcher.ts
var CommentFetcherImpl = class {
  _cache = {};
  async getComments(url) {
    if (this._cache !== void 0 && this._cache.url === url) {
      return AttemptResultWidthDataImpl.Succeeded(
        this._cache.comments
      );
    }
    const startTime = Date.now();
    Logger.log(`\u30B3\u30E1\u30F3\u30C8\u3092\u53D6\u5F97\u3057\u307E\u3059\u3002(${url})`);
    let response;
    try {
      response = await fetch(url);
    } catch {
      Logger.error(`\u30CD\u30C3\u30C8\u30EF\u30FC\u30AF\u30A8\u30E9\u30FC\u306B\u3088\u308A\u30B3\u30E1\u30F3\u30C8\u306E\u53D6\u5F97\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002`);
      return AttemptResultWidthDataImpl.Fail(
        `\u30B3\u30E1\u30F3\u30C8\u306E\u53D6\u5F97\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002`
      );
    }
    if (!response.ok) {
      const message = await response.text();
      Logger.error(`\u30B3\u30E1\u30F3\u30C8\u306E\u53D6\u5F97\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002 ${message}`);
      return AttemptResultWidthDataImpl.Fail(
        `\u30B3\u30E1\u30F3\u30C8\u306E\u53D6\u5F97\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002 ${message}`
      );
    }
    const data = await response.json();
    Logger.log(`\u30B3\u30E1\u30F3\u30C8\u3092\u53D6\u5F97\u3057\u307E\u3057\u305F\u3002(${Date.now() - startTime}ms)`);
    this._cache = {
      url,
      comments: data.comments.sort((a2, b) => a2.vposMS - b.vposMS).map((comment, i) => this.getManagedComment(comment, i))
    };
    return AttemptResultWidthDataImpl.Succeeded(
      this._cache.comments
    );
  }
  getManagedComment(comment, index) {
    let position = "naka";
    if (comment.mail.includes("ue")) {
      position = "ue";
    } else if (comment.mail.includes("shita")) {
      position = "shita";
    }
    let color = "#fff";
    if (comment.mail.includes("red")) {
      color = "#ff0000";
    } else if (comment.mail.includes("pink")) {
      color = "#ff8080";
    } else if (comment.mail.includes("orange")) {
      color = "#ffcc00";
    } else if (comment.mail.includes("yellow")) {
      color = "#ffff00";
    } else if (comment.mail.includes("green")) {
      color = "#00ff00";
    } else if (comment.mail.includes("cyan")) {
      color = "#00ffff";
    } else if (comment.mail.includes("blue")) {
      color = "#0000ff";
    } else if (comment.mail.includes("purple")) {
      color = "#8000ff";
    } else if (comment.mail.includes("black")) {
      color = "#000";
    }
    return {
      body: comment.body,
      isAdded: false,
      mail: comment.mail,
      number: comment.number,
      postedAt: comment.postedAt,
      type: position,
      userID: comment.userID,
      vposMS: comment.vposMS,
      color,
      innnerIndex: index
    };
  }
};
var CommentFetcherObj = new CommentFetcherImpl();

// NiconicomeWeb/src/watch/ui/hooks/useResize.ts
var useResizeHandled = () => {
  const [resizeHandled, setResizeHandled] = Ae(false);
  Te(() => {
    globalThis.addEventListener("resize", () => {
      setResizeHandled(false);
    });
  }, []);
  return [resizeHandled, setResizeHandled];
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoComment.tsx
var VideoComment = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const ref = qe(null);
  const fetching = qe(false);
  const rendered = qe(false);
  const [resizeHandled, setResizeHandled] = useResizeHandled();
  function sourceChanged() {
    if (state.jsWatchInfo === void 0)
      return false;
    if (state.comments === void 0)
      return true;
    return state.comments.niconicoID !== state.jsWatchInfo.video.niconicoID;
  }
  Te(() => {
    if (!sourceChanged() && resizeHandled && rendered.current)
      return;
    setResizeHandled(true);
    if (state.jsWatchInfo === void 0)
      return;
    if (state.commentManager !== void 0) {
      state.commentManager.dispose();
    }
    const jsWatchInfo = state.jsWatchInfo;
    if (fetching.current)
      return;
    if (sourceChanged()) {
      rendered.current = false;
      fetching.current = true;
      const fetcher = CommentFetcherObj;
      fetcher.getComments(jsWatchInfo.comment.contentUrl).then(
        (result) => {
          fetching.current = false;
          if (result.IsSucceeded && result.Data !== null) {
            const filtered = state.ngHandler.filterNG(result.Data).then(
              (comments) => {
                dispatch({
                  "type": "comments",
                  "payload": {
                    comments,
                    niconicoID: jsWatchInfo.video.niconicoID
                  }
                });
              }
            );
          } else {
            dispatch({
              "type": "comments",
              "payload": {
                comments: [],
                niconicoID: jsWatchInfo.video.niconicoID
              }
            });
          }
        }
      );
    }
    if (state.video === void 0 || state.comments === void 0)
      return;
    if (state.comments.niconicoID !== state.jsWatchInfo.video.niconicoID) {
      return;
    }
    if (ref.current === null)
      return;
    const drawer = new NicommentJS(
      "commentCanvas",
      ref.current.clientWidth,
      ref.current.clientHeight,
      {
        autoTickDisabled: true
      }
    );
    if (state.comments.comments.length > 0) {
      const manager = new CommentManagerImpl(state.video, drawer);
      dispatch({
        type: "commentManager",
        payload: manager
      });
      manager.load(
        state.comments.comments,
        state.video.duration,
        state.comments.niconicoID
      );
      if (!state.video.paused) {
        manager.start();
      }
      rendered.current = true;
    }
  });
  if (state.comments === void 0 || state.comments.comments.length === 0) {
    return /* @__PURE__ */ We.createElement("div", { className: "commentWrapper" });
  }
  return /* @__PURE__ */ We.createElement("div", { className: "commentWrapper", ref }, /* @__PURE__ */ We.createElement(
    "canvas",
    {
      id: "commentCanvas",
      className: `commentCanvas ${state.isCommentVisible ? "visible" : ""}`
    }
  ));
};

// NiconicomeWeb/src/shared/Extension/dateExtension.ts
Object.defineProperty(Date.prototype, "Format", {
  enumerable: false,
  configurable: true,
  writable: true,
  value: function(format) {
    const year = this.getFullYear();
    const month = (this.getMonth() + 1).toString().padStart(2, "0");
    const date = this.getDate().toString().padStart(2, "0");
    const hours = this.getHours().toString().padStart(2, "0");
    const minutes = this.getMinutes().toString().padStart(2, "0");
    const seconds = this.getSeconds().toString().padStart(2, "0");
    const milliseconds = this.getMilliseconds().toString().padStart(3, "0");
    const milliseconds2 = milliseconds.slice(0, 2);
    return format.replace("yyyy", year.toString()).replace("MM", month).replace("dd", date).replace("HH", hours).replace("mm", minutes).replace("ss", seconds).replace("SSS", milliseconds).replace("SS", milliseconds2);
  }
});

// NiconicomeWeb/src/watch/ui/componetnts/video/systemMessage/systemMessage.tsx
var SystemMessage = () => {
  const [message, setMessage] = We.useState(Logger.messages);
  const { state, dispatch } = Ie(VideoStateContext);
  const messageHandler = je((log) => {
    if (log === null) {
      setMessage([]);
      return;
    }
    setMessage([...message, log]);
  }, []);
  Te(() => {
    Logger.addEventListner("write", messageHandler);
    Logger.addEventListner("clear", messageHandler);
    return () => {
      Logger.removeEventListner("write", messageHandler);
      Logger.removeEventListner("clear", messageHandler);
    };
  }, []);
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `systemMessage ${state.isSystemMessageVisible ? "" : "hide"}`,
      onMouseDown: (e) => e.stopPropagation()
    },
    message.map((m3, i) => /* @__PURE__ */ We.createElement(
      "div",
      {
        key: i,
        className: `message ${m3.type} ${m3.border ? "messageBorder" : ""}`
      },
      /* @__PURE__ */ We.createElement("div", { className: "time" }, "[", m3.time.Format("yyyy/MM/dd mm:ss:SS"), "]"),
      /* @__PURE__ */ We.createElement("div", { className: "message" }, m3.message)
    ))
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/video/systemMessage/videoOverflow.tsx
var VideoOverflow = () => {
  const [contextOpen, setContextOpen] = Ae(false);
  const [position, setPosition] = Ae({ x: 0, y: 0 });
  const { state, dispatch } = Ie(VideoStateContext);
  const ref = qe(null);
  function onClick(e) {
    if (ref.current === null)
      return;
    if (e.button !== 2) {
      if (!state.isSystemMessageVisible && !state.contextMenu.open && state.video !== void 0) {
        if (state.video.paused) {
          state.video.play();
        } else {
          state.video.pause();
        }
      }
      dispatch({ type: "systemMessage", payload: false });
      return;
    }
    const left = e.clientX;
    const top = e.clientY;
    dispatch({ type: "contextMenu", payload: { open: true, left, top } });
  }
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "videoOverflow",
      onMouseDown: (e) => onClick(e),
      ref
    },
    /* @__PURE__ */ We.createElement(SystemMessage, null)
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/video/video.tsx
var VideoElement = ({ videoRef }) => {
  const initialized = qe(false);
  const currentNiconioID = qe("");
  const { state, dispatch } = Ie(VideoStateContext);
  Te(() => {
    if (state.jsWatchInfo === void 0)
      return;
    if (currentNiconioID.current === state.jsWatchInfo.video.niconicoID && initialized.current)
      return;
    let changeSource = false;
    if (currentNiconioID.current !== "" && currentNiconioID.current !== state.jsWatchInfo.video.niconicoID) {
      changeSource = true;
    }
    initialized.current = true;
    currentNiconioID.current = state.jsWatchInfo.video.niconicoID;
    if (!changeSource && videoRef.current !== null) {
      const video = new VideoImpl(
        videoRef.current
      );
      Logger.log("\u52D5\u753B\u306E\u8AAD\u307F\u8FBC\u307F\u3092\u958B\u59CB\u3057\u307E\u3059\u3002");
      const start = Date.now();
      video.Initialize(state.jsWatchInfo).then((result) => {
        if (!result.IsSucceeded) {
          Logger.error(result.Message ?? "\u52D5\u753B\u306E\u8AAD\u307F\u8FBC\u307F\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002");
        } else {
          Logger.log(
            `\u52D5\u753B\u3092\u8AAD\u307F\u8FBC\u307F\u307E\u3057\u305F\u3002(${Date.now() - start}ms, ${state.jsWatchInfo.media.contentUrl})`
          );
          video.on("pause", () => {
            dispatch({
              type: "isPlaying",
              payload: false
            });
          });
          video.on("play", () => {
            dispatch({
              type: "isPlaying",
              payload: true
            });
          });
          dispatch({
            type: "video",
            payload: video
          });
        }
      });
    } else {
      const video = state.video;
      if (video === void 0)
        return;
      if (videoRef.current === null)
        return;
      Logger.log("\u52D5\u753B\u306E\u8AAD\u307F\u8FBC\u307F\u3092\u958B\u59CB\u3057\u307E\u3059\u3002");
      const start = Date.now();
      video.Initialize(state.jsWatchInfo, videoRef.current).then((result) => {
        if (!result.IsSucceeded) {
          Logger.error(result.Message ?? "\u52D5\u753B\u306E\u8AAD\u307F\u8FBC\u307F\u306B\u5931\u6557\u3057\u307E\u3057\u305F\u3002");
        } else {
          Logger.log(
            `\u52D5\u753B\u3092\u8AAD\u307F\u8FBC\u307F\u307E\u3057\u305F\u3002(${Date.now() - start}ms, ${state.jsWatchInfo.media.contentUrl})`
          );
        }
      });
    }
  });
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "videoWrapper"
    },
    /* @__PURE__ */ We.createElement(
      "video",
      {
        ref: videoRef,
        id: "player",
        className: "videoElm",
        poster: state.jsWatchInfo?.thumbnail.contentUrl
      }
    ),
    /* @__PURE__ */ We.createElement(VideoComment, null),
    /* @__PURE__ */ We.createElement(VideoOverflow, null)
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/title.tsx
var Title = ({ title }) => {
  return /* @__PURE__ */ We.createElement("div", { className: "title" }, title);
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/owner.tsx
var Owner = ({ userID, userName }) => {
  const [userIcon, setUserIcon] = We.useState(
    "https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/defaults/blank.jpg"
  );
  Te(() => {
    const userIDNum = Number.parseInt(userID);
    const userIDDivided = Math.floor(userIDNum / 1e4);
    const userIconURL = `https://secure-dcdn.cdn.nimg.jp/nicoaccount/usericon/${userIDDivided}/${userIDNum}.jpg`;
    const controller = new AbortController();
    fetch(userIconURL, { signal: controller.signal }).then((res) => {
      if (res.ok) {
        setUserIcon(userIconURL);
      }
    });
    return () => {
      controller.abort();
    };
  }, [userID]);
  return /* @__PURE__ */ We.createElement("div", { className: "ownerWrapper" }, /* @__PURE__ */ We.createElement("a", { href: `https://nicovideo.jp/user/${userID}` }, /* @__PURE__ */ We.createElement("img", { src: userIcon, className: "ownerIcon" })), /* @__PURE__ */ We.createElement("a", { href: `https://nicovideo.jp/user/${userID}/video`, className: "ownerName" }, userName));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/tag.tsx
var Tags = ({ tags }) => {
  return /* @__PURE__ */ We.createElement("div", { className: "tagWrapper" }, tags.map((tag, index) => {
    const className = tag.isNicodicExists ? "nicodic exist" : "nicodic";
    const classNameIcon = tag.isNicodicExists ? "fa-solid fa-book fa-sm" : "fa-solid fa-circle-question fa-sm";
    return /* @__PURE__ */ We.createElement("div", { key: index, className: "tag" }, /* @__PURE__ */ We.createElement(
      "a",
      {
        className: "tagName",
        href: `https://nicovideo.jp/tag/${tag.name}`
      },
      tag.name
    ), /* @__PURE__ */ We.createElement("a", { href: `https://dic.nicovideo.jp/a/${tag.name}`, className }, /* @__PURE__ */ We.createElement("i", { className: classNameIcon })));
  }));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/description.tsx
var Description = ({ description }) => {
  const [isExpanded, setIsExpanded] = Ae(false);
  if (!isExpanded) {
    description = description.replace(/<.+?>/g, "");
  }
  return /* @__PURE__ */ We.createElement("div", { className: "descriptionWrapper" }, /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `description ${!isExpanded ? "collapsed" : ""}`,
      onClick: () => setIsExpanded(true),
      dangerouslySetInnerHTML: { __html: description }
    }
  ), /* @__PURE__ */ We.createElement(
    "span",
    {
      className: `expandButton ${!isExpanded ? "collapsed" : ""}`,
      onClick: () => setIsExpanded(!isExpanded)
    },
    isExpanded ? "\u25B2\u9589\u3058\u308B" : "\u25BC\u7D9A\u304D\u3092\u8AAD\u3080"
  ));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/majorInfo.tsx
var MajorInfo = ({ uploadedAt, count, niconicoID }) => {
  const formattedDate = `${uploadedAt.getFullYear()}/${(uploadedAt.getMonth() + 1).toString().padStart(2, "0")}/${uploadedAt.getDate().toString().padStart(2, "0")} ${uploadedAt.getHours().toString().padStart(2, "0")}:${uploadedAt.getMinutes().toString().padStart(2, "0")}`;
  let view;
  if (count.view > 1e4) {
    view = `${Math.floor(count.view / 1e3) / 10}\u4E07`;
  } else {
    view = count.view.toString();
  }
  let comment;
  if (count.comment > 1e4) {
    comment = `${Math.floor(count.comment / 1e3) / 10}\u4E07`;
  } else {
    comment = count.comment.toString();
  }
  let mylist;
  if (count.mylist > 1e4) {
    mylist = `${Math.floor(count.mylist / 1e3) / 10}\u4E07`;
  } else {
    mylist = count.mylist.toString();
  }
  let like;
  if (count.like > 1e4) {
    like = `${Math.floor(count.like / 1e3) / 10}\u4E07`;
  } else {
    like = count.like.toString();
  }
  return /* @__PURE__ */ We.createElement("div", { className: "majorInfoWrapper" }, /* @__PURE__ */ We.createElement("div", { className: "uploadedAt" }, /* @__PURE__ */ We.createElement("span", null, "\u6295\u7A3F\u65E5: ", formattedDate)), /* @__PURE__ */ We.createElement("div", { className: "counts" }, /* @__PURE__ */ We.createElement("span", null, /* @__PURE__ */ We.createElement("i", { className: "icon fa-solid fa-play" }), view), /* @__PURE__ */ We.createElement("span", null, /* @__PURE__ */ We.createElement("i", { className: "icon fa-solid fa-message" }), " ", comment), /* @__PURE__ */ We.createElement("span", null, /* @__PURE__ */ We.createElement("i", { className: "icon fa-solid fa-folder" }), mylist), /* @__PURE__ */ We.createElement("span", null, /* @__PURE__ */ We.createElement("i", { className: "icon like fa-solid fa-heart" }), " ", like), /* @__PURE__ */ We.createElement("a", { href: `https://nico.ms/${niconicoID}` }, /* @__PURE__ */ We.createElement("i", { className: "icon fa-solid fa-globe" }), "\u30CB\u30B3\u30CB\u30B3\u52D5\u753B")));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/videoInfo/videoInfo.tsx
var VideoInfo = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const jsWatchInfo = state.jsWatchInfo;
  if (jsWatchInfo === void 0)
    return /* @__PURE__ */ We.createElement("div", { className: "videoInfoWrapper" });
  return /* @__PURE__ */ We.createElement("div", { className: "videoInfoWrapper" }, /* @__PURE__ */ We.createElement("div", { className: "titleAndOwner" }, /* @__PURE__ */ We.createElement(Title, { title: jsWatchInfo.video.title }), /* @__PURE__ */ We.createElement(
    Owner,
    {
      userID: jsWatchInfo.video.owner.id,
      userName: jsWatchInfo.video.owner.name
    }
  )), /* @__PURE__ */ We.createElement(MajorInfo, { uploadedAt: new Date(jsWatchInfo.video.uploadedAt), count: jsWatchInfo.video.count, niconicoID: jsWatchInfo.video.niconicoID }), /* @__PURE__ */ We.createElement(Tags, { tags: jsWatchInfo.video.tags }), /* @__PURE__ */ We.createElement(Description, { description: jsWatchInfo.video.description }));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/slider.tsx
var Slider = ({ percentage, className }) => {
  return /* @__PURE__ */ We.createElement("div", { className: "slider" }, /* @__PURE__ */ We.createElement(
    "div",
    {
      className: `sliderInner ${className}`,
      style: { transform: `scaleX(${percentage})` }
    }
  ));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/handle.tsx
var Handle = ({ percentage }) => {
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "sliderHandle",
      style: { transform: `translateX(${percentage * 100}%)` }
    },
    /* @__PURE__ */ We.createElement("div", { className: "sliderHandleInner" })
  );
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/sliderWrapper.tsx
var SliderWrapper = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [timePercentage, setTimePercentage] = Ae(0);
  const [bufferPercentage, setBufferPercentage] = Ae(0);
  const wrapperRef = We.useRef(null);
  Te(() => {
    if (state.video === void 0 || state.jsWatchInfo === void 0)
      return;
    const video = state.video;
    const duration = state.jsWatchInfo.video.duration;
    const update = () => {
      const time = video.currentTime;
      if (video.buffered.length === 0)
        return;
      const buffer = video.buffered.end(video.buffered.length - 1);
      setTimePercentage(time / duration);
      setBufferPercentage(buffer / duration);
    };
    video.on("timeupdate", update);
    video.on("progress", update);
    return () => {
      video.off("timeupdate", update);
      video.off("progress", update);
    };
  });
  function handleClicked(e) {
    if (wrapperRef.current === null || state.video === void 0)
      return;
    const x3 = e.clientX - wrapperRef.current.getBoundingClientRect().left;
    const percentage = x3 / wrapperRef.current.clientWidth;
    const time = state.video.duration * percentage;
    state.video.currentTime = time;
  }
  return /* @__PURE__ */ We.createElement("div", { className: "sliderWrapper", ref: wrapperRef, onClick: handleClicked }, /* @__PURE__ */ We.createElement(Slider, { percentage: bufferPercentage, className: "buffer" }), /* @__PURE__ */ We.createElement(Slider, { percentage: timePercentage, className: "time" }), /* @__PURE__ */ We.createElement(Handle, { percentage: timePercentage }));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/playCOntroler.tsx
var PlayControler = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  function switchPlay() {
    if (state.isPlaying) {
      state.video?.pause();
    } else {
      state.video?.play();
    }
  }
  return /* @__PURE__ */ We.createElement("div", { className: "playControler" }, /* @__PURE__ */ We.createElement("div", { className: "playButton", onClick: () => switchPlay() }, /* @__PURE__ */ We.createElement("i", { className: `fa-solid fa-lg ${state.isPlaying ? "fa-pause" : "fa-play"}` })));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/timeControler.tsx
var TimeControler = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [currentTime, setCurrentTime] = Ae("00:00");
  const ref = qe(null);
  const duration = Fe(() => {
    if (state.video === void 0)
      return "00:00";
    const duration2 = state.video.duration;
    if (isNaN(duration2))
      return "00:00";
    if (duration2 > 3600) {
      const hour = String(Math.floor(duration2 / 3600)).padStart(2, "0");
      const minute = String(Math.floor(duration2 % 3600 / 60)).padStart(2, "0");
      const second = String(Math.floor(duration2 % 60)).padStart(2, "0");
      return `${hour}:${minute}:${second}`;
    } else {
      const minute = String(Math.floor(duration2 / 60)).padStart(2, "0");
      const second = String(Math.floor(duration2 % 60)).padStart(2, "0");
      return `${minute}:${second}`;
    }
  }, [state.video?.duration]);
  Te(() => {
    if (state.video === void 0)
      return;
    const video = state.video;
    const onTimeUpdate = () => {
      if (video === void 0)
        return;
      const currentTime2 = video.currentTime;
      if (video.duration > 3600) {
        const hour = String(Math.floor(currentTime2 / 3600)).padStart(2, "0");
        const minute = String(Math.floor(currentTime2 % 3600 / 60)).padStart(
          2,
          "0"
        );
        const second = String(Math.floor(currentTime2 % 60)).padStart(2, "0");
        setCurrentTime(`${hour}:${minute}:${second}`);
      } else {
        const minute = String(Math.floor(currentTime2 / 60)).padStart(2, "0");
        const second = String(Math.floor(currentTime2 % 60)).padStart(2, "0");
        setCurrentTime(`${minute}:${second}`);
      }
    };
    video.on("timeupdate", onTimeUpdate);
    return () => {
      video.off("timeupdate", onTimeUpdate);
    };
  }, [state.video]);
  function handleSkip(isForward) {
    if (state.video === void 0)
      return;
    const video = state.video;
    const currentTime2 = video.currentTime;
    const skipTime = 10;
    if (isForward) {
      video.currentTime = currentTime2 + skipTime;
    } else {
      video.currentTime = currentTime2 - skipTime;
    }
  }
  return /* @__PURE__ */ We.createElement("div", { className: "timeControler" }, /* @__PURE__ */ We.createElement("p", { onClick: () => handleSkip(false) }, /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-arrow-rotate-left fa-lg" })), /* @__PURE__ */ We.createElement("p", { className: "time" }, currentTime, " / ", duration), /* @__PURE__ */ We.createElement("p", { onClick: () => handleSkip(true) }, /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-arrow-rotate-right fa-lg" })));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/generalControler.tsx
var GeneralControler = () => {
  const { state, dispatch } = Ie(VideoStateContext);
  const [repeat, setRepeat] = Ae(state.video?.repeat ?? false);
  function toggleComment() {
    dispatch({
      type: "isCommentVisible",
      payload: !state.isCommentVisible
    });
  }
  function toggleRepeat() {
    if (state.video === void 0)
      return;
    state.video.repeat = !state.video.repeat;
    setRepeat(state.video.repeat);
  }
  return /* @__PURE__ */ We.createElement("div", { className: "generalControler" }, /* @__PURE__ */ We.createElement(
    "p",
    {
      className: `repeat ${repeat ? "enable" : ""}`,
      onClick: toggleRepeat
    },
    /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-repeat fa-lg" })
  ), /* @__PURE__ */ We.createElement(
    "p",
    {
      className: `commentVisibility ${state.isCommentVisible ? "visible" : ""}`,
      onClick: toggleComment
    },
    /* @__PURE__ */ We.createElement("i", { className: "fa-solid fa-message fa-lg" })
  ));
};

// NiconicomeWeb/src/watch/ui/componetnts/video/controler/controler.tsx
var Controler = () => {
  return /* @__PURE__ */ We.createElement("div", { className: "controler" }, /* @__PURE__ */ We.createElement(SliderWrapper, null), /* @__PURE__ */ We.createElement("div", { className: "panel" }, /* @__PURE__ */ We.createElement(PlayControler, null), /* @__PURE__ */ We.createElement(TimeControler, null), /* @__PURE__ */ We.createElement(GeneralControler, null)));
};

// NiconicomeWeb/src/watch/ui/video/videoEventHandler.ts
function handleEvent(e, param) {
  const { state, dispatch } = param;
  if (state.video === void 0)
    return;
  console.log(e.key);
  if (e.key === " " || e.key === "k") {
    e.preventDefault();
    state.video.paused ? state.video.play() : state.video.pause();
  } else if (e.key === "c") {
    e.preventDefault();
    dispatch({ type: "isCommentVisible", payload: !state.isCommentVisible });
  } else if (e.key === "ArrowRight" || e.key === "l") {
    e.preventDefault();
    if (state.video.currentTime + 10 < state.video.duration) {
      state.video.currentTime += 10;
    } else {
      state.video.currentTime = state.video.duration;
    }
  } else if (e.key === "ArrowLeft" || e.key === "j") {
    e.preventDefault();
    if (state.video.currentTime - 10 > 0) {
      state.video.currentTime -= 10;
    } else {
      state.video.currentTime = 0;
    }
  } else if (e.key === "Home") {
    e.preventDefault();
    state.video.currentTime = 0;
  }
}

// NiconicomeWeb/src/watch/ui/leftContent.tsx
var LeftContent = () => {
  const videoRef = qe(null);
  const { state, dispatch } = Ie(VideoStateContext);
  return /* @__PURE__ */ We.createElement(
    "div",
    {
      className: "leftContent",
      tabIndex: -1,
      onKeyDown: (e) => handleEvent(e, { state, dispatch })
    },
    /* @__PURE__ */ We.createElement(VideoElement, { videoRef }),
    /* @__PURE__ */ We.createElement(Controler, null),
    /* @__PURE__ */ We.createElement(VideoInfo, null)
  );
};

// NiconicomeWeb/src/watch/ui/main.tsx
function main() {
  console.log(true);
  const element = document.querySelector("#watchApp");
  const handler = new JsWatchInfoHandlerImpl();
  const watchInfo = handler.getData();
  if (element !== null && watchInfo !== null) {
    O3(element).render(
      /* @__PURE__ */ We.createElement(Main, { jsWatchInfo: watchInfo })
    );
  }
}
var Main = ({ jsWatchInfo }) => {
  const [state, dispatch] = Ue(reduceFunc, {
    ...InitialData,
    jsWatchInfo,
    ngHandler: new NGHandlerImpl(
      new NGDataFetcherImpl(jsWatchInfo.comment.commentNGAPIBaseUrl)
    )
  });
  return /* @__PURE__ */ We.createElement(VideoStateContext.Provider, { value: { state, dispatch } }, state.contextMenu.open ? /* @__PURE__ */ We.createElement(ContextMenu, null) : void 0, /* @__PURE__ */ We.createElement("div", { className: "watchWrapper" }, /* @__PURE__ */ We.createElement(LeftContent, null), /* @__PURE__ */ We.createElement("div", { className: "rightContent" }, /* @__PURE__ */ We.createElement(Comment, null), /* @__PURE__ */ We.createElement(Shortcut, null), /* @__PURE__ */ We.createElement(
    Playlist,
    {
      videos: jsWatchInfo.playlistVideos,
      baseURL: jsWatchInfo.api.baseUrl
    }
  ))));
};
export {
  main
};
/*! Bundled license information:

react/cjs/react.production.min.js:
  (**
   * @license React
   * react.production.min.js
   *
   * Copyright (c) Facebook, Inc. and its affiliates.
   *
   * This source code is licensed under the MIT license found in the
   * LICENSE file in the root directory of this source tree.
   *)
*/
/*! Bundled license information:

scheduler/cjs/scheduler.production.min.js:
  (**
   * @license React
   * scheduler.production.min.js
   *
   * Copyright (c) Facebook, Inc. and its affiliates.
   *
   * This source code is licensed under the MIT license found in the
   * LICENSE file in the root directory of this source tree.
   *)
*/
/*! Bundled license information:

react-dom/cjs/react-dom.production.min.js:
  (**
   * @license React
   * react-dom.production.min.js
   *
   * Copyright (c) Facebook, Inc. and its affiliates.
   *
   * This source code is licensed under the MIT license found in the
   * LICENSE file in the root directory of this source tree.
   *)
*/