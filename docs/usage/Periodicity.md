## Periodicity

DDS.Net.Connector.Types.Enumerations.**Periodicity** specifies the periodicity for data updates. Its enumeration includes following types:

  * *OnChange* - only when data value is changed
  * *Highest* - update is done every *BASE_TIME_SLOT_MS* x 1 millisecond
  * *High* - update is done every *BASE_TIME_SLOT_MS* x 2 millisecond
  * *Normal* - update is done every *BASE_TIME_SLOT_MS* x 4 millisecond
  * *Low* - update is done every *BASE_TIME_SLOT_MS* x 8 millisecond
  * *Lowest* - update is done every *BASE_TIME_SLOT_MS* x 16 millisecond

> *BASE_TIME_SLOT_MS* by default is set to *50*.



