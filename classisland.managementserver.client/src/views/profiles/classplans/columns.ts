import { h } from 'vue';
import { NAvatar, NTag } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {ClassPlan, Client, Subject} from "@/api/globals";
import ClientGroupIndicator from "@/components/ClientGroupIndicator/index.vue";

export const columns: BasicColumn<ClassPlan>[] = [
  {
    title: '名称',
    key: 'name',
  },
  {
    title: '启用规则',
    key: 'weekDiv',
    render(data) {
      if (!data.isEnabled) {
        return h(NTag, {  }, { default: () => '不自动启用' });
      }
      let text = "???";
      if (data.weekDay == 1) {
        text = "周一";
      } else if (data.weekDay == 2) {
        text = "周二";
      } else if (data.weekDay == 3) {
        text = "周三";
      } else if (data.weekDay == 4) {
        text = "周四";
      } else if (data.weekDay == 5) {
        text = "周五";
      } else if (data.weekDay == 6) {
        text = "周六";
      } else if (data.weekDay == 0) {
        text = "周日";
      }
      if (data.weekDiv != 0) {
        text += ` ${data.weekDiv}/${data.weekCountDivTotal}周`;
      }
      return h(NTag, { type: 'info' }, { default: () => text });
    }
  },
  {
    'title': '分组',
    'key': 'group',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data.group,
      });
    }
  }
];
