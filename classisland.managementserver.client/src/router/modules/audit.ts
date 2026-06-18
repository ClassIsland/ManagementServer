import {RouteRecordRaw} from "vue-router";
import {Layout} from "@/router/constant";
import {FileTextOutlined} from "@vicons/antd";
import { renderIcon } from '@/utils/index';

const routeName = "audit";

const routes: Array<RouteRecordRaw> = [
  {
    path: '/audit',
    name: routeName,
    component: Layout,
    meta: {
      title: '审计',
      sort: 99,
      icon: renderIcon(FileTextOutlined)
    },
    children: [
      {
        path: 'logs',
        name: `${routeName}_logs`,
        component: () => import("@/views/audit/index.vue"),
        meta: {
          title: '审计日志',
          icon: renderIcon(FileTextOutlined)
        },
      }
    ]
  }
];

export default routes;
