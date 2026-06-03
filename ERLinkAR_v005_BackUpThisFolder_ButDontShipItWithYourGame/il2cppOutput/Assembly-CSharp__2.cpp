#include "pch-cpp.hpp"





template <typename R>
struct VirtualFuncInvoker0
{
	typedef R (*Func)(void*, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeObject* obj)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_virtual_invoke_data(slot, obj);
		return ((Func)invokeData.methodPtr)(obj, invokeData.method);
	}
};
template <typename T1, typename T2>
struct InterfaceActionInvoker2
{
	typedef void (*Action)(void*, T1, T2, const RuntimeMethod*);

	static inline void Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1, T2 p2)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		((Action)invokeData.methodPtr)(obj, p1, p2, invokeData.method);
	}
};
template <typename R, typename T1>
struct InterfaceFuncInvoker1
{
	typedef R (*Func)(void*, T1, const RuntimeMethod*);

	static inline R Invoke (Il2CppMethodSlot slot, RuntimeClass* declaringInterface, RuntimeObject* obj, T1 p1)
	{
		const VirtualInvokeData& invokeData = il2cpp_codegen_get_interface_invoke_data(slot, obj, declaringInterface);
		return ((Func)invokeData.methodPtr)(obj, p1, invokeData.method);
	}
};

struct Action_1_t6F9EB113EB3F16226AEF811A2744F4111C116C87;
struct Dictionary_2_t403063CE4960B4F46C688912237C6A27E550FF55;
struct Func_1_tD59A12717D79BFB403BF973694B1BE5B85474BD1;
struct Predicate_1_t8342C85FF4E41CD1F7024AC0CDC3E5312A32CB12;
struct Predicate_1_t7F48518B008C1472339EEEBABA3DE203FE1F26ED;
struct Task_1_t0C4CD3A5BB93A184420D73218644C56C70FDA7E2;
struct Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0;
struct Task_1_tE41CFF640EB7C045550D9D0D92BE67533B084C17;
struct ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031;
struct CharU5BU5D_t799905CF001DD5F13F7DBB310181FC4D8B7D0AAB;
struct IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832;
struct StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF;
struct Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07;
struct ContextCallback_tE8AFBDBFCC040FDA8DA8C1EEFE9BD66B16BDA007;
struct Delegate_t;
struct Exception_t;
struct HttpContent_tD09737BB27CB151BC9688882F785208620211E1C;
struct HttpContentHeaders_t4E2506F081BD682D0538A5CB38ED1D836C2E8C4F;
struct HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40;
struct HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970;
struct IAsyncStateMachine_t0680C7F905C553076B552D5A1A6E39E2F0F36AA2;
struct IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220;
struct SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6;
struct StackGuard_tACE063A1B7374BDF4AD472DE4585D05AD8745352;
struct String_t;
struct TaskFactory_tF781BD37BE23917412AD83424D1497C7C1509DF0;
struct TaskScheduler_t3F0550EBEF7C41F74EC8C08FF4BED0D8CE66006E;
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915;
struct FixedMemoryStream_tDCEF941500AF29277C42DDF40970FB21B4A66188;
struct ContingentProperties_t3FA59480914505CEA917B1002EC675F29D0CB540;

IL2CPP_EXTERN_C RuntimeClass* AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* Exception_t_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* HttpStatusCode_t530B6899270B44ED560C3872DB5F9698FB7D7374_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C RuntimeClass* IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220_il2cpp_TypeInfo_var;
IL2CPP_EXTERN_C String_t* _stringLiteral9088F268A1F162EAD32D98393F0A86ADFB081311;
IL2CPP_EXTERN_C String_t* _stringLiteralC1474C6BD4467CFD68B1CA36B9507822CB666C6E;
IL2CPP_EXTERN_C String_t* _stringLiteralC4D3BC3F281BAD104FC0396AA1AE83B0623B4368;
IL2CPP_EXTERN_C String_t* _stringLiteralCF76C852895B538714F94EF0D9FFC35C02DAD171;
IL2CPP_EXTERN_C String_t* _stringLiteralF94C4D62FF598B6FF81FB2096B94729F816C6758;
IL2CPP_EXTERN_C const RuntimeMethod* AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m81CD66DE1ADCDBC224C53AD2B890E1C7B130DA63_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Nullable_1__ctor_m1F490FAFDD40D3F3FD44351C18EB16E0008039D3_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* TaskAwaiter_1_GetResult_m82A392802A854576DC9525B87B0053B56201ABB9_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* TaskAwaiter_1_get_IsCompleted_mE207C5509602B0BB59366E53CED6CC9B10A1F8A8_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* Task_1_GetAwaiter_m7727657658441E9D4CE9D3F8B532F9D65CB9CE1F_RuntimeMethod_var;
IL2CPP_EXTERN_C const RuntimeMethod* U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878_RuntimeMethod_var;
struct Exception_t_marshaled_com;
struct Exception_t_marshaled_pinvoke;


IL2CPP_EXTERN_C_BEGIN
IL2CPP_EXTERN_C_END

#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA  : public RuntimeObject
{
};
struct HttpContent_tD09737BB27CB151BC9688882F785208620211E1C  : public RuntimeObject
{
	FixedMemoryStream_tDCEF941500AF29277C42DDF40970FB21B4A66188* ___buffer;
	bool ___disposed;
	HttpContentHeaders_t4E2506F081BD682D0538A5CB38ED1D836C2E8C4F* ___headers;
};
struct HttpRequestExceptionExtensions_t8025A8F562BA4A6AB479B8D62569F5CDDE4948F0  : public RuntimeObject
{
};
struct String_t  : public RuntimeObject
{
	int32_t ____stringLength;
	Il2CppChar ____firstChar;
};
struct Task_t751C4CC3ECD055BABA8A0B6A5DFBB4283DCA8572  : public RuntimeObject
{
	int32_t ___m_taskId;
	Delegate_t* ___m_action;
	RuntimeObject* ___m_stateObject;
	TaskScheduler_t3F0550EBEF7C41F74EC8C08FF4BED0D8CE66006E* ___m_taskScheduler;
	Task_t751C4CC3ECD055BABA8A0B6A5DFBB4283DCA8572* ___m_parent;
	int32_t ___m_stateFlags;
	RuntimeObject* ___m_continuationObject;
	ContingentProperties_t3FA59480914505CEA917B1002EC675F29D0CB540* ___m_contingentProperties;
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F  : public RuntimeObject
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_pinvoke
{
};
struct ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F_marshaled_com
{
};
struct TaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA 
{
	Task_1_t0C4CD3A5BB93A184420D73218644C56C70FDA7E2* ___m_task;
};
struct TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 
{
	Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0* ___m_task;
};
struct Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0  : public Task_t751C4CC3ECD055BABA8A0B6A5DFBB4283DCA8572
{
	String_t* ___m_result;
};
struct AsyncMethodBuilderCore_tD5ABB3A2536319A3345B32A5481E37E23DD8CEDF 
{
	RuntimeObject* ___m_stateMachine;
	Action_tD00B0A84D7945E50C2DFFC28EFEE6ED44ED2AD07* ___m_defaultContextAction;
};
struct AsyncMethodBuilderCore_tD5ABB3A2536319A3345B32A5481E37E23DD8CEDF_marshaled_pinvoke
{
	RuntimeObject* ___m_stateMachine;
	Il2CppMethodPointer ___m_defaultContextAction;
};
struct AsyncMethodBuilderCore_tD5ABB3A2536319A3345B32A5481E37E23DD8CEDF_marshaled_com
{
	RuntimeObject* ___m_stateMachine;
	Il2CppMethodPointer ___m_defaultContextAction;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22 
{
	bool ___m_value;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17 
{
	Il2CppChar ___m_value;
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2  : public ValueType_t6D9B272BD21782F0A9A14F2E41F85A50E97A986F
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_pinvoke
{
};
struct Enum_t2A1A94B24E3B776EEF4E5E485E290BB9D4D072E2_marshaled_com
{
};
struct Int32_t680FF22E76F6EFAD4375103CBBFFA0421349384C 
{
	int32_t ___m_value;
};
struct IntPtr_t 
{
	void* ___m_value;
};
struct UInt32_t1833D51FFA667B18A5AA4B8D34DE284F8495D29B 
{
	uint32_t ___m_value;
};
struct Void_t4861ACF8F4594C3437BB48B6E56783494B843915 
{
	union
	{
		struct
		{
		};
		uint8_t Void_t4861ACF8F4594C3437BB48B6E56783494B843915__padding[1];
	};
};
#pragma pack(push, tp, 1)
struct __StaticArrayInitTypeSizeU3D1280_t41B8ECB5BFF11D609C6723B3F3A766DE9F8CD3A7 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D1280_t41B8ECB5BFF11D609C6723B3F3A766DE9F8CD3A7__padding[1280];
	};
};
#pragma pack(pop, tp)
#pragma pack(push, tp, 1)
struct __StaticArrayInitTypeSizeU3D1900_t466E44B8483F908DA6A89B05D7497A8547850BCF 
{
	union
	{
		struct
		{
			union
			{
			};
		};
		uint8_t __StaticArrayInitTypeSizeU3D1900_t466E44B8483F908DA6A89B05D7497A8547850BCF__padding[1900];
	};
};
#pragma pack(pop, tp)
struct AsyncTaskMethodBuilder_1_tE88892A6B2F97B5D44B7C3EE2DBEED85743412AC 
{
	AsyncMethodBuilderCore_tD5ABB3A2536319A3345B32A5481E37E23DD8CEDF ___m_coreState;
	Task_1_tE41CFF640EB7C045550D9D0D92BE67533B084C17* ___m_task;
};
struct Exception_t  : public RuntimeObject
{
	String_t* ____className;
	String_t* ____message;
	RuntimeObject* ____data;
	Exception_t* ____innerException;
	String_t* ____helpURL;
	RuntimeObject* ____stackTrace;
	String_t* ____stackTraceString;
	String_t* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	RuntimeObject* ____dynamicMethods;
	int32_t ____HResult;
	String_t* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	IntPtrU5BU5D_tFD177F8C806A6921AD7150264CCC62FA00CAD832* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_pinvoke
{
	char* ____className;
	char* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_pinvoke* ____innerException;
	char* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	char* ____stackTraceString;
	char* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	char* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct Exception_t_marshaled_com
{
	Il2CppChar* ____className;
	Il2CppChar* ____message;
	RuntimeObject* ____data;
	Exception_t_marshaled_com* ____innerException;
	Il2CppChar* ____helpURL;
	Il2CppIUnknown* ____stackTrace;
	Il2CppChar* ____stackTraceString;
	Il2CppChar* ____remoteStackTraceString;
	int32_t ____remoteStackIndex;
	Il2CppIUnknown* ____dynamicMethods;
	int32_t ____HResult;
	Il2CppChar* ____source;
	SafeSerializationManager_tCBB85B95DFD1634237140CD892E82D06ECB3F5E6* ____safeSerializationManager;
	StackTraceU5BU5D_t32FBCB20930EAF5BAE3F450FF75228E5450DA0DF* ___captured_traces;
	Il2CppSafeArray* ___native_trace_ips;
	int32_t ___caught_in_unmanaged;
};
struct HttpStatusCode_t530B6899270B44ED560C3872DB5F9698FB7D7374 
{
	int32_t ___value__;
};
struct Int32Enum_tCBAC8BA2BFF3A845FA599F303093BBBA374B6F0C 
{
	int32_t ___value__;
};
struct Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD 
{
	bool ___hasValue;
	int32_t ___value;
};
struct Nullable_1_t163D49A1147F217B7BD43BE8ACC8A5CC6B846D14 
{
	bool ___hasValue;
	int32_t ___value;
};
struct AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06 
{
	AsyncTaskMethodBuilder_1_tE88892A6B2F97B5D44B7C3EE2DBEED85743412AC ___m_builder;
};
struct AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_marshaled_pinvoke
{
	AsyncTaskMethodBuilder_1_tE88892A6B2F97B5D44B7C3EE2DBEED85743412AC ___m_builder;
};
struct AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_marshaled_com
{
	AsyncTaskMethodBuilder_1_tE88892A6B2F97B5D44B7C3EE2DBEED85743412AC ___m_builder;
};
struct HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40  : public Exception_t
{
};
struct HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970  : public RuntimeObject
{
	String_t* ___reasonPhrase;
	int32_t ___statusCode;
	HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* ___U3CContentU3Ek__BackingField;
};
struct U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0 
{
	int32_t ___U3CU3E1__state;
	AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06 ___U3CU3Et__builder;
	HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* ___response;
	TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 ___U3CU3Eu__1;
};
struct U3CPrivateImplementationDetailsU3E_t0F5473E849A5A5185A9F4C5246F0C32816C49FCA_StaticFields
{
	__StaticArrayInitTypeSizeU3D1900_t466E44B8483F908DA6A89B05D7497A8547850BCF ___31013AB3E1CBFB28B91AC0F52BEC02450023689429EF4BBBCB0DE49D2D6E884E;
	__StaticArrayInitTypeSizeU3D1280_t41B8ECB5BFF11D609C6723B3F3A766DE9F8CD3A7 ___3E8402F4DE99F83914646B1665C26378AAA71FF631085140921EE6F83DDB880A;
};
struct String_t_StaticFields
{
	String_t* ___Empty;
};
struct Boolean_t09A6377A54BE2F9E6985A8149F19234FD7DDFE22_StaticFields
{
	String_t* ___TrueString;
	String_t* ___FalseString;
};
struct Char_t521A6F19B456D956AF452D926C32709DC03D6B17_StaticFields
{
	ByteU5BU5D_tA6237BF417AE52AD70CFB4EF24A7A82613DF9031* ___s_categoryForLatin1;
};
struct Exception_t_StaticFields
{
	RuntimeObject* ___s_EDILock;
};
struct AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_StaticFields
{
	Task_1_tE41CFF640EB7C045550D9D0D92BE67533B084C17* ___s_cachedCompleted;
};
#ifdef __clang__
#pragma clang diagnostic pop
#endif


IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR TaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA Task_1_GetAwaiter_mD80ED263BF3F1F8DBDBD177BA3401A0AAAFA38E3_gshared (Task_1_t0C4CD3A5BB93A184420D73218644C56C70FDA7E2* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool TaskAwaiter_1_get_IsCompleted_mEEBB09E26F4165A0F864D92E1890CFCD2C8CFD54_gshared (TaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m0B2E1CCEFF9769489829AE193DAF5304F70AA632_gshared (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* __this, TaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA* ___0_awaiter, U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* ___1_stateMachine, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR RuntimeObject* TaskAwaiter_1_GetResult_mA4A8A1F43A456B40DDA251D00026C60919AED85B_gshared (TaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void Nullable_1__ctor_m5100B58428BDAD8C79F3D8576B0C2E1D4F3924EB_gshared (Nullable_1_t163D49A1147F217B7BD43BE8ACC8A5CC6B846D14* __this, int32_t ___0_value, const RuntimeMethod* method) ;

IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR bool HttpResponseMessage_get_IsSuccessStatusCode_m2B9DA5ECF8EE760D5A1A511C798FA08EAA931B9C (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* HttpResponseMessage_get_Content_m2350C12EA59DAD014A59B17398E5B50F62202AF6_inline (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0* HttpContent_ReadAsStringAsync_m69166E8C01E4287FFBA3E8E41805FF068362BF2B (HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* __this, const RuntimeMethod* method) ;
inline TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 Task_1_GetAwaiter_m7727657658441E9D4CE9D3F8B532F9D65CB9CE1F (Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0* __this, const RuntimeMethod* method)
{
	return ((  TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 (*) (Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0*, const RuntimeMethod*))Task_1_GetAwaiter_mD80ED263BF3F1F8DBDBD177BA3401A0AAAFA38E3_gshared)(__this, method);
}
inline bool TaskAwaiter_1_get_IsCompleted_mE207C5509602B0BB59366E53CED6CC9B10A1F8A8 (TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6* __this, const RuntimeMethod* method)
{
	return ((  bool (*) (TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6*, const RuntimeMethod*))TaskAwaiter_1_get_IsCompleted_mEEBB09E26F4165A0F864D92E1890CFCD2C8CFD54_gshared)(__this, method);
}
inline void AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m81CD66DE1ADCDBC224C53AD2B890E1C7B130DA63 (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* __this, TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6* ___0_awaiter, U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* ___1_stateMachine, const RuntimeMethod* method)
{
	((  void (*) (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06*, TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6*, U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0*, const RuntimeMethod*))AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t0B808409CD8201F13AAC85F29D646518C4857BEA_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m0B2E1CCEFF9769489829AE193DAF5304F70AA632_gshared)(__this, ___0_awaiter, ___1_stateMachine, method);
}
inline String_t* TaskAwaiter_1_GetResult_m82A392802A854576DC9525B87B0053B56201ABB9 (TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6* __this, const RuntimeMethod* method)
{
	return ((  String_t* (*) (TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6*, const RuntimeMethod*))TaskAwaiter_1_GetResult_mA4A8A1F43A456B40DDA251D00026C60919AED85B_gshared)(__this, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m9E3155FB84015C823606188F53B47CB44C444991 (String_t* ___0_str0, String_t* ___1_str1, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t HttpResponseMessage_get_StatusCode_m63BE26E4C79137B35F3066C6BA6A5FF5F3D16AAA_inline (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* HttpResponseMessage_get_ReasonPhrase_mBF3A464D41137F5C0261AC1406D441F25C3B7656 (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987 (String_t* ___0_format, RuntimeObject* ___1_arg0, RuntimeObject* ___2_arg1, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR String_t* String_Concat_m8855A6DE10F84DA7F4EC113CADDB59873A25573B (String_t* ___0_str0, String_t* ___1_str1, String_t* ___2_str2, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void HttpRequestException__ctor_mF583393A0C841D522489165F032D87F0E4177AA4 (HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* __this, String_t* ___0_message, Exception_t* ___1_inner, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AsyncTaskMethodBuilder_SetException_mBE41863F0571E0177A15731294087DE45E1FC10B (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* __this, Exception_t* ___0_exception, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AsyncTaskMethodBuilder_SetResult_m76D8B84F0068257C1823B1200B00E58E0C8DDDDE (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878 (U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* __this, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void AsyncTaskMethodBuilder_SetStateMachine_mE52B5B6B076025592A7AB462E3D26FA434AEB795 (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* __this, RuntimeObject* ___0_stateMachine, const RuntimeMethod* method) ;
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CValidateHttpResponseU3Ed__1_SetStateMachine_m296F7A1AB459E1F948EE8F8F815781657A526A01 (U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* __this, RuntimeObject* ___0_stateMachine, const RuntimeMethod* method) ;
inline void Nullable_1__ctor_m1F490FAFDD40D3F3FD44351C18EB16E0008039D3 (Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD* __this, int32_t ___0_value, const RuntimeMethod* method)
{
	((  void (*) (Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD*, int32_t, const RuntimeMethod*))Nullable_1__ctor_m5100B58428BDAD8C79F3D8576B0C2E1D4F3924EB_gshared)(__this, ___0_value, method);
}
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Il2CppChar String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3 (String_t* __this, int32_t ___0_index, const RuntimeMethod* method) ;
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method) ;
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 84847
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878 (U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* __this, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m81CD66DE1ADCDBC224C53AD2B890E1C7B130DA63_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&TaskAwaiter_1_get_IsCompleted_mE207C5509602B0BB59366E53CED6CC9B10A1F8A8_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Task_1_GetAwaiter_m7727657658441E9D4CE9D3F8B532F9D65CB9CE1F_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralF94C4D62FF598B6FF81FB2096B94729F816C6758);
		s_Il2CppMethodInitialized = true;
	}
	int32_t V_0 = 0;
	String_t* V_1 = NULL;
	TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 V_2;
	memset((&V_2), 0, sizeof(V_2));
	Exception_t* V_3 = NULL;
	Exception_t* V_4 = NULL;
	il2cpp::utils::ExceptionSupportStack<RuntimeObject*, 2> __active_exceptions;
	{
		int32_t L_0 = __this->___U3CU3E1__state;
		V_0 = L_0;
	}
	try
	{
		{
			int32_t L_1 = V_0;
			if (!L_1)
			{
				goto IL_0032_1;
			}
		}
		{
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:45>
			HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_2 = __this->___response;
			NullCheck(L_2);
			bool L_3;
			L_3 = HttpResponseMessage_get_IsSuccessStatusCode_m2B9DA5ECF8EE760D5A1A511C798FA08EAA931B9C(L_2, NULL);
			if (!L_3)
			{
				goto IL_001c_1;
			}
		}
		{
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:47>
			goto IL_0122;
		}

IL_001c_1:
		{
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:51>
			V_1 = _stringLiteralF94C4D62FF598B6FF81FB2096B94729F816C6758;
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:52>
			HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_4 = __this->___response;
			NullCheck(L_4);
			HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* L_5;
			L_5 = HttpResponseMessage_get_Content_m2350C12EA59DAD014A59B17398E5B50F62202AF6_inline(L_4, NULL);
			if (!L_5)
			{
				goto IL_00b2_1;
			}
		}

IL_0032_1:
		{
		}
		try
		{
			{
				int32_t L_6 = V_0;
				if (!L_6)
				{
					goto IL_0078_2;
				}
			}
			{
				//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:56>
				HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_7 = __this->___response;
				NullCheck(L_7);
				HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* L_8;
				L_8 = HttpResponseMessage_get_Content_m2350C12EA59DAD014A59B17398E5B50F62202AF6_inline(L_7, NULL);
				NullCheck(L_8);
				Task_1_t3D7638C82ED289AF156EDBAE76842D8DF4C4A9E0* L_9;
				L_9 = HttpContent_ReadAsStringAsync_m69166E8C01E4287FFBA3E8E41805FF068362BF2B(L_8, NULL);
				NullCheck(L_9);
				TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 L_10;
				L_10 = Task_1_GetAwaiter_m7727657658441E9D4CE9D3F8B532F9D65CB9CE1F(L_9, Task_1_GetAwaiter_m7727657658441E9D4CE9D3F8B532F9D65CB9CE1F_RuntimeMethod_var);
				V_2 = L_10;
				bool L_11;
				L_11 = TaskAwaiter_1_get_IsCompleted_mE207C5509602B0BB59366E53CED6CC9B10A1F8A8((&V_2), TaskAwaiter_1_get_IsCompleted_mE207C5509602B0BB59366E53CED6CC9B10A1F8A8_RuntimeMethod_var);
				if (L_11)
				{
					goto IL_0094_2;
				}
			}
			{
				int32_t L_12 = 0;
				V_0 = L_12;
				__this->___U3CU3E1__state = L_12;
				TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 L_13 = V_2;
				__this->___U3CU3Eu__1 = L_13;
				Il2CppCodeGenWriteBarrier((void**)&(((&__this->___U3CU3Eu__1))->___m_task), (void*)NULL);
				AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* L_14 = (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06*)(&__this->___U3CU3Et__builder);
				il2cpp_codegen_runtime_class_init_inline(AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var);
				AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m81CD66DE1ADCDBC224C53AD2B890E1C7B130DA63(L_14, (&V_2), __this, AsyncTaskMethodBuilder_AwaitUnsafeOnCompleted_TisTaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6_TisU3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0_m81CD66DE1ADCDBC224C53AD2B890E1C7B130DA63_RuntimeMethod_var);
				goto IL_0135;
			}

IL_0078_2:
			{
				TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6 L_15 = __this->___U3CU3Eu__1;
				V_2 = L_15;
				TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6* L_16 = (TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6*)(&__this->___U3CU3Eu__1);
				il2cpp_codegen_initobj(L_16, sizeof(TaskAwaiter_1_t254638BB1FAD695D9A9542E098A189D438A000F6));
				int32_t L_17 = (-1);
				V_0 = L_17;
				__this->___U3CU3E1__state = L_17;
			}

IL_0094_2:
			{
				String_t* L_18;
				L_18 = TaskAwaiter_1_GetResult_m82A392802A854576DC9525B87B0053B56201ABB9((&V_2), ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&TaskAwaiter_1_GetResult_m82A392802A854576DC9525B87B0053B56201ABB9_RuntimeMethod_var)));
				V_1 = L_18;
				//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:57>
				goto IL_00b2_1;
			}
		}
		catch(Il2CppExceptionWrapper& e)
		{
			if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
			{
				IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
				goto CATCH_009e_1;
			}
			throw e;
		}

CATCH_009e_1:
		{
			Exception_t* L_19 = ((Exception_t*)IL2CPP_GET_ACTIVE_EXCEPTION(Exception_t*));;
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:58>
			V_3 = L_19;
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:61>
			Exception_t* L_20 = V_3;
			NullCheck(L_20);
			String_t* L_21;
			L_21 = VirtualFuncInvoker0< String_t* >::Invoke(5, L_20);
			String_t* L_22;
			L_22 = String_Concat_m9E3155FB84015C823606188F53B47CB44C444991(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralCF76C852895B538714F94EF0D9FFC35C02DAD171)), L_21, NULL);
			V_1 = L_22;
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:62>
			IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
			goto IL_00b2_1;
		}

IL_00b2_1:
		{
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:66>
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:67>
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:68>
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:69>
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:70>
			HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_23 = __this->___response;
			NullCheck(L_23);
			int32_t L_24;
			L_24 = HttpResponseMessage_get_StatusCode_m63BE26E4C79137B35F3066C6BA6A5FF5F3D16AAA_inline(L_23, NULL);
			int32_t L_25 = ((int32_t)L_24);
			RuntimeObject* L_26 = Box(il2cpp_defaults.int32_class, &L_25);
			HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_27 = __this->___response;
			NullCheck(L_27);
			String_t* L_28;
			L_28 = HttpResponseMessage_get_ReasonPhrase_mBF3A464D41137F5C0261AC1406D441F25C3B7656(L_27, NULL);
			String_t* L_29;
			L_29 = String_Format_mFB7DA489BD99F4670881FF50EC017BFB0A5C0987(((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteral9088F268A1F162EAD32D98393F0A86ADFB081311)), L_26, L_28, NULL);
			String_t* L_30 = V_1;
			String_t* L_31;
			L_31 = String_Concat_m8855A6DE10F84DA7F4EC113CADDB59873A25573B(L_29, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralC4D3BC3F281BAD104FC0396AA1AE83B0623B4368)), L_30, NULL);
			HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* L_32 = (HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40*)il2cpp_codegen_object_new(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40_il2cpp_TypeInfo_var)));
			HttpRequestException__ctor_mF583393A0C841D522489165F032D87F0E4177AA4(L_32, L_31, (Exception_t*)NULL, NULL);
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:71>
			HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* L_33 = L_32;
			NullCheck(L_33);
			RuntimeObject* L_34;
			L_34 = VirtualFuncInvoker0< RuntimeObject* >::Invoke(6, L_33);
			HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* L_35 = __this->___response;
			NullCheck(L_35);
			int32_t L_36;
			L_36 = HttpResponseMessage_get_StatusCode_m63BE26E4C79137B35F3066C6BA6A5FF5F3D16AAA_inline(L_35, NULL);
			int32_t L_37 = L_36;
			RuntimeObject* L_38 = Box(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&HttpStatusCode_t530B6899270B44ED560C3872DB5F9698FB7D7374_il2cpp_TypeInfo_var)), &L_37);
			NullCheck(L_34);
			InterfaceActionInvoker2< RuntimeObject*, RuntimeObject* >::Invoke(1, ((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220_il2cpp_TypeInfo_var)), L_34, ((String_t*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&_stringLiteralC1474C6BD4467CFD68B1CA36B9507822CB666C6E)), L_38);
			//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:73>
			IL2CPP_RAISE_MANAGED_EXCEPTION(L_33, ((RuntimeMethod*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878_RuntimeMethod_var)));
		}
	}
	catch(Il2CppExceptionWrapper& e)
	{
		if(il2cpp_codegen_class_is_assignable_from (((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&Exception_t_il2cpp_TypeInfo_var)), il2cpp_codegen_object_class(e.ex)))
		{
			IL2CPP_PUSH_ACTIVE_EXCEPTION(e.ex);
			goto CATCH_0109;
		}
		throw e;
	}

CATCH_0109:
	{
		Exception_t* L_39 = ((Exception_t*)IL2CPP_GET_ACTIVE_EXCEPTION(Exception_t*));;
		V_4 = L_39;
		__this->___U3CU3E1__state = ((int32_t)-2);
		AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* L_40 = (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06*)(&__this->___U3CU3Et__builder);
		Exception_t* L_41 = V_4;
		il2cpp_codegen_runtime_class_init_inline(((RuntimeClass*)il2cpp_codegen_initialize_runtime_metadata_inline((uintptr_t*)&AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var)));
		AsyncTaskMethodBuilder_SetException_mBE41863F0571E0177A15731294087DE45E1FC10B(L_40, L_41, NULL);
		IL2CPP_POP_ACTIVE_EXCEPTION(Exception_t*);
		goto IL_0135;
	}

IL_0122:
	{
		//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:74>
		__this->___U3CU3E1__state = ((int32_t)-2);
		AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* L_42 = (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06*)(&__this->___U3CU3Et__builder);
		il2cpp_codegen_runtime_class_init_inline(AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var);
		AsyncTaskMethodBuilder_SetResult_m76D8B84F0068257C1823B1200B00E58E0C8DDDDE(L_42, NULL);
	}

IL_0135:
	{
		return;
	}
}
IL2CPP_EXTERN_C  void U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878_AdjustorThunk (RuntimeObject* __this, const RuntimeMethod* method)
{
	U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0*>(__this + _offset);
	U3CValidateHttpResponseU3Ed__1_MoveNext_mC77EE3E0DE889FAA47AC14A66DB71C5E6D7F4878(_thisAdjusted, method);
}
// Method Definition Index: 84848
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void U3CValidateHttpResponseU3Ed__1_SetStateMachine_m296F7A1AB459E1F948EE8F8F815781657A526A01 (U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* __this, RuntimeObject* ___0_stateMachine, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var);
		s_Il2CppMethodInitialized = true;
	}
	{
		AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06* L_0 = (AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06*)(&__this->___U3CU3Et__builder);
		RuntimeObject* L_1 = ___0_stateMachine;
		il2cpp_codegen_runtime_class_init_inline(AsyncTaskMethodBuilder_t7A5128C134547B5918EB1AA24FE47ED4C1DF3F06_il2cpp_TypeInfo_var);
		AsyncTaskMethodBuilder_SetStateMachine_mE52B5B6B076025592A7AB462E3D26FA434AEB795(L_0, L_1, NULL);
		return;
	}
}
IL2CPP_EXTERN_C  void U3CValidateHttpResponseU3Ed__1_SetStateMachine_m296F7A1AB459E1F948EE8F8F815781657A526A01_AdjustorThunk (RuntimeObject* __this, RuntimeObject* ___0_stateMachine, const RuntimeMethod* method)
{
	U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0* _thisAdjusted;
	int32_t _offset = 1;
	_thisAdjusted = reinterpret_cast<U3CValidateHttpResponseU3Ed__1_tFEA4F81FF8F7EE90229460EAE9CAC2ACAA70C4D0*>(__this + _offset);
	U3CValidateHttpResponseU3Ed__1_SetStateMachine_m296F7A1AB459E1F948EE8F8F815781657A526A01(_thisAdjusted, ___0_stateMachine, method);
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 84849
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD HttpRequestExceptionExtensions_GetStatusCode_mBE168627DFB98AA3FD5734FA7552AEC77EFA4594 (HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* ___0_exception, const RuntimeMethod* method) 
{
	static bool s_Il2CppMethodInitialized;
	if (!s_Il2CppMethodInitialized)
	{
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&HttpStatusCode_t530B6899270B44ED560C3872DB5F9698FB7D7374_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220_il2cpp_TypeInfo_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&Nullable_1__ctor_m1F490FAFDD40D3F3FD44351C18EB16E0008039D3_RuntimeMethod_var);
		il2cpp_codegen_initialize_runtime_metadata((uintptr_t*)&_stringLiteralC1474C6BD4467CFD68B1CA36B9507822CB666C6E);
		s_Il2CppMethodInitialized = true;
	}
	Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD V_0;
	memset((&V_0), 0, sizeof(V_0));
	{
		//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:82>
		HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* L_0 = ___0_exception;
		NullCheck(L_0);
		RuntimeObject* L_1;
		L_1 = VirtualFuncInvoker0< RuntimeObject* >::Invoke(6, L_0);
		NullCheck(L_1);
		bool L_2;
		L_2 = InterfaceFuncInvoker1< bool, RuntimeObject* >::Invoke(2, IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220_il2cpp_TypeInfo_var, L_1, _stringLiteralC1474C6BD4467CFD68B1CA36B9507822CB666C6E);
		if (!L_2)
		{
			goto IL_002d;
		}
	}
	{
		//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:84>
		HttpRequestException_t4460572C60D2686D9713A867A73B238DB3C1BB40* L_3 = ___0_exception;
		NullCheck(L_3);
		RuntimeObject* L_4;
		L_4 = VirtualFuncInvoker0< RuntimeObject* >::Invoke(6, L_3);
		NullCheck(L_4);
		RuntimeObject* L_5;
		L_5 = InterfaceFuncInvoker1< RuntimeObject*, RuntimeObject* >::Invoke(0, IDictionary_t6D03155AF1FA9083817AA5B6AD7DEEACC26AB220_il2cpp_TypeInfo_var, L_4, _stringLiteralC1474C6BD4467CFD68B1CA36B9507822CB666C6E);
		Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD L_6;
		memset((&L_6), 0, sizeof(L_6));
		Nullable_1__ctor_m1F490FAFDD40D3F3FD44351C18EB16E0008039D3((&L_6), ((*(int32_t*)UnBox(L_5, HttpStatusCode_t530B6899270B44ED560C3872DB5F9698FB7D7374_il2cpp_TypeInfo_var))), Nullable_1__ctor_m1F490FAFDD40D3F3FD44351C18EB16E0008039D3_RuntimeMethod_var);
		return L_6;
	}

IL_002d:
	{
		//<source_info:D:/ERLink-AR-Mobile-main/ERLink-AR-Mobile-main/Assets/Firebase/FirebaseApp/Internal/HttpHelpers.cs:86>
		il2cpp_codegen_initobj((&V_0), sizeof(Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD));
		Nullable_1_t125B11C44516F77D142E48132EE59127A46CC8BD L_7 = V_0;
		return L_7;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
// Method Definition Index: 84850
IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR uint32_t U3CPrivateImplementationDetailsU3E_ComputeStringHash_m6EA1F233618497AEFF8902A5EDFA24C74E2F2876 (String_t* ___0_s, const RuntimeMethod* method) 
{
	uint32_t V_0 = 0;
	int32_t V_1 = 0;
	{
		String_t* L_0 = ___0_s;
		if (!L_0)
		{
			goto IL_002a;
		}
	}
	{
		V_0 = ((int32_t)-2128831035);
		V_1 = 0;
		goto IL_0021;
	}

IL_000d:
	{
		String_t* L_1 = ___0_s;
		int32_t L_2 = V_1;
		NullCheck(L_1);
		Il2CppChar L_3;
		L_3 = String_get_Chars_mC49DF0CD2D3BE7BE97B3AD9C995BE3094F8E36D3(L_1, L_2, NULL);
		uint32_t L_4 = V_0;
		V_0 = ((int32_t)il2cpp_codegen_multiply(((int32_t)((int32_t)L_3^(int32_t)L_4)), ((int32_t)16777619)));
		int32_t L_5 = V_1;
		V_1 = ((int32_t)il2cpp_codegen_add(L_5, 1));
	}

IL_0021:
	{
		int32_t L_6 = V_1;
		String_t* L_7 = ___0_s;
		NullCheck(L_7);
		int32_t L_8;
		L_8 = String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline(L_7, NULL);
		if ((((int32_t)L_6) < ((int32_t)L_8)))
		{
			goto IL_000d;
		}
	}

IL_002a:
	{
		uint32_t L_9 = V_0;
		return L_9;
	}
}
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
#ifdef __clang__
#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Winvalid-offsetof"
#pragma clang diagnostic ignored "-Wunused-variable"
#endif
#ifdef __clang__
#pragma clang diagnostic pop
#endif
// Method Definition Index: 93247
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* HttpResponseMessage_get_Content_m2350C12EA59DAD014A59B17398E5B50F62202AF6_inline (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) 
{
	{
		HttpContent_tD09737BB27CB151BC9688882F785208620211E1C* L_0 = __this->___U3CContentU3Ek__BackingField;
		return L_0;
	}
}
// Method Definition Index: 93250
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t HttpResponseMessage_get_StatusCode_m63BE26E4C79137B35F3066C6BA6A5FF5F3D16AAA_inline (HttpResponseMessage_t5D2737606E4036A6E3E50FB0D651D3F76C61A970* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->___statusCode;
		return L_0;
	}
}
// Method Definition Index: 434
IL2CPP_MANAGED_FORCE_INLINE IL2CPP_METHOD_ATTR int32_t String_get_Length_m42625D67623FA5CC7A44D47425CE86FB946542D2_inline (String_t* __this, const RuntimeMethod* method) 
{
	{
		int32_t L_0 = __this->____stringLength;
		return L_0;
	}
}
