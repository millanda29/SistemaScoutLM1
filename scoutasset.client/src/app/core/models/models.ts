export interface LoginRequest {
  userName: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userName: string;
  email: string;
  roles: string[];
}

export interface Category {
  id: number;
  name: string;
  description?: string;
  prefix: string;
  nextNumber: number;
  isActive: boolean;
}

export interface Location {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
}

export interface Resource {
  id: number;
  code: string;
  name: string;
  description?: string;
  categoryId: number;
  category?: string;
  brand?: string;
  model?: string;
  serialNumber?: string;
  acquisitionDate?: string;
  acquisitionType: string;
  acquisitionCost?: number;
  appraisedValue?: number;
  physicalCondition: string;
  administrativeStatus: string;
  locationId: number;
  location?: string;
  currentResponsibleId?: string;
  observations?: string;
  createdAt: string;
}

export interface Loan {
  id: number;
  requestNumber: string;
  requesterId: string;
  approverId?: string;
  status: string;
  reason: string;
  approvalType?: string;
  expectedReturnDate: string;
  actualDeliveryDate?: string;
  actualReturnDate?: string;
  rejectionReason?: string;
  items: LoanItem[];
  createdAt: string;
  requesterType?: string;
  pdfDocumentPath?: string;
  loanDate?: string;
}

export interface LoanItem {
  id: number;
  loanId: number;
  resourceId: number;
  conditionAtDelivery: string;
  conditionAtReturn?: string;
  returnObservations?: string;
  damagesDetected?: string;
  resource?: Resource;
}

export interface Maintenance {
  id: number;
  resourceId: number;
  resourceCode?: string;
  resourceName?: string;
  type: string;
  status: string;
  description: string;
  scheduledDate: string;
  completedDate?: string;
  cost?: number;
  result?: string;
  nextMaintenanceDate?: string;
}

export interface PhysicalInventory {
  id: number;
  inventoryNumber: string;
  status: string;
  description?: string;
  startDate: string;
  endDate?: string;
  locations: { locationId: number; locationName?: string }[];
  items: InventoryItem[];
}

export interface InventoryItem {
  id: number;
  physicalInventoryId: number;
  resourceId?: number;
  locationId: number;
  result: string;
  physicalCondition?: string;
  foundCode?: string;
  foundName?: string;
  observations?: string;
  verifiedAt: string;
}

export interface Loss {
  id: number;
  resourceId: number;
  resourceCode?: string;
  resourceName?: string;
  lossNumber: string;
  status: string;
  circumstances: string;
  reportedById: string;
  confirmedAt?: string;
  recoveredAt?: string;
}

export interface Retirement {
  id: number;
  resourceId: number;
  resourceCode?: string;
  resourceName?: string;
  retirementNumber: string;
  status: string;
  reason: string;
  requestedById: string;
  authorizedById?: string;
  authorizedAt?: string;
  executedAt?: string;
  rejectionReason?: string;
}

export interface CategoryStat {
  category: string;
  count: number;
  disponibles: number;
}

export interface RecentMovement {
  resourceCode: string;
  resourceName: string;
  type: string;
  description: string;
  performedAt: string;
}

export interface DashboardData {
  total: number;
  disponibles: number;
  prestados: number;
  enMantenimiento: number;
  noLocalizados: number;
  perdidos: number;
  dadosDeBaja: number;
  daniados: number;
  prestamosVencidos: number;
  totalAcquisitionCost?: number;
  totalAppraisedValue?: number;
  categoryStats?: CategoryStat[];
  recentMovements?: RecentMovement[];
}

export interface User {
  id: string;
  userName: string;
  email: string;
  emailConfirmed: boolean;
  lockoutEnabled: boolean;
  lockoutEnd?: string | null;
  accessFailedCount: number;
  roles: string[];
  ci?: string;
  nombres?: string;
  apellidos?: string;
  telefono?: string;
}

export interface Movement {
  id: number;
  resourceId: number;
  resourceCode?: string;
  resourceName?: string;
  type: string;
  description: string;
  previousStatus?: string;
  newStatus?: string;
  performedAt: string;
  performedById: string;
}

export interface Dirigente {
  id: number;
  ci: string;
  nombres: string;
  apellidos: string;
  correo: string;
  telefono: string;
  habilitadoParaCustodio: boolean;
  isActive: boolean;
}
