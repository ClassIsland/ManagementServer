import { createAlova } from 'alova';
import VueHook from 'alova/vue';
import { isString } from 'lodash-es';
import { useUser } from '@/store/modules/user';
import { storage } from '@/utils/Storage';
import { useGlobSetting } from '@/hooks/setting';
import { PageEnum } from '@/enums/pageEnum';
import { ResultEnum } from '@/enums/httpEnum';
import { isUrl } from '@/utils';
import adapterFetch from 'alova/fetch';


const {  apiUrl, urlPrefix } = useGlobSetting();

export const Alova = createAlova({
  baseURL: apiUrl,
  statesHook: VueHook,
  // 关闭全局请求缓存
  cacheFor: null,
  // 在开发环境开启缓存命中日志
  cacheLogger: process.env.NODE_ENV === 'development',
  requestAdapter: adapterFetch(),
  beforeRequest(method) {
    const userStore = useUser();
    const token = userStore.getToken;
    // 添加 token 到请求头
    if (!method.meta?.ignoreToken && token) {
      method.config.headers['Authorization'] = "Bearer " + token;
    }
    // 处理 api 请求前缀
    const isUrlStr = isUrl(method.url as string);
    if (!isUrlStr && urlPrefix) {
      method.url = `${urlPrefix}${method.url}`;
    }
    if (!isUrlStr && apiUrl && isString(apiUrl)) {
      method.url = `${apiUrl}${method.url}`;
    }
  },
  responded: {
    onSuccess: async (response, method) => {
      let resRaw = null;
      try {
        resRaw = await response.json()
      } catch (e) {
        
      }
      const res = resRaw || response.body;
      // 不进行任何处理，直接返回
      // 用于需要直接获取 code、result、 message 这些信息时开启
      if (method.meta?.isTransformResponse === false) {
        return res;
      }
      // @ts-ignore
      const Message = window.$message;
      // @ts-ignore
      const Modal = window.$dialog;
      const code = response.status;

      const LoginPath = PageEnum.BASE_LOGIN;
      if (ResultEnum.SUCCESS === code) {
        return res;
      }
      // 需要登录
      console.log(method)
      if (code === 401 && !method.meta?.ignoreAuth) {
        Modal?.warning({
          title: '提示',
          content: '登录身份已失效，请重新登录!',
          positiveText: '确定',
          closable: false,
          maskClosable: false,

          onPositiveClick: async () => {
            await useUser().logout();
            window.location.href = LoginPath;
          },
        });
      } else {
        // 可按需处理错误 一般情况下不是 912 错误，不一定需要弹出 message
        Message?.error(res.message ?? code);
        throw new Error(code);
      }
      
      return res;
      
    },
  },
});

// 项目，多个不同 api 地址，可导出多个实例
// export const AlovaTwo = createAlova({
//   baseURL: 'http://localhost:9001',
// });
