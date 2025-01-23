
import type { Mapping, RemapInput, PatchUrlMappingsConfig, MatchAndRewriteURLInputs } from "../official_types";

/**
 * Exposes utils from the Embedded App SDK.
 */
export class OfficialUtils {

  formatPrice(price: {amount: number; currency: string}, locale: string = 'en-US'): string {
    const {amount, currency} = price;
    const formatter = Intl.NumberFormat(locale, {style: 'currency', currency});
    return formatter.format(convertToMajorCurrencyUnits(amount, currency as CurrencyCodes));
  }

  patchUrlMappings(
    mappings: Mapping[],
    {patchFetch = true, patchWebSocket = true, patchXhr = true, patchSrcAttributes = false}: PatchUrlMappingsConfig = {},
  ) {
    // Bail out if we're not in a browser
    if (typeof window === 'undefined') return;
  
    if (patchFetch) {
      const fetchImpl = window.fetch;
      // fetch is a duplex, but this is consistent
      window.fetch = function (input: RequestInfo | URL | Request | any, init?: RequestInit) {
        // If fetch has Request as input, we need to resolve any stream
        // before we create a new request with the mapped url
        if (input instanceof Request) {
          const newUrl = attemptRemap({url: absoluteURL(input.url), mappings});
          // eslint-disable-next-line @typescript-eslint/no-unused-vars
          const {url, ...newInit} = (init ?? {}) as RequestInit & {url: any};
          Object.keys(Request.prototype).forEach((value) => {
            if (value === 'url') return;
            try {
              // @ts-expect-error
              newInit[value] = input[value];
            } catch (ex) {
              console.warn(`Remapping fetch request key "${value}" failed`, ex);
            }
          });
  
          return new Promise((resolve, reject) => {
            try {
              input.blob().then((blob) => {
                if (input.method.toUpperCase() !== 'HEAD' && input.method.toUpperCase() !== 'GET' && blob.size > 0) {
                  newInit.body = blob;
                }
  
                resolve(fetchImpl(new Request(newUrl, newInit)));
              });
            } catch (ex) {
              reject(ex);
            }
          });
        }
  
        // Assuming a generic url or string
        const remapped = attemptRemap({url: input instanceof URL ? input : absoluteURL(input), mappings});
        return fetchImpl(remapped, init);
      };
    }
    if (patchWebSocket) {
      class WebSocketProxy extends WebSocket {
        constructor(url: string | URL, protocols?: string | string[]) {
          const remapped = attemptRemap({url: url instanceof URL ? url : absoluteURL(url), mappings});
          super(remapped, protocols);
        }
      }
      window.WebSocket = WebSocketProxy;
    }
  
    if (patchXhr) {
      const openImpl = XMLHttpRequest.prototype.open;
      // @ts-expect-error - the ts interface exports two 'open' methods
      XMLHttpRequest.prototype.open = function (
        method: string,
        url: string,
        async: boolean,
        username?: string | null,
        password?: string | null,
      ) {
        const remapped = attemptRemap({url: absoluteURL(url), mappings});
        openImpl.apply(this, [method, remapped, async, username, password]);
      };
    }
  
    if (patchSrcAttributes) {
      const callback: MutationCallback = function (mutationsList) {
        for (const mutation of mutationsList) {
          if (mutation.type === 'attributes' && mutation.attributeName === 'src') {
            attemptSetNodeSrc(mutation.target, mappings);
          } else if (mutation.type === 'childList') {
            mutation.addedNodes.forEach((node) => {
              attemptSetNodeSrc(node, mappings);
              recursivelyRemapChildNodes(node, mappings);
            });
          }
        }
      };
  
      const observer = new MutationObserver(callback);
      const config: MutationObserverInit = {
        attributeFilter: ['src'],
        childList: true,
        subtree: true,
      };
      observer.observe(window.document, config);
  
      window.document.querySelectorAll('[src]').forEach((node) => {
        attemptSetNodeSrc(node, mappings);
      });
    }
  }
}


//# FORMAT PRICE - - - - -
enum CurrencyCodes {
  AED = 'aed',
  AFN = 'afn',
  ALL = 'all',
  AMD = 'amd',
  ANG = 'ang',
  AOA = 'aoa',
  ARS = 'ars',
  AUD = 'aud',
  AWG = 'awg',
  AZN = 'azn',
  BAM = 'bam',
  BBD = 'bbd',
  BDT = 'bdt',
  BGN = 'bgn',
  BHD = 'bhd',
  BIF = 'bif',
  BMD = 'bmd',
  BND = 'bnd',
  BOB = 'bob',
  BOV = 'bov',
  BRL = 'brl',
  BSD = 'bsd',
  BTN = 'btn',
  BWP = 'bwp',
  BYN = 'byn',
  BYR = 'byr',
  BZD = 'bzd',
  CAD = 'cad',
  CDF = 'cdf',
  CHE = 'che',
  CHF = 'chf',
  CHW = 'chw',
  CLF = 'clf',
  CLP = 'clp',
  CNY = 'cny',
  COP = 'cop',
  COU = 'cou',
  CRC = 'crc',
  CUC = 'cuc',
  CUP = 'cup',
  CVE = 'cve',
  CZK = 'czk',
  DJF = 'djf',
  DKK = 'dkk',
  DOP = 'dop',
  DZD = 'dzd',
  EGP = 'egp',
  ERN = 'ern',
  ETB = 'etb',
  EUR = 'eur',
  FJD = 'fjd',
  FKP = 'fkp',
  GBP = 'gbp',
  GEL = 'gel',
  GHS = 'ghs',
  GIP = 'gip',
  GMD = 'gmd',
  GNF = 'gnf',
  GTQ = 'gtq',
  GYD = 'gyd',
  HKD = 'hkd',
  HNL = 'hnl',
  HRK = 'hrk',
  HTG = 'htg',
  HUF = 'huf',
  IDR = 'idr',
  ILS = 'ils',
  INR = 'inr',
  IQD = 'iqd',
  IRR = 'irr',
  ISK = 'isk',
  JMD = 'jmd',
  JOD = 'jod',
  JPY = 'jpy',
  KES = 'kes',
  KGS = 'kgs',
  KHR = 'khr',
  KMF = 'kmf',
  KPW = 'kpw',
  KRW = 'krw',
  KWD = 'kwd',
  KYD = 'kyd',
  KZT = 'kzt',
  LAK = 'lak',
  LBP = 'lbp',
  LKR = 'lkr',
  LRD = 'lrd',
  LSL = 'lsl',
  LTL = 'ltl',
  LVL = 'lvl',
  LYD = 'lyd',
  MAD = 'mad',
  MDL = 'mdl',
  MGA = 'mga',
  MKD = 'mkd',
  MMK = 'mmk',
  MNT = 'mnt',
  MOP = 'mop',
  MRO = 'mro',
  MUR = 'mur',
  MVR = 'mvr',
  MWK = 'mwk',
  MXN = 'mxn',
  MXV = 'mxv',
  MYR = 'myr',
  MZN = 'mzn',
  NAD = 'nad',
  NGN = 'ngn',
  NIO = 'nio',
  NOK = 'nok',
  NPR = 'npr',
  NZD = 'nzd',
  OMR = 'omr',
  PAB = 'pab',
  PEN = 'pen',
  PGK = 'pgk',
  PHP = 'php',
  PKR = 'pkr',
  PLN = 'pln',
  PYG = 'pyg',
  QAR = 'qar',
  RON = 'ron',
  RSD = 'rsd',
  RUB = 'rub',
  RWF = 'rwf',
  SAR = 'sar',
  SBD = 'sbd',
  SCR = 'scr',
  SDG = 'sdg',
  SEK = 'sek',
  SGD = 'sgd',
  SHP = 'shp',
  SLL = 'sll',
  SOS = 'sos',
  SRD = 'srd',
  SSP = 'ssp',
  STD = 'std',
  SVC = 'svc',
  SYP = 'syp',
  SZL = 'szl',
  THB = 'thb',
  TJS = 'tjs',
  TMT = 'tmt',
  TND = 'tnd',
  TOP = 'top',
  TRY = 'try',
  TTD = 'ttd',
  TWD = 'twd',
  TZS = 'tzs',
  UAH = 'uah',
  UGX = 'ugx',
  USD = 'usd',
  USN = 'usn',
  USS = 'uss',
  UYI = 'uyi',
  UYU = 'uyu',
  UZS = 'uzs',
  VEF = 'vef',
  VND = 'vnd',
  VUV = 'vuv',
  WST = 'wst',
  XAF = 'xaf',
  XAG = 'xag',
  XAU = 'xau',
  XBA = 'xba',
  XBB = 'xbb',
  XBC = 'xbc',
  XBD = 'xbd',
  XCD = 'xcd',
  XDR = 'xdr',
  XFU = 'xfu',
  XOF = 'xof',
  XPD = 'xpd',
  XPF = 'xpf',
  XPT = 'xpt',
  XSU = 'xsu',
  XTS = 'xts',
  XUA = 'xua',
  YER = 'yer',
  ZAR = 'zar',
  ZMW = 'zmw',
  ZWL = 'zwl',
};

const CurrencyExponents = {
  [CurrencyCodes.AED]: 2,
  [CurrencyCodes.AFN]: 2,
  [CurrencyCodes.ALL]: 2,
  [CurrencyCodes.AMD]: 2,
  [CurrencyCodes.ANG]: 2,
  [CurrencyCodes.AOA]: 2,
  [CurrencyCodes.ARS]: 2,
  [CurrencyCodes.AUD]: 2,
  [CurrencyCodes.AWG]: 2,
  [CurrencyCodes.AZN]: 2,
  [CurrencyCodes.BAM]: 2,
  [CurrencyCodes.BBD]: 2,
  [CurrencyCodes.BDT]: 2,
  [CurrencyCodes.BGN]: 2,
  [CurrencyCodes.BHD]: 3,
  [CurrencyCodes.BIF]: 0,
  [CurrencyCodes.BMD]: 2,
  [CurrencyCodes.BND]: 2,
  [CurrencyCodes.BOB]: 2,
  [CurrencyCodes.BOV]: 2,
  [CurrencyCodes.BRL]: 2,
  [CurrencyCodes.BSD]: 2,
  [CurrencyCodes.BTN]: 2,
  [CurrencyCodes.BWP]: 2,
  [CurrencyCodes.BYR]: 0,
  [CurrencyCodes.BYN]: 2,
  [CurrencyCodes.BZD]: 2,
  [CurrencyCodes.CAD]: 2,
  [CurrencyCodes.CDF]: 2,
  [CurrencyCodes.CHE]: 2,
  [CurrencyCodes.CHF]: 2,
  [CurrencyCodes.CHW]: 2,
  [CurrencyCodes.CLF]: 0,
  [CurrencyCodes.CLP]: 0,
  [CurrencyCodes.CNY]: 2,
  [CurrencyCodes.COP]: 2,
  [CurrencyCodes.COU]: 2,
  [CurrencyCodes.CRC]: 2,
  [CurrencyCodes.CUC]: 2,
  [CurrencyCodes.CUP]: 2,
  [CurrencyCodes.CVE]: 2,
  [CurrencyCodes.CZK]: 2,
  [CurrencyCodes.DJF]: 0,
  [CurrencyCodes.DKK]: 2,
  [CurrencyCodes.DOP]: 2,
  [CurrencyCodes.DZD]: 2,
  [CurrencyCodes.EGP]: 2,
  [CurrencyCodes.ERN]: 2,
  [CurrencyCodes.ETB]: 2,
  [CurrencyCodes.EUR]: 2,
  [CurrencyCodes.FJD]: 2,
  [CurrencyCodes.FKP]: 2,
  [CurrencyCodes.GBP]: 2,
  [CurrencyCodes.GEL]: 2,
  [CurrencyCodes.GHS]: 2,
  [CurrencyCodes.GIP]: 2,
  [CurrencyCodes.GMD]: 2,
  [CurrencyCodes.GNF]: 0,
  [CurrencyCodes.GTQ]: 2,
  [CurrencyCodes.GYD]: 2,
  [CurrencyCodes.HKD]: 2,
  [CurrencyCodes.HNL]: 2,
  [CurrencyCodes.HRK]: 2,
  [CurrencyCodes.HTG]: 2,
  [CurrencyCodes.HUF]: 2,
  [CurrencyCodes.IDR]: 2,
  [CurrencyCodes.ILS]: 2,
  [CurrencyCodes.INR]: 2,
  [CurrencyCodes.IQD]: 3,
  [CurrencyCodes.IRR]: 2,
  [CurrencyCodes.ISK]: 0,
  [CurrencyCodes.JMD]: 2,
  [CurrencyCodes.JOD]: 3,
  [CurrencyCodes.JPY]: 0,
  [CurrencyCodes.KES]: 2,
  [CurrencyCodes.KGS]: 2,
  [CurrencyCodes.KHR]: 2,
  [CurrencyCodes.KMF]: 0,
  [CurrencyCodes.KPW]: 2,
  [CurrencyCodes.KRW]: 0,
  [CurrencyCodes.KWD]: 3,
  [CurrencyCodes.KYD]: 2,
  [CurrencyCodes.KZT]: 2,
  [CurrencyCodes.LAK]: 2,
  [CurrencyCodes.LBP]: 2,
  [CurrencyCodes.LKR]: 2,
  [CurrencyCodes.LRD]: 2,
  [CurrencyCodes.LSL]: 2,
  [CurrencyCodes.LTL]: 2,
  [CurrencyCodes.LVL]: 2,
  [CurrencyCodes.LYD]: 3,
  [CurrencyCodes.MAD]: 2,
  [CurrencyCodes.MDL]: 2,
  [CurrencyCodes.MGA]: 2,
  [CurrencyCodes.MKD]: 2,
  [CurrencyCodes.MMK]: 2,
  [CurrencyCodes.MNT]: 2,
  [CurrencyCodes.MOP]: 2,
  [CurrencyCodes.MRO]: 2,
  [CurrencyCodes.MUR]: 2,
  [CurrencyCodes.MVR]: 2,
  [CurrencyCodes.MWK]: 2,
  [CurrencyCodes.MXN]: 2,
  [CurrencyCodes.MXV]: 2,
  [CurrencyCodes.MYR]: 2,
  [CurrencyCodes.MZN]: 2,
  [CurrencyCodes.NAD]: 2,
  [CurrencyCodes.NGN]: 2,
  [CurrencyCodes.NIO]: 2,
  [CurrencyCodes.NOK]: 2,
  [CurrencyCodes.NPR]: 2,
  [CurrencyCodes.NZD]: 2,
  [CurrencyCodes.OMR]: 3,
  [CurrencyCodes.PAB]: 2,
  [CurrencyCodes.PEN]: 2,
  [CurrencyCodes.PGK]: 2,
  [CurrencyCodes.PHP]: 2,
  [CurrencyCodes.PKR]: 2,
  [CurrencyCodes.PLN]: 2,
  [CurrencyCodes.PYG]: 0,
  [CurrencyCodes.QAR]: 2,
  [CurrencyCodes.RON]: 2,
  [CurrencyCodes.RSD]: 2,
  [CurrencyCodes.RUB]: 2,
  [CurrencyCodes.RWF]: 0,
  [CurrencyCodes.SAR]: 2,
  [CurrencyCodes.SBD]: 2,
  [CurrencyCodes.SCR]: 2,
  [CurrencyCodes.SDG]: 2,
  [CurrencyCodes.SEK]: 2,
  [CurrencyCodes.SGD]: 2,
  [CurrencyCodes.SHP]: 2,
  [CurrencyCodes.SLL]: 2,
  [CurrencyCodes.SOS]: 2,
  [CurrencyCodes.SRD]: 2,
  [CurrencyCodes.SSP]: 2,
  [CurrencyCodes.STD]: 2,
  [CurrencyCodes.SVC]: 2,
  [CurrencyCodes.SYP]: 2,
  [CurrencyCodes.SZL]: 2,
  [CurrencyCodes.THB]: 2,
  [CurrencyCodes.TJS]: 2,
  [CurrencyCodes.TMT]: 2,
  [CurrencyCodes.TND]: 3,
  [CurrencyCodes.TOP]: 2,
  [CurrencyCodes.TRY]: 2,
  [CurrencyCodes.TTD]: 2,
  [CurrencyCodes.TWD]: 2,
  [CurrencyCodes.TZS]: 2,
  [CurrencyCodes.UAH]: 2,
  [CurrencyCodes.UGX]: 0,
  [CurrencyCodes.USD]: 2,
  [CurrencyCodes.USN]: 2,
  [CurrencyCodes.USS]: 2,
  [CurrencyCodes.UYI]: 0,
  [CurrencyCodes.UYU]: 2,
  [CurrencyCodes.UZS]: 2,
  [CurrencyCodes.VEF]: 2,
  [CurrencyCodes.VND]: 0,
  [CurrencyCodes.VUV]: 0,
  [CurrencyCodes.WST]: 2,
  [CurrencyCodes.XAF]: 0,
  [CurrencyCodes.XAG]: 0,
  [CurrencyCodes.XAU]: 0,
  [CurrencyCodes.XBA]: 0,
  [CurrencyCodes.XBB]: 0,
  [CurrencyCodes.XBC]: 0,
  [CurrencyCodes.XBD]: 0,
  [CurrencyCodes.XCD]: 2,
  [CurrencyCodes.XDR]: 0,
  [CurrencyCodes.XFU]: 0,
  [CurrencyCodes.XOF]: 0,
  [CurrencyCodes.XPD]: 0,
  [CurrencyCodes.XPF]: 0,
  [CurrencyCodes.XPT]: 0,
  [CurrencyCodes.XSU]: 0,
  [CurrencyCodes.XTS]: 0,
  [CurrencyCodes.XUA]: 0,
  [CurrencyCodes.YER]: 2,
  [CurrencyCodes.ZAR]: 2,
  [CurrencyCodes.ZMW]: 2,
  [CurrencyCodes.ZWL]: 2,
};

// This function has been modified to not use Decimal
function convertToMajorCurrencyUnits(minorUnitValue: number, currency: CurrencyCodes): number {
  const exponent = CurrencyExponents[currency];
  if (exponent == null) {
    console.warn(`Unexpected currency ${currency}`);
    return minorUnitValue;
  }
  //const minorUnit = new Decimal(minorUnitValue);
  //return minorUnit.dividedBy(10 ** exponent).toNumber();
  return minorUnitValue / 10 ** exponent;
}


//# PATCH URL MAPPINGS - - - - -
const SUBSTITUTION_REGEX = /\{([a-z]+)\}/g;
const PROXY_PREFIX = '/.proxy';

function recursivelyRemapChildNodes(node: Node, mappings: Mapping[]) {
    if (node.hasChildNodes()) {
      node.childNodes.forEach((child) => {
        attemptSetNodeSrc(child, mappings);
        recursivelyRemapChildNodes(child, mappings);
      });
    }
}

function attemptSetNodeSrc(node: Node, mappings: Mapping[]) {
    if (node instanceof HTMLElement && node.hasAttribute('src')) {
      const rawSrc = node.getAttribute('src');
      const url = absoluteURL(rawSrc ?? '');
      if (url.host === window.location.host) return;

      if (node.tagName.toLowerCase() === 'script') {
        // Scripts are a special case, and need to be wholly recreated since
        // modifying a script tag doesn't refetch.
        attemptRecreateScriptNode(node, {url, mappings});
      } else {
        const newSrc = attemptRemap({url, mappings}).toString();
        // Only apply the remapping if we actually remapped the value
        if (newSrc !== rawSrc) {
          node.setAttribute('src', newSrc);
        }
      }
    }
}

function attemptRecreateScriptNode(node: HTMLElement, {url, mappings}: RemapInput) {
    const newUrl = attemptRemap({url, mappings});
    if (url.toString() !== newUrl.toString()) {
      // Note: Script tags cannot be duplicated via `node.clone()` because their internal 'already started'
      // state prevents the new one from being fetched. We must manually recreate the duplicate tag instead.
      const newNode = document.createElement(node.tagName);
      newNode.innerHTML = node.innerHTML;
      for (const attr of node.attributes) {
        newNode.setAttribute(attr.name, attr.value);
      }
      newNode.setAttribute('src', attemptRemap({url, mappings}).toString());
      node.after(newNode);
      node.remove();
    }
}

function attemptRemap({url, mappings}: RemapInput): URL {
    const newURL = new URL(url.toString());
    if (
      (newURL.hostname.includes('discordsays.com') || newURL.hostname.includes('discordsez.com')) &&
      // Only apply proxy prefix once
      !newURL.pathname.startsWith(PROXY_PREFIX)
    ) {
      newURL.pathname = PROXY_PREFIX + newURL.pathname;
    }
    for (const mapping of mappings) {
      const mapped = matchAndRewriteURL({
        originalURL: newURL,
        prefix: mapping.prefix,
        target: mapping.target,
        prefixHost: window.location.host,
      });
      if (mapped != null && mapped?.toString() !== url.toString()) {
        return mapped;
      }
    }
    return newURL;
}

function absoluteURL(
    url: string,
    protocol: string = window.location.protocol,
    host: string = window.location.host,
  ): URL {
    // If the first arg is a complete url, it will ignore the second arg
    // This call structure lets us set relative urls to have a full url with the proper protocol and host
    return new URL(url, `${protocol}//${host}`);
}

function matchAndRewriteURL({originalURL, prefix, prefixHost, target}: MatchAndRewriteURLInputs): URL | null {
    // coerce url with filler https protocol so we can retrieve host and pathname from target
    const targetURL = new URL(`https://${target}`);
    // Depending on the environment, the URL constructor may turn `{` and `}` into `%7B` and `%7D`, respectively
    const targetRegEx = regexFromTarget(targetURL.host.replace(/%7B/g, '{').replace(/%7D/g, '}'));
    const match = originalURL.toString().match(targetRegEx);
    // Null match indicates that this target is not relevant
    if (match == null) return originalURL;
    const newURL = new URL(originalURL.toString());
    newURL.host = prefixHost;
    newURL.pathname = prefix.replace(SUBSTITUTION_REGEX, (_, matchName) => {
      const replaceValue = match.groups?.[matchName];
      if (replaceValue == null) throw new Error('Misconfigured route.');
      return replaceValue;
    });
  
    // Append the original path
    newURL.pathname += newURL.pathname === '/' ? originalURL.pathname.slice(1) : originalURL.pathname;
    // prepend /.proxy/ to path if using discord activities proxy
    if (
      (newURL.hostname.includes('discordsays.com') || newURL.hostname.includes('discordsez.com')) &&
      !newURL.pathname.startsWith(PROXY_PREFIX)
    ) {
      newURL.pathname = PROXY_PREFIX + newURL.pathname;
    }
    // Remove the target's path from the new url path
    newURL.pathname = newURL.pathname.replace(targetURL.pathname, '');
    // Add a trailing slash if original url had it, and if it doesn't already have one or if matches filename regex
    if (originalURL.pathname.endsWith('/') && !newURL.pathname.endsWith('/')) {
      newURL.pathname += '/';
    }
    return newURL;
}

function regexFromTarget(target: string): RegExp {
  const regexString = target.replace(SUBSTITUTION_REGEX, (match, name) => `(?<${name}>[\\w-]+)`);
  return new RegExp(`${regexString}(/|$)`);
}