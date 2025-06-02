import { h } from 'vue';
import { NAvatar, NCheckbox } from 'naive-ui';
import { BasicColumn } from '@/components/Table';
import {
  AbstractClient,
  ClassPlan,
  Client,
  ClientGroup,
  ObjectsAssignee,
  Subject
} from "@/api/globals";
import ClientGroupIndicator from "@/components/ClientGroupIndicator/index.vue";


export const columns: BasicColumn<ObjectsAssignee>[] = [
  {
    title: '目标',
    key: 'id',
    render: (record) => {
      switch (record.assigneeType) {
        case 1:
          return record.targetClientCuid;
        case 2:
          return record.targetClientId;
        case 3:
          return record.targetGroup?.name;
      }
      return "？？？"
    }
  },
  {
    title: '类型',
    key: 'assigneeType',
    render: (record) => {
      switch (record.assigneeType) {
        case 1:
          return "CUID";
        case 2:
          return "客户端 ID";
        case 3:
          return "客户端组";
      }
      return "？？？"
    }
  }
];


export const columnsClient: BasicColumn[] = [
  {
    title: '已分配',
    key: 'hasAssignee',
    render(record) {
      return h(NCheckbox, {
        checked: record.hasAssignee,
        onUpdateChecked(checked: boolean) {
          record.hasAssignee = checked;
          record.updateHasAssignee();
        }
      });
    },
    width: 75
  },
  {
    title: '客户端 CUID',
    key: 'clientObject.cuid'
  },
  {
    title: '客户端 ID',
    key: 'clientObject.id'
  }
];

export const columnsAbstractClients: BasicColumn[] = [
  {
    title: '已分配',
    key: 'hasAssignee',
    render(record) {
      return h(NCheckbox, {
        checked: record.hasAssignee,
        onUpdateChecked(checked: boolean) {
          record.hasAssignee = checked;
          record.updateHasAssignee();
        }
      });
    },
    width: 75
  },
  {
    title: 'ID',
    key: 'clientObject.id',
  },
  {
    'title': '分组',
    'key': 'group',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data.clientObject.group,
      });
    }
  }
];

export const columnsGroups: BasicColumn[] = [
  {
    title: '已分配',
    key: 'hasAssignee',
    render(record) {
      return h(NCheckbox, {
        checked: record.hasAssignee,
        onUpdateChecked(checked: boolean) {
          record.hasAssignee = checked;
          record.updateHasAssignee();
        }
      });
    },
    width: 75
  },
  {
    title: '名称',
    key: 'clientObject.name',
    render(data) {
      return h(ClientGroupIndicator, {
        group: data.clientObject,
      });
    }
  }
];

