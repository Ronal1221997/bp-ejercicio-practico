export interface Customer {
  customerId: number;
  personId: number;
  personName: string;
  status: boolean;
}

export interface CustomerRequest {
  personId: number;
  password: string;
  status: boolean;
}