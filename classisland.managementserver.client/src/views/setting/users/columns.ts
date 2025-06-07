import { h } from 'vue';
import { NTime } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {UserInfo} from "@/api/globals";
import roles from "@/models/roles";


export const columns: BasicColumn<UserInfo>[] = [
  {
    title: '昵称',
    key: 'name',
    width: 200
  },
  {
    title: '用户名',
    key: 'userName',
    width: 200
  },
  {
    title: "角色",
    key: 'roles',
    render(data) {
      const strings = [];
      data.roles.toSorted().forEach(v => {
        strings.push(roles.find(x => x.id === v)?.name ?? "")
      });
      return strings.join(", ");
    },
    width: 470
  },
  {
    title: '创建时间',
    key: 'createdTime',
    render(data) {
      return h(NTime, { time:data.createdTime })
    },
  }
];
