import { createRouter, createWebHistory } from 'vue-router'

const APP_NAME = 'TaskFlow'

const routes = [
  {
    path: '/',
    name: 'dashboard',
    component: () => import('@/views/DashboardView.vue'),
    meta: { title: 'Dashboard' },
  },
  {
    path: '/work-items',
    name: 'work-items',
    component: () => import('@/views/WorkItemsView.vue'),
    meta: { title: 'Work items' },
  },
  {
    path: '/assignments',
    name: 'assignments',
    component: () => import('@/views/AssignmentsView.vue'),
    meta: { title: 'Assignments' },
  },
  {
    path: '/employees',
    name: 'employees',
    component: () => import('@/views/EmployeesView.vue'),
    meta: { title: 'Employees' },
  },
  {
    path: '/task-types',
    name: 'task-types',
    component: () => import('@/views/TaskTypesView.vue'),
    meta: { title: 'Task types' },
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'not-found',
    component: () => import('@/views/NotFoundView.vue'),
    meta: { title: 'Page not found' },
  },
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  linkExactActiveClass: 'active',
})

router.afterEach((to) => {
  document.title = to.meta.title ? `${to.meta.title} · ${APP_NAME}` : APP_NAME
})

export default router